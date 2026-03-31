using Microsoft.Extensions.Caching.Distributed;
using TravelApp.Services.Flight.DTOs;
using System.Text.Json;
using TravelApp.Services.Flight.Interfaces;

namespace TravelApp.Services.Flight.Services;

public class FlightService : IFlightService
{
    private readonly IFlightRepository _repo;
    private readonly ILogger<FlightService> _logger;
    private readonly IDistributedCache _cache;

    public FlightService(IFlightRepository repo, ILogger<FlightService> logger, IDistributedCache cache)
    {
        _repo = repo;
        _logger = logger;
        _cache = cache;
    }

    public async Task<List<FlightOfferDto>> SearchFlightsAsync(string origin, string destination, string date, int adults = 1, string cabinClass = "ECONOMY")
    {
        try
        {
            var cacheKey = $"flights-{origin}-{destination}-{date}-{adults}-{cabinClass}".ToLower();
            var cachedFlights = await _cache.GetStringAsync(cacheKey);

            if (!string.IsNullOrEmpty(cachedFlights))
            {
                _logger.LogInformation("Cache hit for SearchFlights: {CacheKey}", cacheKey);
                return JsonSerializer.Deserialize<List<FlightOfferDto>>(cachedFlights) ?? new List<FlightOfferDto>();
            }

            _logger.LogInformation("Cache miss for SearchFlights: {CacheKey}. Fetching from API...", cacheKey);

            // 1. Business Logic: Map string cabin class to API specific integer
            int travelClassInt = cabinClass.ToUpper() switch
            {
                "PREMIUM_ECONOMY" => 2,
                "BUSINESS" => 3,
                "FIRST" => 4,
                _ => 1
            };

            // 2. Data Access: Call the repository
            var result = await _repo.GetFlightsRawAsync(origin, destination, date, adults, travelClassInt);
            if (result == null) return new List<FlightOfferDto>();

            // 3. Mapping Logic: Transform raw API models into our clean DTOs
            var flightOffers = new List<FlightOfferDto>();
            var allOptions = new List<SerpApiFlightOption>();

            if (result.best_flights != null) allOptions.AddRange(result.best_flights);
            if (result.other_flights != null) allOptions.AddRange(result.other_flights);

            var fallbackUrl = $"https://www.google.com/travel/flights?q=flights+{origin}+to+{destination}+{date}";
            var deepLink = result.search_metadata?.google_flights_url ?? fallbackUrl;

            foreach (var option in allOptions.Take(15))
            {
                var firstFlight = option.flights.FirstOrDefault();
                var lastFlight = option.flights.LastOrDefault();

                if (firstFlight == null || lastFlight == null) continue;

                var id = Guid.NewGuid().ToString("N");
                var isNonStop = option.flights.Count == 1;

                var dto = new FlightOfferDto(
                    Id: id,
                    Airline: firstFlight.airline,
                    AirlineCode: firstFlight.airline.Length >= 2 ? firstFlight.airline.Substring(0, 2).ToUpper() : "?",
                    FlightNumber: firstFlight.flight_number,
                    Origin: firstFlight.departure_airport.name,
                    OriginIata: firstFlight.departure_airport.id,
                    Destination: lastFlight.arrival_airport.name,
                    DestinationIata: lastFlight.arrival_airport.id,
                    DepartureTime: firstFlight.departure_airport.time,
                    ArrivalTime: lastFlight.arrival_airport.time,
                    Duration: $"PT{option.total_duration}M",
                    Price: option.price,
                    Currency: "INR",
                    SeatsAvailable: 9,
                    CabinClass: firstFlight.travel_class,
                    IsNonStop: isNonStop,
                    BookingUrl: deepLink
                );

                flightOffers.Add(dto);
            }

            if (flightOffers.Any())
            {
                var cacheOptions = new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1),
                    SlidingExpiration = TimeSpan.FromMinutes(15)
                };

                var serializedData = JsonSerializer.Serialize(flightOffers);
                await _cache.SetStringAsync(cacheKey, serializedData, cacheOptions);
                _logger.LogInformation("Saved {Count} flights into cache for SearchFlights: {CacheKey}", flightOffers.Count, cacheKey);
            }

            return flightOffers;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Flight Search logic failed");
            return new List<FlightOfferDto>();
        }
    }
}
