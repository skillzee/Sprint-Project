using Microsoft.EntityFrameworkCore;
using TravelApp.Services.Booking.Data;
using TravelApp.Services.Booking.Interfaces;

namespace TravelApp.Services.Booking.Repositories
{
    public class BookingRepository : IBookingRepository
    {
        private readonly BookingDbContext _db;

        public BookingRepository(BookingDbContext db)
        {
            _db = db;
        }

        public async Task AddAsync(Models.Booking booking)
        {
            await _db.Bookings.AddAsync(booking);
            await _db.SaveChangesAsync();
        }

        public async Task<IEnumerable<Models.Booking>> GetAllAsync()
        {
            return await _db.Bookings.OrderByDescending(b => b.CreatedAt).ToListAsync();
        }

        public async Task<Models.Booking?> GetByIdAsync(int id)
        {
            return await _db.Bookings.FindAsync(id);   
        }

        public async Task<IEnumerable<Models.Booking>> GetByUserIdAsync(int userId)
        {
            return await _db.Bookings.Where(t => t.UserId == userId).OrderByDescending(item => item.CreatedAt).ToListAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _db.SaveChangesAsync();
        }
    }
}
