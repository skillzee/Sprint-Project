using TravelApp.Services.Hotel.DTOs;

namespace TravelApp.Services.Hotel.Interfaces
{
    public interface IHotelService
    {
        Task<IEnumerable<HotelDto>> GetHotelsAsync(string? city);
        Task<HotelDto?> GetHotelByIdAsync(int id);
        Task<HotelDto> CreateHotelAsync(CreateHotelDto dto, int ownerId, string ownerEmail, string ownerName);
        Task<RoomDto?> AddRoomToHotelAsync(int hotelId, CreateRoomDto dto, int requestingUserId);
        Task<bool> DeleteHotelAsync(int id);

        // Owner-scoped
        Task<IEnumerable<HotelDto>> GetHotelsByOwnerAsync(int ownerId);

        // Admin approval
        Task<HotelDto?> ApproveHotelAsync(int id);
        Task<HotelDto?> RejectHotelAsync(int id, string? reason);
        Task<IEnumerable<HotelDto>> GetPendingHotelsAsync();

        // Room approval
        Task<RoomDto?> ApproveRoomAsync(int hotelId, int roomId);
        Task<RoomDto?> RejectRoomAsync(int hotelId, int roomId);
        Task<IEnumerable<RoomDto>> GetPendingRoomsAsync();
        Task<RoomDto?> GetRoomStatusAsync(int roomId);
    }
}
