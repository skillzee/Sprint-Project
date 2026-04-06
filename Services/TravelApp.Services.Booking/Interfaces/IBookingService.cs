using TravelApp.Services.Booking.DTOs;

namespace TravelApp.Services.Booking.Interfaces
{
    public interface IBookingService
    {
        Task<Object> GetAllBookingWithRevenueAsync();
        Task<IEnumerable<BookingDto>> GetUserBookingsAsync(int userId);
        Task<(BookingDto? result, string? errorMessage)> CreateBookingAsync(CreateBookingDto dto, int userId, string userName, string userEmai);
        Task<bool> CancelBookingAsync(int id, int userId, string role);
    }
}
