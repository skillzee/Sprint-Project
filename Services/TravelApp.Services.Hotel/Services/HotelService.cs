using TravelApp.Services.Hotel.DTOs;
using TravelApp.Services.Hotel.Interfaces;
using TravelApp.Services.Hotel.Models;

namespace TravelApp.Services.Hotel.Services
{
    public class HotelService : IHotelService
    {

        private readonly IHotelRepository _repo;
        public HotelService(IHotelRepository repo)
        {
            _repo = repo;
        }


        public async Task<RoomDto?> AddRoomToHotelAsync(int hotelId, CreateRoomDto dto)
        {
            var hotel = await _repo.GetByIdAsync(hotelId);
            if(hotel == null)
            {
                return null;
            }

            var room = new Room
            {
                HotelId = hotelId,
                Type = dto.Type,
                PricePerNight = dto.PricePerNight,
                MaxOccupancy = dto.MaxOccupancy,
                Description = dto.Description
            };

            await _repo.AddRoomAsync(room);
            await _repo.SaveChangesAsync();

            return ToRoomDto(room);
        }

        public async Task<HotelDto> CreateHotelAsync(CreateHotelDto dto)
        {
            var hotel = new Models.Hotel
            {
                Name = dto.Name,
                City = dto.City,
                Address = dto.Address,
                Description = dto.Description,
                StarRating = dto.StarRating,
                Amenities = dto.Amenities
            };

            await _repo.AddHotelAsync(hotel);
            await _repo.SaveChangesAsync();

            return ToDto(hotel);
        }

        public async Task<bool> DeleteHotelAsync(int id)
        {
            var hotel = await _repo.GetByIdAsync(id);
            if(hotel == null)
            {
                return false;
            }
            await _repo.DeleteHotelAsync(hotel);
            await _repo.SaveChangesAsync();

            return true;
        }

        public async Task<HotelDto?> GetHotelByIdAsync(int id)
        {
            var hotel = await _repo.GetByIdWithRoomsAsync(id);
            if (hotel == null)
            {
                return null;
            }
            return ToDto(hotel);

        }

        public async Task<IEnumerable<HotelDto>> GetHotelsAsync(string? city)
        {
            var hotels = await _repo.GetAllWithRoomsAsync(city);
            return hotels.Select(ToDto); 

        }


        private static HotelDto ToDto(Models.Hotel h) =>
           new(
               h.Id,
               h.Name,
               h.City,
               h.Address,
               h.Description,
               h.StarRating,
               h.Amenities,
               h.Rooms.Select(ToRoomDto).ToList()
           );
        private static RoomDto ToRoomDto(Room r) =>
            new(
                r.Id,
                r.HotelId,
                r.Type,
                r.PricePerNight,
                r.MaxOccupancy,
                r.IsAvailable,
                r.Description
            );
    }
}
