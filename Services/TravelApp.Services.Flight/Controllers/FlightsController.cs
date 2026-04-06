using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TravelApp.Services.Flight.DTOs;
using TravelApp.Services.Flight.Interfaces;
using TravelApp.Services.Flight.Services;

namespace TravelApp.Services.Flight.Controllers
{
    [Route("api/flights")]
    [ApiController]
    public class FlightsController(IFlightService flightService) : ControllerBase
    {

        [HttpGet]
        public async Task<ActionResult<IEnumerable<FlightOfferDto>>> Search([FromQuery] string origin = "DEL",
        [FromQuery] string destination = "BOM",
        [FromQuery] string? date = null,
        [FromQuery] int adults = 1,
        [FromQuery] string cabin = "ECONOMY")
        {


            var searchDate = date ?? DateTime.Today.AddDays(7).ToString("yyyy-MM-dd");
            var results = await flightService.SearchFlightsAsync(origin, destination, searchDate, adults, cabin);
            return Ok(results);
        
        }



        [HttpGet("airports")]
        public ActionResult<object> GetAirports()
        {
            return Ok(new[]
            {
                // --- INDIA (Domestic & International) ---
                new { code = "DEL", city = "New Delhi", country = "India" },
                new { code = "BOM", city = "Mumbai", country = "India" },
                new { code = "BLR", city = "Bangalore", country = "India" },
                new { code = "MAA", city = "Chennai", country = "India" },
                new { code = "CCU", city = "Kolkata", country = "India" },
                new { code = "HYD", city = "Hyderabad", country = "India" },
                new { code = "GOI", city = "Goa (Dabolim)", country = "India" },
                new { code = "GOX", city = "Goa (Mopa)", country = "India" },
                new { code = "JAI", city = "Jaipur", country = "India" },
                new { code = "AMD", city = "Ahmedabad", country = "India" },
                new { code = "COK", city = "Kochi", country = "India" },
                new { code = "PNQ", city = "Pune", country = "India" },
                new { code = "LKO", city = "Lucknow", country = "India" },
                new { code = "ATQ", city = "Amritsar", country = "India" },
                new { code = "SXR", city = "Srinagar", country = "India" },
                new { code = "TRV", city = "Thiruvananthapuram", country = "India" },
                new { code = "IXC", city = "Chandigarh", country = "India" },
                new { code = "BBI", city = "Bhubaneswar", country = "India" },
                new { code = "GAU", city = "Guwahati", country = "India" },
                new { code = "PAT", city = "Patna", country = "India" },
                new { code = "IDR", city = "Indore", country = "India" },
                new { code = "NAG", city = "Nagpur", country = "India" },
                new { code = "VNS", city = "Varanasi", country = "India" },
                new { code = "VTZ", city = "Visakhapatnam", country = "India" },
                new { code = "IXB", city = "Bagdogra", country = "India" },
                new { code = "COB", city = "Coimbatore", country = "India" },
                new { code = "STV", city = "Surat", country = "India" },
                new { code = "RPR", city = "Raipur", country = "India" },
                new { code = "IXR", city = "Ranchi", country = "India" },
                new { code = "IXJ", city = "Jammu", country = "India" },
                new { code = "BDQ", city = "Vadodara", country = "India" },
                new { code = "IMF", city = "Imphal", country = "India" },
                new { code = "IXZ", city = "Port Blair", country = "India" },
                new { code = "UDR", city = "Udaipur", country = "India" },
                new { code = "JDH", city = "Jodhpur", country = "India" },
                new { code = "AYJ", city = "Ayodhya", country = "India" },

                // --- ASIA & MIDDLE EAST ---
                new { code = "DXB", city = "Dubai", country = "UAE" },
                new { code = "AUH", city = "Abu Dhabi", country = "UAE" },
                new { code = "SIN", city = "Singapore", country = "Singapore" },
                new { code = "BKK", city = "Bangkok", country = "Thailand" },
                new { code = "HKT", city = "Phuket", country = "Thailand" },
                new { code = "HKG", city = "Hong Kong", country = "China" },
                new { code = "NRT", city = "Tokyo (Narita)", country = "Japan" },
                new { code = "HND", city = "Tokyo (Haneda)", country = "Japan" },
                new { code = "ICN", city = "Seoul", country = "South Korea" },
                new { code = "PVG", city = "Shanghai", country = "China" },
                new { code = "PEK", city = "Beijing", country = "China" },
                new { code = "KUL", city = "Kuala Lumpur", country = "Malaysia" },
                new { code = "CGK", city = "Jakarta", country = "Indonesia" },
                new { code = "DPS", city = "Bali", country = "Indonesia" },
                new { code = "SGN", city = "Ho Chi Minh City", country = "Vietnam" },
                new { code = "DOH", city = "Doha", country = "Qatar" },
                new { code = "MCT", city = "Muscat", country = "Oman" },
                new { code = "RUH", city = "Riyadh", country = "Saudi Arabia" },
                new { code = "JED", city = "Jeddah", country = "Saudi Arabia" },
                new { code = "CMB", city = "Colombo", country = "Sri Lanka" },
                new { code = "MLE", city = "Male", country = "Maldives" },
                new { code = "KTM", city = "Kathmandu", country = "Nepal" },

                // --- EUROPE ---
                new { code = "LHR", city = "London (Heathrow)", country = "UK" },
                new { code = "LGW", city = "London (Gatwick)", country = "UK" },
                new { code = "CDG", city = "Paris", country = "France" },
                new { code = "FRA", city = "Frankfurt", country = "Germany" },
                new { code = "MUC", city = "Munich", country = "Germany" },
                new { code = "AMS", city = "Amsterdam", country = "Netherlands" },
                new { code = "IST", city = "Istanbul", country = "Turkey" },
                new { code = "MAD", city = "Madrid", country = "Spain" },
                new { code = "BCN", city = "Barcelona", country = "Spain" },
                new { code = "FCO", city = "Rome", country = "Italy" },
                new { code = "MXP", city = "Milan", country = "Italy" },
                new { code = "ZRH", city = "Zurich", country = "Switzerland" },
                new { code = "VIE", city = "Vienna", country = "Austria" },
                new { code = "BRU", city = "Brussels", country = "Belgium" },
                new { code = "CPH", city = "Copenhagen", country = "Denmark" },
                new { code = "SVO", city = "Moscow", country = "Russia" },
                new { code = "DUB", city = "Dublin", country = "Ireland" },
                new { code = "LIS", city = "Lisbon", country = "Portugal" },

                // --- NORTH & SOUTH AMERICA ---
                new { code = "JFK", city = "New York", country = "USA" },
                new { code = "LAX", city = "Los Angeles", country = "USA" },
                new { code = "SFO", city = "San Francisco", country = "USA" },
                new { code = "ORD", city = "Chicago", country = "USA" },
                new { code = "DFW", city = "Dallas", country = "USA" },
                new { code = "ATL", city = "Atlanta", country = "USA" },
                new { code = "MIA", city = "Miami", country = "USA" },
                new { code = "EWR", city = "Newark", country = "USA" },
                new { code = "SEA", city = "Seattle", country = "USA" },
                new { code = "YYZ", city = "Toronto", country = "Canada" },
                new { code = "YVR", city = "Vancouver", country = "Canada" },
                new { code = "MEX", city = "Mexico City", country = "Mexico" },
                new { code = "GRU", city = "Sao Paulo", country = "Brazil" },
                new { code = "EZE", city = "Buenos Aires", country = "Argentina" },

                // --- OCEANIA & AFRICA ---
                new { code = "SYD", city = "Sydney", country = "Australia" },
                new { code = "MEL", city = "Melbourne", country = "Australia" },
                new { code = "BNE", city = "Brisbane", country = "Australia" },
                new { code = "AKL", city = "Auckland", country = "New Zealand" },
                new { code = "JNB", city = "Johannesburg", country = "South Africa" },
                new { code = "CPT", city = "Cape Town", country = "South Africa" },
                new { code = "CAI", city = "Cairo", country = "Egypt" },
                new { code = "NBO", city = "Nairobi", country = "Kenya" },
                new { code = "CAS", city = "Casablanca", country = "Morocco" },
                new { code = "ADD", city = "Addis Ababa", country = "Ethiopia" },
            });
        }

    }
}
