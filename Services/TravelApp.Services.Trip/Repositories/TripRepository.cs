using Microsoft.EntityFrameworkCore;
using TravelApp.Services.Trip.Data;
using TravelApp.Services.Trip.Interfaces;
using TravelApp.Services.Trip.Models;

namespace TravelApp.Services.Trip.Repositories
{
    /// <summary>
    /// Implements data access for <see cref="Models.Trip"/> and <see cref="Itinerary"/> entities
    /// using Entity Framework Core against the Trip SQL database.
    /// </summary>
    public class TripRepository : ITripRepository
    {
        private readonly TripDbContext _db;

        /// <summary>
        /// Initializes a new instance of <see cref="TripRepository"/>.
        /// </summary>
        /// <param name="db">The EF Core database context for the Trip service.</param>
        public TripRepository(TripDbContext db)
        {
            _db = db;
        }

        /// <summary>
        /// Stages itinerary items for bulk insertion into the database.
        /// </summary>
        /// <param name="items">The itinerary entries to add.</param>
        public async Task AddItinerariesAsync(IEnumerable<Itinerary> items)
        {
            await _db.Itineraries.AddRangeAsync(items);
        }

        /// <summary>
        /// Stages a new trip for insertion into the database.
        /// </summary>
        /// <param name="trip">The trip entity to add.</param>
        public async Task AddTripAsync(Models.Trip trip)
        {
            await _db.Trips.AddAsync(trip);
        }

        /// <summary>
        /// Removes all itinerary items for a given trip ID; used before regenerating an AI itinerary.
        /// </summary>
        /// <param name="tripId">The trip whose itinerary items should be cleared.</param>
        public async Task ClearItinerariesAsync(int tripId)
        {
            var itenary = await _db.Itineraries.Where(i => i.TripId == tripId).ToListAsync();
            _db.Itineraries.RemoveRange(itenary);
        }

        /// <summary>
        /// Removes a trip entity from the database (including cascade-deleted itineraries).
        /// </summary>
        /// <param name="trip">The trip to remove.</param>
        public async Task DeleteTripAsync(Models.Trip trip)
        {
            _db.Trips.Remove(trip);
        }

        /// <summary>
        /// Retrieves a trip by its primary key with all itinerary items eagerly loaded.
        /// </summary>
        /// <param name="id">The trip's unique identifier.</param>
        /// <returns>The <see cref="Models.Trip"/> with itineraries, or <c>null</c> if not found.</returns>
        public async Task<Models.Trip?> GetTripByIdAsync(int id)
        {
            return await _db.Trips.Include(t => t.Itineraries).FirstOrDefaultAsync(t => t.Id == id);
        }

        /// <summary>
        /// Retrieves all trips for a specific user with itineraries loaded, ordered newest first.
        /// </summary>
        /// <param name="userId">The user's ID to filter by.</param>
        /// <returns>A list of that user's trips with itineraries.</returns>
        public async Task<IEnumerable<Models.Trip>> GetUserTripsAsync(int userId)
        {
            return await _db.Trips.Include(t => t.Itineraries)
                .Where(t => t.UserId == userId)
                .OrderByDescending(t => t.CreatedAt).ToListAsync();
        }

        /// <summary>
        /// Commits all pending EF Core change-tracked operations to the database.
        /// </summary>
        public async Task SaveChangesAsync()
        {
            await _db.SaveChangesAsync();
        }
    }
}
