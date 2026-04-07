using Microsoft.EntityFrameworkCore;

namespace TravelApp.Services.Booking.Data
{
    public class BookingDbContext(DbContextOptions<BookingDbContext> options) : DbContext(options)
    {
        public DbSet<Models.Booking> Bookings { get; set; }
    }
}
