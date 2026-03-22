using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TravelApp.Services.Flight.DTOs;
using TravelApp.Services.Flight.Services;

namespace TravelApp.Services.Flight.Controllers
{
    [Route("api/flights")]
    [ApiController]
    public class FlightsController(SerpApiFlightService flightService) : ControllerBase
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
            new { code = "DEL", city = "New Delhi",    country = "India" },
            new { code = "BOM", city = "Mumbai",       country = "India" },
            new { code = "BLR", city = "Bangalore",    country = "India" },
            new { code = "MAA", city = "Chennai",      country = "India" },
            new { code = "CCU", city = "Kolkata",      country = "India" },
            new { code = "HYD", city = "Hyderabad",    country = "India" },
            new { code = "GOI", city = "Goa",          country = "India" },
            new { code = "JAI", city = "Jaipur",       country = "India" },
            new { code = "AMD", city = "Ahmedabad",    country = "India" },
            new { code = "COK", city = "Kochi",        country = "India" },
            new { code = "PNQ", city = "Pune",         country = "India" },
            new { code = "LKO", city = "Lucknow",      country = "India" },
            new { code = "ATQ", city = "Amritsar",     country = "India" },
            new { code = "SXR", city = "Srinagar",     country = "India" },
            new { code = "LHR", city = "London",       country = "UK" },
            new { code = "DXB", city = "Dubai",        country = "UAE" },
            new { code = "SIN", city = "Singapore",    country = "Singapore" },
            new { code = "BKK", city = "Bangkok",      country = "Thailand" },
            new { code = "NRT", city = "Tokyo",        country = "Japan" },
            new { code = "CDG", city = "Paris",        country = "France" },
            new { code = "JFK", city = "New York",     country = "USA" },
            new { code = "SYD", city = "Sydney",       country = "Australia" },
        });
        }

    }
}
