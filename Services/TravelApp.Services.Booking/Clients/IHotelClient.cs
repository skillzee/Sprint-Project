using TravelApp.Services.Booking.DTOs;

namespace TravelApp.Services.Booking.Clients;

public interface IHotelClient
{
    Task<RoomStatusDto?> GetRoomAsync(int roomId);
}
