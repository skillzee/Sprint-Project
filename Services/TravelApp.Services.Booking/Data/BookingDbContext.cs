using Microsoft.EntityFrameworkCore;

namespace TravelApp.Services.Booking.Data
{
    public class BookingDbContext(DbContextOptions<BookingDbContext> options) : DbContext(options)
    {
        public DbSet<Models.Booking> Bookings { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Models.Booking>()
                .Property(b => b.TotalPrice)
                .HasColumnType("decimal(18,2)");
        }
    }
}
