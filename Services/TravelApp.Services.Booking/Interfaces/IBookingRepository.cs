
namespace TravelApp.Services.Booking.Interfaces;

/// <summary>
/// Defines the data access contract for booking persistence operations.
/// </summary>
public interface IBookingRepository
{
    /// <summary>
    /// Retrieves all bookings from the database, ordered by creation date descending.
    /// </summary>
    /// <returns>A collection of all <see cref="Models.Booking"/> entities.</returns>
    Task<IEnumerable<Models.Booking>> GetAllAsync();

    /// <summary>
    /// Retrieves all bookings belonging to a specific user, ordered by creation date descending.
    /// </summary>
    /// <param name="userId">The ID of the user whose bookings to retrieve.</param>
    /// <returns>A collection of the user's <see cref="Models.Booking"/> entities.</returns>
    Task<IEnumerable<Models.Booking>> GetByUserIdAsync(int userId);

    /// <summary>
    /// Retrieves a single booking by its unique identifier.
    /// </summary>
    /// <param name="id">The booking's primary key.</param>
    /// <returns>The <see cref="Models.Booking"/> if found; otherwise, <c>null</c>.</returns>
    Task<Models.Booking?> GetByIdAsync(int id);

    /// <summary>
    /// Adds a new booking entity to the change tracker (call <see cref="SaveChangesAsync"/> to persist).
    /// </summary>
    /// <param name="booking">The <see cref="Models.Booking"/> to add.</param>
    Task AddAsync(Models.Booking booking);

    /// <summary>
    /// Checks if the user already has a confirmed booking for the same room that overlaps with the requested date range.
    /// </summary>
    /// <param name="userId">The user's ID.</param>
    /// <param name="roomId">The room's ID.</param>
    /// <param name="checkInDate">The desired check-in date.</param>
    /// <param name="checkOutDate">The desired check-out date.</param>
    /// <returns><c>true</c> if an overlapping booking exists; otherwise, <c>false</c>.</returns>
    Task<bool> HasOverlappingBookingAsync(int userId, int roomId, DateTime checkInDate, DateTime checkOutDate);

    /// <summary>
    /// Persists all pending changes to the database.
    /// </summary>
    Task SaveChangesAsync();
}