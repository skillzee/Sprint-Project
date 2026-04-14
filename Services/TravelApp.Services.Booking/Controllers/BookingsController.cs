using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

using TravelApp.Services.Booking.DTOs;
using TravelApp.Services.Booking.Interfaces;
using TravelApp.Shared.Exceptions;
using Microsoft.Extensions.Logging;




namespace TravelApp.Services.Booking.Controllers
{
    [Route("api/bookings")]
    [ApiController]
    public class BookingsController : ControllerBase
    {
        private readonly IBookingService _service;
        private readonly ILogger<BookingsController> _logger;
        public BookingsController(IBookingService service, ILogger<BookingsController> logger) {
            _service = service;
            _logger = logger;
        }


        

        /// <summary>
        /// Retrieves all bookings with total revenue (Admin only)
        /// </summary>
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> GetAll()
        {
            try
            {
                var result = await _service.GetAllBookingWithRevenueAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all bookings");
                throw;
            }
        }



        /// <summary>
        /// Retrieves bookings for the currently authenticated user
        /// </summary>
        [HttpGet("my")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<BookingDto>>> GetMine()
        {
            try
            {
                var userId = GetUserId();
                var result = await _service.GetUserBookingsAsync(userId);
                return Ok(result);
            }
            catch (UnauthorizedException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving bookings for user");
                throw;
            }
        }

        /// <summary>
        /// Creates a new booking
        /// </summary>
        [HttpPost]
        [Authorize]
        public async Task<ActionResult> Create(CreateBookingDto dto)
        {
            try
            {
                var userId = GetUserId();
                var userName = User.FindFirstValue(ClaimTypes.Name) ?? "";
                var userEmail = User.FindFirstValue(ClaimTypes.Email) ?? "";

                var result = await _service.CreateBookingAsync(dto, userId, userName, userEmail);

                if (result.result == null)
                {
                    return BadRequest(result.errorMessage ?? "Invalid booking data");
                }

                return Ok(result.result);
            }
            catch (UnauthorizedException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating booking for user");
                throw;
            }
        }


        /// <summary>
        /// Cancels an existing booking
        /// </summary>
        [HttpPut("cancel/{id}")]
        [Authorize]
        public async Task<ActionResult> Cancel(int id)
        {
            var userId = GetUserId();
            var role = User.FindFirstValue(ClaimTypes.Role) ?? "";

            var success = await _service.CancelBookingAsync(id, userId, role);

            if (!success)
                return BadRequest("Unable to cancel booking");

            return Ok(new { message = "Booking cancelled" });
        }


        private int GetUserId()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userIdClaim))
                throw new UnauthorizedException("User ID not found in token");

            if (!int.TryParse(userIdClaim, out int userId))
                throw new UnauthorizedException("Invalid User ID format in token");

            return userId;
        }


        
    }
}
