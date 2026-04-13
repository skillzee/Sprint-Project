using TravelApp.Services.Booking.DTOs;

namespace TravelApp.Services.Booking.Interfaces
{
    /// <summary>
    /// Defines the contract for booking business logic operations.
    /// </summary>
    public interface IBookingService
    {
        /// <summary>
        /// Retrieves all bookings across all users along with the total revenue generated.
        /// Intended for Admin use only.
        /// </summary>
        /// <returns>An anonymous object containing a list of bookings and the total revenue sum.</returns>
        Task<Object> GetAllBookingWithRevenueAsync();

        /// <summary>
        /// Retrieves all bookings made by a specific user.
        /// </summary>
        /// <param name="userId">The unique identifier of the user.</param>
        /// <returns>A collection of <see cref="BookingDto"/> for that user, ordered newest first.</returns>
        Task<IEnumerable<BookingDto>> GetUserBookingsAsync(int userId);

        /// <summary>
        /// Creates a new confirmed booking after performing date validation, overlap checks, and room availability verification.
        /// Also publishes a <c>BookingConfirmedEvent</c> to notify downstream services (e.g., Notification).
        /// </summary>
        /// <param name="dto">The booking request details including room, hotel, and dates.</param>
        /// <param name="userId">The ID of the user placing the booking.</param>
        /// <param name="userName">The display name of the user.</param>
        /// <param name="userEmai">The email address of the user for confirmation notifications.</param>
        /// <returns>
        /// A tuple of the created <see cref="BookingDto"/> and a <c>null</c> error message on success,
        /// or a <c>null</c> result and a descriptive error message on failure.
        /// </returns>
        Task<(BookingDto? result, string? errorMessage)> CreateBookingAsync(CreateBookingDto dto, int userId, string userName, string userEmai);

        /// <summary>
        /// Cancels an existing booking by ID. Admins can cancel any booking; regular users can only cancel their own.
        /// Also publishes a <c>BookingCancelledEvent</c> to notify downstream services.
        /// </summary>
        /// <param name="id">The ID of the booking to cancel.</param>
        /// <param name="userId">The ID of the requesting user.</param>
        /// <param name="role">The role of the requesting user (<c>"Admin"</c> or <c>"Customer"</c>).</param>
        /// <returns><c>true</c> if the booking was successfully cancelled; otherwise, <c>false</c>.</returns>
        Task<bool> CancelBookingAsync(int id, int userId, string role);
    }
}
