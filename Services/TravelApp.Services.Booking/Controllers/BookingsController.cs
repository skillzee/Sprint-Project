using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

using TravelApp.Services.Booking.DTOs;
using TravelApp.Services.Booking.Interfaces;




namespace TravelApp.Services.Booking.Controllers
{
    [Route("api/bookings")]
    [ApiController]
    public class BookingsController : ControllerBase
    {
        private readonly IBookingService _service;
        public BookingsController(IBookingService service) {
            _service = service;
        }


        

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> GetAll()
        {
            var result = await _service.GetAllBookingWithRevenueAsync();

            return Ok(result);

        }



        [HttpGet("my")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<BookingDto>>> GetMine()
        {
            var userId = GetUserId();
            var result = await _service.GetUserBookingsAsync(userId);
            return Ok(result);
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult> Create(CreateBookingDto dto)
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


        public int GetUserId()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userIdClaim))
                throw new UnauthorizedAccessException("User ID not found in token");

            return int.Parse(userIdClaim);
        }


        
    }
}
