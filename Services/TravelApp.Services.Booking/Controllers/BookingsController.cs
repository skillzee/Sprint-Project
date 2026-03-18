using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TravelApp.Services.Booking.Data;
using TravelApp.Services.Booking.DTOs;
using TravelApp.Shared;



namespace TravelApp.Services.Booking.Controllers
{
    [Route("api/bookings")]
    [ApiController]
    public class BookingsController : ControllerBase
    {
        private readonly BookingDbContext _db;
        private readonly IPublishEndpoint _bus;
        public BookingsController(BookingDbContext db, IPublishEndpoint bus) {
            _db = db;
            _bus = bus;
        }


        

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> GetAll()
        {
            var bookings = await _db.Bookings.OrderByDescending(b => b.CreatedAt).ToListAsync();
            var result = bookings.Select(b => new BookingDto
            (
                 b.Id,
                b.UserId,
                b.UserName,
                b.RoomId,
                b.RoomType,
                b.HotelName,
                b.CheckInDate,
                b.CheckOutDate,
                b.TotalPrice,
                b.Status,
                b.BookingRef,
                b.CreatedAt
            ));


            var totalRevenue = bookings.Sum(b => b.TotalPrice);
            return Ok(new
            {
                bookings = result,
                totalRevenue
            });

        }



        [HttpGet("my")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<BookingDto>>> GetMine()
        {
            var userId = GetUserId();

            var bookings = await _db.Bookings.Where(t => t.UserId == userId).OrderByDescending(item => item.CreatedAt).ToListAsync();


            var result = bookings.Select(b=>new BookingDto(
                 b.Id,
                b.UserId,
                b.UserName,
                b.RoomId,
                b.RoomType,
                b.HotelName,
                b.CheckInDate,
                b.CheckOutDate,
                b.TotalPrice,
                b.Status,
                b.BookingRef,
                b.CreatedAt
                ));


            return Ok(result);
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult> Create(CreateBookingDto dto)
        {
            var userId = GetUserId();
            var userName = User.FindFirstValue(ClaimTypes.Name) ?? "";
            var userEmail = User.FindFirstValue(ClaimTypes.Email) ?? "";

            if (dto.CheckInDate < DateTime.Today)
            {
                return BadRequest("Check-in date cannot be in the past");
            }

            if (dto.CheckOutDate <= dto.CheckInDate)
            {
                return BadRequest("Check-out date must be after check-in date");
            }

            var nights = (dto.CheckOutDate - dto.CheckInDate).Days;
            var total = nights * dto.PricePerNight;
            var bookingRef = $"STY-{DateTime.UtcNow.Year}-{Random.Shared.Next(100000, 999999)}";

            var booking = new Models.Booking
            {
                UserId = userId,
                UserName = userName,
                UserEmail = userEmail,
                RoomId = dto.RoomId,
                RoomType = dto.RoomType,
                HotelName = dto.HotelName,
                CheckInDate = dto.CheckInDate,
                CheckOutDate = dto.CheckOutDate,
                TotalPrice = total,
                Status = "Confirmed",
                BookingRef = bookingRef,
                CreatedAt = DateTime.UtcNow

            };

            _db.Add(booking);
            await _db.SaveChangesAsync();


            await _bus.Publish(new BookingConfirmedEvent
            {
                BookingId = booking.Id,
                UserId = userId,
                UserName = userName,
                UserEmail = userEmail,
                HotelName = dto.HotelName,
                RoomType = dto.RoomType,
                CheckInDate = dto.CheckInDate,
                CheckOutDate = dto.CheckOutDate,
                TotalPrice = total,
                BookingRef = bookingRef
            });


            return Ok(new BookingDto(
                booking.Id,
                booking.UserId,
                booking.UserName,
                booking.RoomId,
                booking.RoomType,
                booking.HotelName,
                booking.CheckInDate,
                booking.CheckOutDate,
                booking.TotalPrice,
                booking.Status,
                booking.BookingRef,
                booking.CreatedAt
            ));







        }


        [HttpPut("cancel/{id}")]
        [Authorize]
        public async Task<ActionResult> Cancel(int id)
        {
            var userId = GetUserId();
            var role = User.FindFirstValue(ClaimTypes.Role);

            var booking = await _db.Bookings.FindAsync(id);
            if (booking == null)
            {
                return NotFound("Booking not found");
            }

            if (role != "Admin" && booking.UserId != userId)
            {
                return Forbid();
            }

            booking.Status = "Cancelled";

            await _db.SaveChangesAsync();

            await _bus.Publish(new BookingCancelledEvent
            {
                BookingId = booking.Id,
                UserEmail = booking.UserEmail,
                UserName = booking.UserName,
                HotelName = booking.HotelName,
                BookingRef = booking.BookingRef
            });

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
