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

        // Retrieves all trips for the currently authenticated user
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TripDto>>> GetMine()
        {
            try
            {
                var userId = GetUserId();
                var result = await _service.GetUserTripsAsync(userId);
                if (result == null)
                {
                    return BadRequest();
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred fetching user trips.", error = ex.Message });
            }
        }

        // Retrieves a specific trip by its ID
        [HttpGet("{id}")]
        public async Task<ActionResult<TripDto>> GetById(int id)
        {
            try
            {
                var userId = GetUserId();
                var result = await _service.GetTripByIdAsync(id, userId);
                if (result == null)
                {
                    return NotFound();
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred fetching the trip.", error = ex.Message });
            }
        }


        // Creates a new trip for the user
        [HttpPost]
        public async Task<ActionResult<TripDto>> Create(CreateTripDto dto)
        {
            try
            {
                var userId = GetUserId();
                var result = await _service.CreateTripAsync(dto, userId);

                if (result == null)
                {
                    return BadRequest();
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred creating the trip.", error = ex.Message });
            }
        }

        // Generates an AI-driven itinerary for a trip
        [HttpPost("generate-itinerary")]
        public async Task<ActionResult<TripDto>> GenerateItinerary(GenerateItineraryDto dto)
        {
            try
            {
                int userId = GetUserId();
                var result = await _service.GenerateItineraryAsync(dto, userId);
                if (result == null)
                    return BadRequest(new { message = "Trip not found." });
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred generating the itinerary.", error = ex.Message });
            }
        }


        // Deletes a trip
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                int userId = GetUserId();
                var result = await _service.DeleteTripAsync(id, userId);

                if (result == false)
                {
                    return NotFound(new { message = "Trip not found or you don't have permission to delete it." });
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred deleting the trip.", error = ex.Message });
            }
        }


        private int GetUserId() => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        
    }

}

