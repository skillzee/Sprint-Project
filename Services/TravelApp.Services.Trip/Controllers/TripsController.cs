using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TravelApp.Services.Trip.AI;
using TravelApp.Services.Trip.Data;

namespace TravelApp.Services.Trip.Controllers
{
    [Route("api/trips")]
    [ApiController]
    [Authorize]
    public class TripsController(TripDbContext db, IGeminiService gemini) : ControllerBase
    {
    }
}
