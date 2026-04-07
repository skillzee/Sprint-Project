using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;
using TravelApp.Services.Hotel.DTOs;
using TravelApp.Services.Hotel.Interfaces;
using TravelApp.Services.Hotel.Models;

namespace TravelApp.Services.Hotel.Services
{
    public class HotelService : IHotelService
    {
        private readonly IHotelRepository _repo;
        private readonly IDistributedCache _cache;
        private readonly ILogger<HotelService> _logger;

        public HotelService(IHotelRepository repo, IDistributedCache cache, ILogger<HotelService> logger)
        {
            _repo = repo;
            _cache = cache;
            _logger = logger;
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

            // Cache Invalidation: The hotel details have changed, so clear it from cache!
            await _cache.RemoveAsync($"hotel-{hotelId}");
            await _cache.RemoveAsync($"hotels-city-{hotel.City.ToLower()}");
            await _cache.RemoveAsync($"hotels-all");

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

            await _cache.RemoveAsync("hotels-all");
            await _cache.RemoveAsync($"hotels-city-{hotel.City.ToLower()}");

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

            // Cache Invalidation: Hotel no longer exists!
            await _cache.RemoveAsync($"hotel-{id}");
            await _cache.RemoveAsync($"hotels-city-{hotel.City.ToLower()}");
            await _cache.RemoveAsync("hotels-all");

            return true;
        }

        public async Task<HotelDto?> GetHotelByIdAsync(int id)
        {
            var cacheKey = $"hotel-{id}";
            var cachedData = await _cache.GetStringAsync(cacheKey);

            if (!string.IsNullOrEmpty(cachedData))
            {
                _logger.LogInformation("Cache hit for GetHotelById: {CacheKey}", cacheKey);
                return JsonSerializer.Deserialize<HotelDto>(cachedData);
            }

            _logger.LogInformation("Cache miss for GetHotelById: {CacheKey}. Fetching from DB...", cacheKey);

            var hotel = await _repo.GetByIdWithRoomsAsync(id);
            if (hotel == null)
            {
                return null;
            }
            
            var dto = ToDto(hotel);

            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1),
                SlidingExpiration = TimeSpan.FromMinutes(20)
            };
            await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(dto), options);
            _logger.LogInformation("Saved hotel to cache: {CacheKey}", cacheKey);

            return dto;
        }

        public async Task<IEnumerable<HotelDto>> GetHotelsAsync(string? city)
        {
            var cacheKey = string.IsNullOrWhiteSpace(city) ? "hotels-all" : $"hotels-city-{city.ToLower()}";
            var cachedData = await _cache.GetStringAsync(cacheKey);

            if (!string.IsNullOrEmpty(cachedData))
            {
                _logger.LogInformation("Cache hit for GetHotels: {CacheKey}", cacheKey);
                return JsonSerializer.Deserialize<IEnumerable<HotelDto>>(cachedData) ?? new List<HotelDto>();
            }

            _logger.LogInformation("Cache miss for GetHotels: {CacheKey}. Fetching from DB...", cacheKey);
            var hotels = await _repo.GetAllWithRoomsAsync(city);
            var dtos = hotels.Select(ToDto).ToList();

            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(15) // Shorter cache for listings
            };
            await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(dtos), options);
            _logger.LogInformation("Saved {Count} hotels to cache: {CacheKey}", dtos.Count, cacheKey);

            return dtos;
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
