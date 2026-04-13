namespace TravelApp.Services.Flight.DTOs
{
    /// <summary>
    /// Root response object returned by the SerpApi Google Flights endpoint.
    /// </summary>
    public class SerpApiFlightResponse
    {
        /// <summary>Gets or sets the list of best-value flight options returned by the API.</summary>
        public List<SerpApiFlightOption>? best_flights { get; set; }

        /// <summary>Gets or sets the list of other available flight options returned by the API.</summary>
        public List<SerpApiFlightOption>? other_flights { get; set; }

        /// <summary>Gets or sets metadata about the search, including the Google Flights deeplink URL.</summary>
        public SerpApiSearchMetadata? search_metadata { get; set; }
    }

    /// <summary>
    /// Contains metadata returned alongside the SerpApi flight search results.
    /// </summary>
    public class SerpApiSearchMetadata
    {
        /// <summary>Gets or sets the Google Flights URL for this specific search query, used as a booking deeplink.</summary>
        public string google_flights_url { get; set; } = "";
    }

    /// <summary>
    /// Represents a single flight itinerary option (which may include one or more flight legs).
    /// </summary>
    public class SerpApiFlightOption
    {
        /// <summary>Gets or sets the individual flight legs that make up this itinerary.</summary>
        public List<SerpApiFlight> flights { get; set; } = new();

        /// <summary>Gets or sets the total price for this itinerary in INR.</summary>
        public decimal price { get; set; }

        /// <summary>Gets or sets the flight type description (e.g., <c>"Nonstop"</c>).</summary>
        public string type { get; set; } = "";

        /// <summary>Gets or sets the total journey duration in minutes.</summary>
        public int total_duration { get; set; }
    }

    /// <summary>
    /// Represents a single flight leg within a <see cref="SerpApiFlightOption"/>.
    /// </summary>
    public class SerpApiFlight
    {
        /// <summary>Gets or sets the departure airport details.</summary>
        public SerpApiAirport departure_airport { get; set; } = new();

        /// <summary>Gets or sets the arrival airport details.</summary>
        public SerpApiAirport arrival_airport { get; set; } = new();

        /// <summary>Gets or sets the name of the operating airline.</summary>
        public string airline { get; set; } = "";

        /// <summary>Gets or sets the flight number (e.g., <c>"AI-202"</c>).</summary>
        public string flight_number { get; set; } = "";

        /// <summary>Gets or sets the travel class for this leg (e.g., <c>"Economy"</c>).</summary>
        public string travel_class { get; set; } = "Economy";

        /// <summary>Gets or sets the duration of this leg in minutes.</summary>
        public int duration { get; set; }
    }

    /// <summary>
    /// Represents an airport with its name, IATA code, and scheduled time.
    /// </summary>
    public class SerpApiAirport
    {
        /// <summary>Gets or sets the full name of the airport.</summary>
        public string name { get; set; } = "";

        /// <summary>Gets or sets the IATA airport code (e.g., <c>"DEL"</c>).</summary>
        public string id { get; set; } = "";

        /// <summary>Gets or sets the scheduled departure or arrival time as a string.</summary>
        public string time { get; set; } = "";
    }
}
