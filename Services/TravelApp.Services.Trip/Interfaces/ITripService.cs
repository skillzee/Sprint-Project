using TravelApp.Services.Trip.DTOs;

namespace TravelApp.Services.Trip.Interfaces
{
    /// <summary>
    /// Defines the contract for trip planning business logic, including AI itinerary generation and Redis caching.
    /// </summary>
    public interface ITripService
    {
        /// <summary>
        /// Retrieves all trips belonging to a specific user.
        /// Results are cached in Redis for 30 minutes; cache is invalidated when trips are created or deleted.
        /// </summary>
        /// <param name="userId">The ID of the authenticated user.</param>
        /// <returns>A collection of <see cref="TripDto"/> objects for that user.</returns>
        Task<IEnumerable<TripDto>> GetUserTripsAsync(int userId);

        /// <summary>
        /// Retrieves a single trip by ID, ensuring it belongs to the requesting user.
        /// Result is cached in Redis for 30 minutes.
        /// </summary>
        /// <param name="id">The trip's unique identifier.</param>
        /// <param name="userId">The authenticated user's ID used for ownership validation.</param>
        /// <returns>The <see cref="TripDto"/> if found and owned by the user; otherwise, <c>null</c>.</returns>
        Task<TripDto?> GetTripByIdAsync(int id, int userId);

        /// <summary>
        /// Creates a new trip for a user after validating that the end date is after the start date.
        /// Invalidates the user's trip list cache on success.
        /// </summary>
        /// <param name="dto">The trip creation payload containing destination and dates.</param>
        /// <param name="userId">The ID of the authenticated user creating the trip.</param>
        /// <returns>The created <see cref="TripDto"/>, or <c>null</c> if validation fails.</returns>
        Task<TripDto?> CreateTripAsync(CreateTripDto dto, int userId);

        /// <summary>
        /// Uses the Gemini AI service to generate a day-by-day itinerary for an existing trip,
        /// replacing any previously generated itinerary. Invalidates related cache entries on success.
        /// </summary>
        /// <param name="dto">The generation request containing the trip ID and optional user preferences.</param>
        /// <param name="userId">The authenticated user's ID for ownership validation.</param>
        /// <returns>The updated <see cref="TripDto"/> with the new itinerary, or <c>null</c> if the trip is not found.</returns>
        Task<TripDto?> GenerateItineraryAsync(GenerateItineraryDto dto, int userId);

        /// <summary>
        /// Deletes a trip and its associated itinerary items. Only the owning user can delete their trip.
        /// Invalidates all related cache entries on success.
        /// </summary>
        /// <param name="id">The ID of the trip to delete.</param>
        /// <param name="userId">The authenticated user's ID for ownership validation.</param>
        /// <returns><c>true</c> if deletion succeeded; <c>false</c> if the trip was not found or not owned by the user.</returns>
        Task<bool> DeleteTripAsync(int id, int userId);
    }
}
