using TravelApp.Services.Hotel.Models;

namespace TravelApp.Services.Hotel.Interfaces
{
    public interface IHotelRepository
    {
        Task<IEnumerable<Models.Hotel>> GetAllWithRoomsAsync(string? city);
        Task<Models.Hotel?> GetByIdWithRoomsAsync(int id);
        Task<Models.Hotel?> GetByIdAsync(int id);
        Task AddHotelAsync(Models.Hotel hotel);
        Task AddRoomAsync(Room room);
        Task DeleteHotelAsync(Models.Hotel hotel);
        Task SaveChangesAsync();

        // New methods
        Task<IEnumerable<Models.Hotel>> GetPendingHotelsAsync();
        Task<IEnumerable<Room>> GetPendingRoomsAsync();
        Task<IEnumerable<Models.Hotel>> GetHotelsByOwnerAsync(int ownerId);
        Task<Room?> GetRoomByIdAsync(int roomId);
    }
}
