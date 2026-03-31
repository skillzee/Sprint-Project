using Microsoft.EntityFrameworkCore;
using TravelApp.Services.Trip.Data;
using TravelApp.Services.Trip.Interfaces;
using TravelApp.Services.Trip.Models;

namespace TravelApp.Services.Trip.Repositories
{
    public class TripRepository : ITripRepository
    {
        private readonly TripDbContext _db;

        public TripRepository(TripDbContext db)
        {
            _db = db;
        }

        public async Task AddItinerariesAsync(IEnumerable<Itinerary> items)
        {
            await _db.Itineraries.AddRangeAsync(items);
        }

        public async Task AddTripAsync(Models.Trip trip)
        {
            await _db.Trips.AddAsync(trip);
        }

        public async Task ClearItinerariesAsync(int tripId)
        {
            var itenary = await _db.Itineraries.Where(i => i.Id == tripId).ToListAsync();
            _db.Itineraries.RemoveRange(itenary);
        }

        public async Task DeleteTripAsync(Models.Trip trip)
        {
            _db.Trips.Remove(trip);
        }

        public async Task<Models.Trip?> GetTripByIdAsync(int id)
        {
            return await _db.Trips.Include(t => t.Itineraries).FirstOrDefaultAsync(t =>  t.Id == id);
        }

        public async Task<IEnumerable<Models.Trip>> GetUserTripsAsync(int userId)
        {
            return await _db.Trips.Include(t => t.Itineraries)
                .Where(t => t.UserId == userId)
                .OrderByDescending(t => t.CreatedAt).ToListAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _db.SaveChangesAsync();
        }
    }
}
