using Microsoft.Extensions.Caching.Memory;
using System.Text.Json;
using TravelApp.Services.Flight.DTOs;

namespace TravelApp.Services.Flight.Services
{
    public class SerpApiFlightService
    {
        private readonly HttpClient _http;
        private readonly IConfiguration _config;
        private readonly ILogger<SerpApiFlightService> _logger;

        public SerpApiFlightService(HttpClient http, IConfiguration config, ILogger<SerpApiFlightService> logger)
        {
            _http = http;
            _config = config;
            _logger = logger;
        }


        public async Task<List<FlightOfferDto>> SearchFlightsAsync(string origin, string destination, string date, int adults = 1, string cabinClass = "ECONOMY")
        {
            try
            {
                var apiKey = _config["SerpApi:ApiKey"];
                if (string.IsNullOrEmpty(apiKey))
                {
                    _logger.LogError("SerpApi Key is entirely missing from configuration.");
                    return new List<FlightOfferDto>();
                }

                int travelClassInt = cabinClass.ToUpper() switch
                {
                    "PREMIUM_ECONOMY" => 2,
                    "BUSINESS" => 3,
                    "FIRST" => 4,
                    _ => 1
                };

                var url = $"https://serpapi.com/search.json?engine=google_flights" +
                      $"&departure_id={origin.ToUpper()}" +
                      $"&arrival_id={destination.ToUpper()}" +
                      $"&outbound_date={date}" +
                      $"&currency=INR&hl=en&type=2" +
                      $"&adults={adults}" +
                      $"&travel_class={travelClassInt}" +
                      $"&api_key={apiKey}";

                var response = await _http.GetAsync(url);


                if (!response.IsSuccessStatusCode)
                {
                    var err = await response.Content.ReadAsStringAsync();
                    _logger.LogWarning("SerpApi error {Status}: {Body}", response.StatusCode, err);
                    return new List<FlightOfferDto>();
                }

                var json = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<SerpApiFlightResponse>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                var flightOffers = new List<FlightOfferDto>();
                var allOptions = new List<SerpApiFlightOption>();

                if (result?.best_flights != null) allOptions.AddRange(result.best_flights);
                if (result?.other_flights != null) allOptions.AddRange(result.other_flights);


                var fallbackUrl = $"https://www.google.com/travel/flights?q=flights+{origin}+to+{destination}+{date}";
                var deepLink = result?.search_metadata?.google_flights_url ?? fallbackUrl;

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
                        SeatsAvailable: 9, // SerpApi doesn't provide seats available
                        CabinClass: firstFlight.travel_class,
                        IsNonStop: isNonStop,
                        BookingUrl: deepLink
                    );

                    flightOffers.Add(dto);
                }

                return flightOffers;

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SerpApi search failed");
                return new List<FlightOfferDto>();
            }
        }

    }
}
