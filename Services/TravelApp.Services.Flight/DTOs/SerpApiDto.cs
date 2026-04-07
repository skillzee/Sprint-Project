namespace TravelApp.Services.Flight.DTOs
{
    

    public class SerpApiFlightResponse
    {
        public List<SerpApiFlightOption>? best_flights { get; set; }
        public List<SerpApiFlightOption>? other_flights { get; set; }
        public SerpApiSearchMetadata? search_metadata { get; set; }
    }

    public class SerpApiSearchMetadata
    {
        public string google_flights_url { get; set; } = "";
    }

    public class SerpApiFlightOption
    {
        public List<SerpApiFlight> flights { get; set; } = new();
        public decimal price { get; set; }
        public string type { get; set; } = ""; // e.g., "Nonstop"
        public int total_duration { get; set; } // minutes
    }

    public class SerpApiFlight
    {
        public SerpApiAirport departure_airport { get; set; } = new();
        public SerpApiAirport arrival_airport { get; set; } = new();
        public string airline { get; set; } = "";
        public string flight_number { get; set; } = "";
        public string travel_class { get; set; } = "Economy";
        public int duration { get; set; }
    }

    public class SerpApiAirport
    {
        public string name { get; set; } = "";
        public string id { get; set; } = "";
        public string time { get; set; } = "";
    }

}
