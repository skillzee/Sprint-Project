using Microsoft.EntityFrameworkCore;
namespace TravelApp.Services.Trip.Data

{
    public class TripDbContext : DbContext
    {
        public TripDbContext(DbContextOptions<TripDbContext> options) : base(options) { }


        public DbSet<Models.Trip> Trips => Set<Models.Trip>();
        public DbSet<Models.Itinerary> Itineraries => Set<Models.Itinerary>();

    }
}
