using System.Text.Json;
using TravelApp.Services.Flight.DTOs;
using TravelApp.Services.Flight.Interfaces;

namespace TravelApp.Services.Flight.Repositories
{
    /// <summary>
    /// Fetches raw flight data from the SerpApi Google Flights endpoint using an <see cref="HttpClient"/>.
    /// </summary>
    public class FlightRepository : IFlightRepository
    {

        private readonly HttpClient _http;
        private readonly IConfiguration _config;

        /// <summary>
        /// Initializes a new instance of <see cref="FlightRepository"/>.
        /// </summary>
        /// <param name="http">The HTTP client used to call the SerpApi endpoint.</param>
        /// <param name="config">The application configuration for reading the SerpApi key.</param>
        public FlightRepository(HttpClient http, IConfiguration config)
        {
            _http = http;
            _config = config;
        }

        /// <summary>
        /// Calls the SerpApi Google Flights endpoint and deserializes the raw response.
        /// </summary>
        /// <param name="origin">The IATA departure airport code.</param>
        /// <param name="destination">The IATA arrival airport code.</param>
        /// <param name="date">The travel date in <c>YYYY-MM-DD</c> format.</param>
        /// <param name="adults">The number of passengers.</param>
        /// <param name="travelClass">The numeric SerpApi travel class (1=Economy, 2=Premium Economy, 3=Business, 4=First).</param>
        /// <returns>A <see cref="SerpApiFlightResponse"/> with raw flight data, or <c>null</c> if the API call fails.</returns>
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
