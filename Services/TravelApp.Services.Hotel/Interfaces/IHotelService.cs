using TravelApp.Services.Hotel.DTOs;

namespace TravelApp.Services.Hotel.Interfaces
{
    /// <summary>
    /// Defines the contract for hotel and room management business logic, including approval workflows and caching.
    /// </summary>
    public interface IHotelService
    {
        /// <summary>
        /// Retrieves all approved hotels, optionally filtered by city.
        /// Results are served from Redis cache when available.
        /// </summary>
        /// <param name="city">Optional city name to filter hotels. Pass <c>null</c> to get all hotels.</param>
        /// <returns>A collection of approved <see cref="HotelDto"/> objects.</returns>
        Task<IEnumerable<HotelDto>> GetHotelsAsync(string? city);

        /// <summary>
        /// Retrieves the full details of a specific hotel, including its approved rooms.
        /// Results are served from Redis cache when available.
        /// </summary>
        /// <param name="id">The hotel's unique identifier.</param>
        /// <returns>The <see cref="HotelDto"/> if found; otherwise, <c>null</c>.</returns>
        Task<HotelDto?> GetHotelByIdAsync(int id);

        /// <summary>
        /// Creates a new hotel in <c>Pending</c> approval status on behalf of a Hotel Manager.
        /// Also publishes a notification event to inform admins.
        /// </summary>
        /// <param name="dto">The hotel creation payload.</param>
        /// <param name="ownerId">The authenticated manager's user ID.</param>
        /// <param name="ownerEmail">The manager's email address.</param>
        /// <param name="ownerName">The manager's display name.</param>
        /// <returns>The created <see cref="HotelDto"/>.</returns>
        Task<HotelDto> CreateHotelAsync(CreateHotelDto dto, int ownerId, string ownerEmail, string ownerName);

        /// <summary>
        /// Adds a new room in <c>Pending</c> approval status to a hotel owned by the requesting user.
        /// </summary>
        /// <param name="hotelId">The hotel to add the room to.</param>
        /// <param name="dto">The room creation payload.</param>
        /// <param name="requestingUserId">The user ID of the Hotel Manager making the request.</param>
        /// <returns>The created <see cref="RoomDto"/>, or <c>null</c> if the user does not own the hotel.</returns>
        Task<RoomDto?> AddRoomToHotelAsync(int hotelId, CreateRoomDto dto, int requestingUserId);

        /// <summary>
        /// Deletes a hotel and all its associated rooms from the database.
        /// </summary>
        /// <param name="id">The ID of the hotel to delete.</param>
        /// <returns><c>true</c> if deletion was successful; <c>false</c> if the hotel was not found.</returns>
        Task<bool> DeleteHotelAsync(int id);

        /// <summary>
        /// Retrieves all hotels owned by a specific Hotel Manager.
        /// </summary>
        /// <param name="ownerId">The owner's user ID.</param>
        /// <returns>A collection of <see cref="HotelDto"/> owned by that manager.</returns>
        Task<IEnumerable<HotelDto>> GetHotelsByOwnerAsync(int ownerId);

        /// <summary>
        /// Approves a pending hotel, making it publicly visible. Publishes a <c>HotelApproved</c> event.
        /// </summary>
        /// <param name="id">The ID of the hotel to approve.</param>
        /// <returns>The updated <see cref="HotelDto"/>, or <c>null</c> if not found.</returns>
        Task<HotelDto?> ApproveHotelAsync(int id);

        /// <summary>
        /// Rejects a pending hotel with a reason. Publishes a <c>HotelRejected</c> event.
        /// </summary>
        /// <param name="id">The ID of the hotel to reject.</param>
        /// <param name="reason">An optional human-readable rejection reason.</param>
        /// <returns>The updated <see cref="HotelDto"/>, or <c>null</c> if not found.</returns>
        Task<HotelDto?> RejectHotelAsync(int id, string? reason);

        /// <summary>
        /// Retrieves all hotels currently in <c>Pending</c> approval status.
        /// </summary>
        /// <returns>A collection of pending <see cref="HotelDto"/> objects.</returns>
        Task<IEnumerable<HotelDto>> GetPendingHotelsAsync();

        /// <summary>
        /// Approves a specific pending room within a hotel.
        /// </summary>
        /// <param name="hotelId">The ID of the hotel that owns the room.</param>
        /// <param name="roomId">The ID of the room to approve.</param>
        /// <returns>The updated <see cref="RoomDto"/>, or <c>null</c> if not found.</returns>
        Task<RoomDto?> ApproveRoomAsync(int hotelId, int roomId);

        /// <summary>
        /// Rejects a specific pending room within a hotel.
        /// </summary>
        /// <param name="hotelId">The ID of the hotel that owns the room.</param>
        /// <param name="roomId">The ID of the room to reject.</param>
        /// <returns>The updated <see cref="RoomDto"/>, or <c>null</c> if not found.</returns>
        Task<RoomDto?> RejectRoomAsync(int hotelId, int roomId);

        /// <summary>
        /// Retrieves all rooms currently in <c>Pending</c> approval status across all hotels.
        /// </summary>
        /// <returns>A collection of pending <see cref="RoomDto"/> objects.</returns>
        Task<IEnumerable<RoomDto>> GetPendingRoomsAsync();

        /// <summary>
        /// Retrieves the current approval status and details of a specific room.
        /// Used internally by the Booking service to check room availability before confirming a booking.
        /// </summary>
        /// <param name="roomId">The room's unique identifier.</param>
        /// <returns>The <see cref="RoomDto"/> if found; otherwise, <c>null</c>.</returns>
        Task<RoomDto?> GetRoomStatusAsync(int roomId);
    }
}
