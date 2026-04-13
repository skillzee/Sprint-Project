using TravelApp.Services.Hotel.Models;

namespace TravelApp.Services.Hotel.Interfaces
{
    /// <summary>
    /// Defines the data access contract for hotel and room persistence operations.
    /// </summary>
    public interface IHotelRepository
    {
        /// <summary>
        /// Retrieves all approved hotels with their approved rooms, optionally filtered by city.
        /// </summary>
        /// <param name="city">Optional city name filter. Pass <c>null</c> to get all hotels.</param>
        /// <returns>A collection of <see cref="Models.Hotel"/> entities with rooms eagerly loaded.</returns>
        Task<IEnumerable<Models.Hotel>> GetAllWithRoomsAsync(string? city);

        /// <summary>
        /// Retrieves a hotel by its ID with all rooms eagerly loaded.
        /// </summary>
        /// <param name="id">The hotel's primary key.</param>
        /// <returns>The <see cref="Models.Hotel"/> with rooms, or <c>null</c> if not found.</returns>
        Task<Models.Hotel?> GetByIdWithRoomsAsync(int id);

        /// <summary>
        /// Retrieves a hotel by its ID without loading rooms.
        /// </summary>
        /// <param name="id">The hotel's primary key.</param>
        /// <returns>The <see cref="Models.Hotel"/>, or <c>null</c> if not found.</returns>
        Task<Models.Hotel?> GetByIdAsync(int id);

        /// <summary>
        /// Stages a new hotel for insertion (call <see cref="SaveChangesAsync"/> to persist).
        /// </summary>
        /// <param name="hotel">The hotel entity to add.</param>
        Task AddHotelAsync(Models.Hotel hotel);

        /// <summary>
        /// Stages a new room for insertion (call <see cref="SaveChangesAsync"/> to persist).
        /// </summary>
        /// <param name="room">The room entity to add.</param>
        Task AddRoomAsync(Room room);

        /// <summary>
        /// Removes a hotel entity from the database (call <see cref="SaveChangesAsync"/> to persist).
        /// </summary>
        /// <param name="hotel">The hotel entity to delete.</param>
        Task DeleteHotelAsync(Models.Hotel hotel);

        /// <summary>
        /// Commits all pending EF Core changes to the database.
        /// </summary>
        Task SaveChangesAsync();

        /// <summary>
        /// Retrieves all hotels with an <c>ApprovalStatus</c> of <c>"Pending"</c>.
        /// </summary>
        /// <returns>A collection of pending <see cref="Models.Hotel"/> entities.</returns>
        Task<IEnumerable<Models.Hotel>> GetPendingHotelsAsync();

        /// <summary>
        /// Retrieves all rooms with an <c>ApprovalStatus</c> of <c>"Pending"</c> across all hotels.
        /// </summary>
        /// <returns>A collection of pending <see cref="Room"/> entities.</returns>
        Task<IEnumerable<Room>> GetPendingRoomsAsync();

        /// <summary>
        /// Retrieves all hotels owned by a specific manager user ID.
        /// </summary>
        /// <param name="ownerId">The owner's user ID.</param>
        /// <returns>A collection of <see cref="Models.Hotel"/> entities owned by that user.</returns>
        Task<IEnumerable<Models.Hotel>> GetHotelsByOwnerAsync(int ownerId);

        /// <summary>
        /// Retrieves a single room by its unique ID.
        /// </summary>
        /// <param name="roomId">The room's primary key.</param>
        /// <returns>The <see cref="Room"/> entity, or <c>null</c> if not found.</returns>
        Task<Room?> GetRoomByIdAsync(int roomId);
    }
}
