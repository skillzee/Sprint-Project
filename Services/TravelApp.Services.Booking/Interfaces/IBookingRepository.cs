

namespace TravelApp.Services.Booking.Interfaces;

public interface IBookingRepository
{
    Task<IEnumerable<Models.Booking>> GetAllAsync();
    Task<IEnumerable<Models.Booking>> GetByUserIdAsync(int  userId);
    Task<Models.Booking?> GetByIdAsync(int id);
    Task AddAsync(Models.Booking booking);
    Task SaveChangesAsync();


}