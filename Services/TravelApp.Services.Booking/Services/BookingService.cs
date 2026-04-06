using MassTransit;
using TravelApp.Services.Booking.DTOs;
using TravelApp.Services.Booking.Interfaces;
using TravelApp.Shared;

namespace TravelApp.Services.Booking.Services
{
    public class BookingService : IBookingService
    {
        private readonly IBookingRepository _repo;
        private readonly IPublishEndpoint _bus;

        public BookingService(IBookingRepository repo, IPublishEndpoint bus)
        {
            _repo = repo;
            _bus = bus;
        }



        public async Task<bool> CancelBookingAsync(int id, int userId, string role)
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

        public async Task<BookingDto?> CreateBookingAsync(CreateBookingDto dto, int userId, string userName, string userEmai)
        {
            // 1. Business Validation
            if (dto.CheckInDate < DateTime.Today || dto.CheckOutDate <= dto.CheckInDate)
            {
                return null;
            }
            // 2. Business Logic: Duplicate booking prevention
            var hasOverlap = await _repo.HasOverlappingBookingAsync(userId, dto.RoomId, dto.CheckInDate, dto.CheckOutDate);
            if (hasOverlap)
            {
                return null;
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
            return MapSingleToDto(booking);
        }

        public async Task<object> GetAllBookingWithRevenueAsync()
        {
            var bookings = await _repo.GetAllAsync();
            var result = MapToDto(bookings);
            var totalRevenue = bookings.Sum(b => b.TotalPrice);
            return new { bookings = result, totalRevenue };
        }

        public async Task<IEnumerable<BookingDto>> GetUserBookingsAsync(int userId)
        {
            var bookings =await _repo.GetByUserIdAsync(userId);
            return MapToDto(bookings);
        }


        private IEnumerable<BookingDto> MapToDto(IEnumerable<Models.Booking> bookings)
        {
            return bookings.Select(MapSingleToDto);
        }
        private BookingDto MapSingleToDto(Models.Booking b)
        {
            return new BookingDto(b.Id, b.UserId, b.UserName, b.RoomId, b.RoomType, b.HotelName,
                                 b.CheckInDate, b.CheckOutDate, b.TotalPrice, b.Status, b.BookingRef, b.CreatedAt);
        }
    }
}
