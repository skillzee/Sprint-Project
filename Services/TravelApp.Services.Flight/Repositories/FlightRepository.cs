using System.Text.Json;
using TravelApp.Services.Flight.DTOs;
using TravelApp.Services.Flight.Interfaces;

namespace TravelApp.Services.Flight.Repositories
{
    public class FlightRepository : IFlightRepository
    {

        private readonly HttpClient _http;
        private readonly IConfiguration _config;

        public FlightRepository(HttpClient http, IConfiguration config)
        {
            _http = http;
            _config = config;
        }

        public async Task<SerpApiFlightResponse?> GetFlightsRawAsync(string origin, string destination, string date, int adults, int travelClass)
        {
            var apiKey = _config["SerpApi:ApiKey"];
            var url = $"https://serpapi.com/search.json?engine=google_flights" +
                  $"&departure_id={origin.ToUpper()}" +
                  $"&arrival_id={destination.ToUpper()}" +
                  $"&outbound_date={date}" +
                  $"&currency=INR&hl=en&type=2" +
                  $"&adults={adults}" +
                  $"&travel_class={travelClass}" +
                  $"&api_key={apiKey}";

            var response = await _http.GetAsync(url);
            if (!response.IsSuccessStatusCode)
            {
                return null;
            }
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<SerpApiFlightResponse>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });


        }
    }
}
