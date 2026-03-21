using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TravelApp.Services.Trip.AI;
using TravelApp.Services.Trip.Data;
using TravelApp.Services.Trip.DTOs;

namespace TravelApp.Services.Trip.Controllers
{
    [Route("api/trips")]
    [ApiController]
    [Authorize]
    public class TripsController(TripDbContext db, IGeminiService gemini) : ControllerBase
    {

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TripDto>> GetMine()
        {
            var userId = GetUserId();
            var trips = db.Trips.Include(t => t.Itineraries).Where(t => t.UserId == userId).OrderByDescending(t => t.CreatedAt);

            return Ok(trips.Select(MapTrip));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TripDto>> GetById(int id)
        {
            var trip = await db.Trips.Include(t => t.Itineraries).FirstOrDefaultAsync(t => t.Id == id);
            if(trip == null)
            {
                return NotFound();

            }
            if(trip.UserId != GetUserId())
            {
                return Forbid();
            }

            return Ok(MapTrip(trip));
        }

    }
}
