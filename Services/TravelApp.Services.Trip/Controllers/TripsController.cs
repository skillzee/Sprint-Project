using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
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
        public async Task<ActionResult<IEnumerable<TripDto>>> GetMine()
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


        [HttpPost]
        public async Task<ActionResult<TripDto>> Create(CreateTripDto dto)
        {
            if(dto.EndDate <= dto.StartDate)
            {
                return BadRequest(new 
                {
                    message = "End date must be after start date."
                }
                );
            }


            var trip = new Models.Trip
            {
                UserId = GetUserId(),
                Destination = dto.Destination,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                CreatedAt = DateTime.Now
            };
            db.Trips.Add(trip);
            await db.SaveChangesAsync();

            return Ok(MapTrip(trip));
        }

        [HttpPost("generate-itinerary")]
        public async Task<ActionResult<TripDto>> GenerateItinerary(GenerateItineraryDto dto)
        {
            var trip = await db.Trips.Include(t => t.Itineraries).FirstOrDefaultAsync(t => t.Id == dto.TripId);

            if(trip == null)
            {
                return NotFound();
            }

            if(trip.UserId != GetUserId())
            {
                return Forbid();
            }

            db.Itineraries.RemoveRange(trip.Itineraries);
            await db.SaveChangesAsync();

            var items = await gemini.GetItineraryAsync(trip.Destination, trip.StartDate, trip.EndDate, dto.Preferences);


            foreach (var item in items)
            {
                item.TripId = trip.Id;
            }
            db.Itineraries.AddRange(items);
            await db.SaveChangesAsync();

            var updated = await db.Trips.Include(t => t.Itineraries).FirstAsync(t => t.Id == trip.Id);
            return Ok(MapTrip(updated));
        }


        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var trip = db.Trips.Include(t => t.Itineraries).FirstOrDefault(t => t.Id == id);

            if(trip == null)
            {
                return NotFound();
            }

            if(trip.UserId != GetUserId())
            {
                return Forbid();
            }

            db.Trips.Remove(trip);
            await db.SaveChangesAsync();
            return NoContent();
        }


        private int GetUserId() => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        private static TripDto MapTrip(Models.Trip t) => new(
        t.Id, t.UserId, t.Destination, t.StartDate, t.EndDate, t.CreatedAt,
        t.Itineraries.OrderBy(i => i.DayNumber)
            .Select(i => new ItineraryDto(i.Id, i.TripId, i.DayNumber, i.Activity, i.Location))
            .ToList());
    }

}

