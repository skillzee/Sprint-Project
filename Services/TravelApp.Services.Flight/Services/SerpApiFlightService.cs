using Microsoft.Extensions.Caching.Memory;
using TravelApp.Services.Flight.DTOs;

namespace TravelApp.Services.Flight.Services
{
    public class SerpApiFlightService
    {
        private readonly HttpClient _http;
        private readonly IConfiguration _config;
        private readonly IMemoryCache _cache;
        private readonly ILogger<SerpApiFlightService> _logger;

        public SerpApiFlightService(HttpClient http, IConfiguration config, IMemoryCache cache, ILogger<SerpApiFlightService> logger)
        {
            _http = http;
            _config = config;
            _cache = cache;
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


            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SerpApi search failed");
                return new List<FlightOfferDto>();
            }
        }

    }
}
