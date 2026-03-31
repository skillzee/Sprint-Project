using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TravelApp.Services.Trip.AI;
using TravelApp.Services.Trip.Data;
using TravelApp.Services.Trip.DTOs;
using TravelApp.Services.Trip.Interfaces;

namespace TravelApp.Services.Trip.Controllers
{
    [Route("api/trips")]
    [ApiController]
    [Authorize]
    public class TripsController(ITripService _service) : ControllerBase
    {

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TripDto>>> GetMine()
        {
            var userId = GetUserId();
            var result =await _service.GetUserTripsAsync(userId);
            if(result == null)
            {
                return BadRequest();
            }
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TripDto>> GetById(int id)
        {
            var userId = GetUserId();
            var result =await _service.GetTripByIdAsync(id, userId);
            if( result == null)
            {
                return NotFound();
            }
            return Ok(result);

        }


        [HttpPost]
        public async Task<ActionResult<TripDto>> Create(CreateTripDto dto)
        {
            var userId = GetUserId();
            var result =await _service.CreateTripAsync(dto, userId);

            if(result == null)
            {
                return BadRequest();
            }

            return Ok(result);
        }

        [HttpPost("generate-itinerary")]
        public async Task<ActionResult<TripDto>> GenerateItinerary(GenerateItineraryDto dto)
        {
            int userId = GetUserId();
            var result = await _service.GenerateItineraryAsync(dto, userId);

            if(result == null)
            {
                return BadRequest();
            }

            return Ok(result);


        }


        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            int userId = GetUserId();
            var ressult = await _service.DeleteTripAsync(id, userId);

            if(ressult == false)
            {
                return NotFound(new { message = "Trip not found or you don't have permission to delete it." });
            }


            return NoContent();
        }


        private int GetUserId() => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        
    }

}

