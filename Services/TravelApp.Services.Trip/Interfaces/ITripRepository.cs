using TravelApp.Services.Trip.Models;

namespace TravelApp.Services.Trip.Interfaces
{
    /// <summary>
    /// Defines the data access contract for trip and itinerary persistence operations.
    /// </summary>
    public interface ITripRepository
    {
        /// <summary>
        /// Retrieves all trips belonging to a user, with itineraries eagerly loaded, ordered newest first.
        /// </summary>
        /// <param name="userId">The user's ID to filter by.</param>
        /// <returns>A collection of <see cref="Models.Trip"/> entities with itineraries.</returns>
        Task<IEnumerable<Models.Trip>> GetUserTripsAsync(int userId);

        /// <summary>
        /// Retrieves a single trip by its primary key, with itineraries eagerly loaded.
        /// </summary>
        /// <param name="id">The trip's unique identifier.</param>
        /// <returns>The <see cref="Models.Trip"/> with itineraries, or <c>null</c> if not found.</returns>
        Task<Models.Trip?> GetTripByIdAsync(int id);

        /// <summary>
        /// Stages a new trip for insertion (call <see cref="SaveChangesAsync"/> to persist).
        /// </summary>
        /// <param name="trip">The trip entity to add.</param>
        Task AddTripAsync(Models.Trip trip);

        /// <summary>
        /// Removes a trip entity from the database (call <see cref="SaveChangesAsync"/> to persist).
        /// </summary>
        /// <param name="trip">The trip entity to delete.</param>
        Task DeleteTripAsync(Models.Trip trip);

        /// <summary>
        /// Removes all itinerary items associated with a given trip, used before regenerating an AI itinerary.
        /// </summary>
        /// <param name="tripId">The ID of the trip whose itineraries should be cleared.</param>
        Task ClearItinerariesAsync(int tripId);

        /// <summary>
        /// Stages a collection of new itinerary items for bulk insertion.
        /// </summary>
        /// <param name="items">The itinerary items to add.</param>
        Task AddItinerariesAsync(IEnumerable<Itinerary> items);

        /// <summary>
        /// Commits all pending EF Core changes to the database.
        /// </summary>
        Task SaveChangesAsync();
    }
}
