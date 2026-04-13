using TravelApp.Services.Booking.DTOs;

namespace TravelApp.Services.Booking.Clients;

/// <summary>
/// Defines the contract for the internal HTTP client that communicates with the Hotel service.
/// </summary>
public interface IHotelClient
{
    /// <summary>
    /// Retrieves the approval status of a specific room from the Hotel service.
    /// </summary>
    /// <param name="roomId">The ID of the room to query.</param>
    /// <returns>A <see cref="RoomStatusDto"/> with the room's status, or <c>null</c> if the Hotel service is unreachable or the room is not found.</returns>
    Task<RoomStatusDto?> GetRoomAsync(int roomId);
}
