using TravelApp.Services.Hotel.DTOs;

namespace TravelApp.Services.Hotel.Interfaces
{
    public interface IHotelService
    {
        Task<IEnumerable<HotelDto>> GetHotelsAsync(string? city);
        Task<HotelDto?> GetHotelByIdAsync(int id);
        Task<HotelDto> CreateHotelAsync(CreateHotelDto dto);
        Task<RoomDto?> AddRoomToHotelAsync(int hotelId, CreateRoomDto dto);
        Task<bool> DeleteHotelAsync(int id);
    }
}
