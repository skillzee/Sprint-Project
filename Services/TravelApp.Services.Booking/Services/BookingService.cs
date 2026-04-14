using MassTransit;
using TravelApp.Services.Booking.Clients;
using TravelApp.Services.Booking.DTOs;
using TravelApp.Services.Booking.Interfaces;
using TravelApp.Shared;
using Microsoft.Extensions.Logging;

namespace TravelApp.Services.Booking.Services
{
    /// <summary>
    /// Implements booking business logic including date validation, overlap prevention,
    /// room availability checks, and RabbitMQ event publishing.
    /// </summary>
    public class BookingService : IBookingService
    {
        private readonly IBookingRepository _repo;
        private readonly IPublishEndpoint _bus;
        private readonly IHotelClient _hotelClient;
        private readonly ILogger<BookingService> _logger;

        /// <summary>
        /// Initializes a new instance of <see cref="BookingService"/>.
        /// </summary>
        /// <param name="repo">The booking data access repository.</param>
        /// <param name="bus">The MassTransit publish endpoint for raising domain events.</param>
        /// <param name="hotelClient">The HTTP client for querying the Hotel service.</param>
        public BookingService(IBookingRepository repo, IPublishEndpoint bus, IHotelClient hotelClient, ILogger<BookingService> logger)
        {
            _repo = repo;
            _bus = bus;
            _hotelClient = hotelClient;
            _logger = logger;
        }

        /// <summary>
        /// Cancels an existing booking. Admins can cancel any booking; users can only cancel their own.
        /// Publishes a <c>BookingCancelledEvent</c> on success.
        /// </summary>
        /// <param name="id">The ID of the booking to cancel.</param>
        /// <param name="userId">The ID of the requesting user.</param>
        /// <param name="role">The role of the requesting user (<c>"Admin"</c> or <c>"Customer"</c>).</param>
        /// <returns><c>true</c> if cancelled successfully; otherwise, <c>false</c>.</returns>
        public async Task<bool> CancelBookingAsync(int id, int userId, string role)
        {
            try
            {
                var booking = await _repo.GetByIdAsync(id);
                if (booking == null) return false;
                // Permission check
                if (role != "Admin" && booking.UserId != userId) return false;
                booking.Status = "Cancelled";
                await _repo.SaveChangesAsync(); 
                await _bus.Publish(new BookingCancelledEvent
                {
                    BookingId = booking.Id,
                    UserEmail = booking.UserEmail,
                    UserName = booking.UserName,
                    HotelName = booking.HotelName,
                    BookingRef = booking.BookingRef
                });
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error cancelling booking {BookingId}", id);
                throw;
            }
        }

        /// <summary>
        /// Creates a new confirmed booking after validating dates, checking for overlaps, and verifying room approval.
        /// Publishes a <c>BookingConfirmedEvent</c> on success.
        /// </summary>
        /// <param name="dto">The booking request details including room, hotel, and dates.</param>
        /// <param name="userId">The ID of the user placing the booking.</param>
        /// <param name="userName">The display name of the user.</param>
        /// <param name="userEmai">The email address of the user for confirmation notifications.</param>
        /// <returns>
        /// A tuple of the created <see cref="BookingDto"/> and a <c>null</c> error on success,
        /// or a <c>null</c> result and a descriptive error message on failure.
        /// </returns>
        public async Task<(BookingDto? result, string? errorMessage)> CreateBookingAsync(CreateBookingDto dto, int userId, string userName, string userEmai)
        {
            try
            {
                // 0. Room approval check (fail-safe: treat unreachable Hotel service as unavailable)
                var room = await _hotelClient.GetRoomAsync(dto.RoomId);
                if (room == null || room.ApprovalStatus != "Approved")
                    return (null, "Room is not available for booking.");

                // Normalize dates to Date part only to avoid time-of-day overlap issues
                var checkIn = dto.CheckInDate.Date;
                var checkOut = dto.CheckOutDate.Date;

                // 1. Business Validation
                if (checkIn < DateTime.Today || checkOut <= checkIn)
                {
                    return (null, "Invalid booking dates");
                }
                // 2. Business Logic: Duplicate booking prevention
                var hasOverlap = await _repo.HasOverlappingBookingAsync(userId, dto.RoomId, checkIn, checkOut);
                if (hasOverlap)
                {
                    return (null, "Booking with same date already exists by the user");
                }

                // 3. Business Logic: Calculations & Ref
                var nights = (dto.CheckOutDate - dto.CheckInDate).Days;
                var total = nights * dto.PricePerNight;
                var bookingRef = $"STY-{DateTime.UtcNow.Year}-{Random.Shared.Next(100000, 999999)}";
                var booking = new Models.Booking
                {
                    UserId = userId,
                    UserName = userName,
                    UserEmail = userEmai,
                    RoomId = dto.RoomId,
                    RoomType = dto.RoomType,
                    HotelName = dto.HotelName,
                    CheckInDate = dto.CheckInDate,
                    CheckOutDate = dto.CheckOutDate,
                    TotalPrice = total,
                    Status = "Confirmed",
                    BookingRef = bookingRef,
                    CreatedAt = DateTime.Now
                };
                // 3. Data Access
                await _repo.AddAsync(booking);
                await _repo.SaveChangesAsync();
                // 4. Messaging Logic: Notify other services
                await _bus.Publish(new BookingConfirmedEvent
                {
                    BookingId = booking.Id,
                    UserId = userId,
                    UserName = userName,
                    UserEmail = userEmai,
                    HotelName = dto.HotelName,
                    RoomType = dto.RoomType,
                    CheckInDate = dto.CheckInDate,
                    CheckOutDate = dto.CheckOutDate,
                    TotalPrice = total,
                    BookingRef = bookingRef
                });
                return (MapSingleToDto(booking), null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating booking for user {UserId}", userId);
                throw;
            }
        }

        /// <summary>
        /// Retrieves all bookings across all users along with the total revenue. Intended for Admin use only.
        /// </summary>
        /// <returns>An anonymous object containing a list of all bookings and the total revenue sum.</returns>
        public async Task<object> GetAllBookingWithRevenueAsync()
        {
            var bookings = await _repo.GetAllAsync();
            var result = MapToDto(bookings);
            var totalRevenue = bookings.Sum(b => b.TotalPrice);
            return new { bookings = result, totalRevenue };
        }

        /// <summary>
        /// Retrieves all bookings made by a specific user, ordered newest first.
        /// </summary>
        /// <param name="userId">The unique identifier of the user.</param>
        /// <returns>A collection of <see cref="BookingDto"/> for that user.</returns>
        public async Task<IEnumerable<BookingDto>> GetUserBookingsAsync(int userId)
        {
            var bookings =await _repo.GetByUserIdAsync(userId);
            return MapToDto(bookings);
        }


        /// <summary>Maps a collection of booking entities to DTOs.</summary>
        private IEnumerable<BookingDto> MapToDto(IEnumerable<Models.Booking> bookings)
        {
            return bookings.Select(MapSingleToDto);
        }
        /// <summary>Maps a single booking entity to a <see cref="BookingDto"/>.</summary>
        private BookingDto MapSingleToDto(Models.Booking b)
        {
            return new BookingDto(b.Id, b.UserId, b.UserName, b.RoomId, b.RoomType, b.HotelName,
                                 b.CheckInDate, b.CheckOutDate, b.TotalPrice, b.Status, b.BookingRef, b.CreatedAt);
        }
    }
}
