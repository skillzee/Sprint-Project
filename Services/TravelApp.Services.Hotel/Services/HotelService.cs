using MassTransit;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;
using TravelApp.Services.Hotel.DTOs;
using TravelApp.Services.Hotel.Interfaces;
using TravelApp.Services.Hotel.Models;
using TravelApp.Shared;

namespace TravelApp.Services.Hotel.Services
{
    public class HotelService : IHotelService
    {
        private readonly IHotelRepository _repo;
        private readonly IDistributedCache _cache;
        private readonly ILogger<HotelService> _logger;
        private readonly IPublishEndpoint _publishEndpoint;

        public HotelService(
            IHotelRepository repo,
            IDistributedCache cache,
            ILogger<HotelService> logger,
            IPublishEndpoint publishEndpoint)
        {
            _repo = repo;
            _cache = cache;
            _logger = logger;
            _publishEndpoint = publishEndpoint;
        }

        public async Task<HotelDto> CreateHotelAsync(CreateHotelDto dto, int ownerId, string ownerEmail, string ownerName)
        {
            var hotel = new Models.Hotel
            {
                Name = dto.Name,
                City = dto.City,
                Address = dto.Address,
                Description = dto.Description,
                StarRating = dto.StarRating,
                Amenities = dto.Amenities,
                ApprovalStatus = "Pending",
                OwnerId = ownerId,
                OwnerEmail = ownerEmail,
                OwnerName = ownerName
            };

            await _repo.AddHotelAsync(hotel);
            await _repo.SaveChangesAsync();

            await _cache.RemoveAsync("hotels-all");
            await _cache.RemoveAsync($"hotels-city-{hotel.City.ToLower()}");

            return ToDto(hotel);
        }

        public async Task<RoomDto?> AddRoomToHotelAsync(int hotelId, CreateRoomDto dto, int requestingUserId)
        {
            var hotel = await _repo.GetByIdAsync(hotelId);
            if (hotel == null)
                return null;

            // Ownership check
            if (hotel.OwnerId != requestingUserId)
                return null;

            var room = new Room
            {
                HotelId = hotelId,
                Type = dto.Type,
                PricePerNight = dto.PricePerNight,
                MaxOccupancy = dto.MaxOccupancy,
                Description = dto.Description,
                ApprovalStatus = "Pending"
            };

            await _repo.AddRoomAsync(room);
            await _repo.SaveChangesAsync();

            await _cache.RemoveAsync($"hotel-{hotelId}");
            await _cache.RemoveAsync($"hotels-city-{hotel.City.ToLower()}");
            await _cache.RemoveAsync("hotels-all");

            return ToRoomDto(room);
        }

        public async Task<bool> DeleteHotelAsync(int id)
        {
            var hotel = await _repo.GetByIdAsync(id);
            if (hotel == null)
                return false;

            await _repo.DeleteHotelAsync(hotel);
            await _repo.SaveChangesAsync();

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
            if (hotel == null || hotel.ApprovalStatus != "Approved")
                return null;

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
            // Filter to approved only for public-facing endpoint
            var dtos = hotels.Where(h => h.ApprovalStatus == "Approved").Select(ToDto).ToList();

            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(15)
            };
            await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(dtos), options);
            _logger.LogInformation("Saved {Count} hotels to cache: {CacheKey}", dtos.Count, cacheKey);

            return dtos;
        }

        public async Task<IEnumerable<HotelDto>> GetHotelsByOwnerAsync(int ownerId)
        {
            var hotels = await _repo.GetHotelsByOwnerAsync(ownerId);
            return hotels.Select(ToDto);
        }

        public async Task<IEnumerable<HotelDto>> GetPendingHotelsAsync()
        {
            var hotels = await _repo.GetPendingHotelsAsync();
            return hotels.Select(ToDto);
        }

        public async Task<IEnumerable<RoomDto>> GetPendingRoomsAsync()
        {
            var rooms = await _repo.GetPendingRoomsAsync();
            return rooms.Select(ToRoomDto);
        }

        public async Task<HotelDto?> ApproveHotelAsync(int id)
        {
            var hotel = await _repo.GetByIdWithRoomsAsync(id);
            if (hotel == null)
                return null;

            hotel.ApprovalStatus = "Approved";
            hotel.RejectionReason = null;
            await _repo.SaveChangesAsync();

            await InvalidateHotelCache(hotel);

            await _publishEndpoint.Publish(new HotelApprovedEvent
            {
                HotelId = hotel.Id,
                HotelName = hotel.Name,
                OwnerEmail = hotel.OwnerEmail,
                OwnerName = hotel.OwnerName
            });

            return ToDto(hotel);
        }

        public async Task<HotelDto?> RejectHotelAsync(int id, string? reason)
        {
            var hotel = await _repo.GetByIdWithRoomsAsync(id);
            if (hotel == null)
                return null;

            hotel.ApprovalStatus = "Rejected";
            hotel.RejectionReason = reason;
            await _repo.SaveChangesAsync();

            await InvalidateHotelCache(hotel);

            await _publishEndpoint.Publish(new HotelRejectedEvent
            {
                HotelId = hotel.Id,
                HotelName = hotel.Name,
                OwnerEmail = hotel.OwnerEmail,
                OwnerName = hotel.OwnerName,
                Reason = reason
            });

            return ToDto(hotel);
        }

        public async Task<RoomDto?> ApproveRoomAsync(int hotelId, int roomId)
        {
            var room = await _repo.GetRoomByIdAsync(roomId);
            if (room == null || room.HotelId != hotelId)
                return null;

            room.ApprovalStatus = "Approved";
            await _repo.SaveChangesAsync();

            var hotel = await _repo.GetByIdAsync(hotelId);
            await InvalidateHotelCache(hotel, hotelId);

            return ToRoomDto(room);
        }

        public async Task<RoomDto?> RejectRoomAsync(int hotelId, int roomId)
        {
            var room = await _repo.GetRoomByIdAsync(roomId);
            if (room == null || room.HotelId != hotelId)
                return null;

            room.ApprovalStatus = "Rejected";
            await _repo.SaveChangesAsync();

            var hotel = await _repo.GetByIdAsync(hotelId);
            await InvalidateHotelCache(hotel, hotelId);

            return ToRoomDto(room);
        }

        public async Task<RoomDto?> GetRoomStatusAsync(int roomId)
        {
            var room = await _repo.GetRoomByIdAsync(roomId);
            return room == null ? null : ToRoomDto(room);
        }

        private async Task InvalidateHotelCache(Models.Hotel? hotel, int? hotelId = null)
        {
            var id = hotel?.Id ?? hotelId;
            if (id.HasValue)
                await _cache.RemoveAsync($"hotel-{id}");

            if (hotel != null)
                await _cache.RemoveAsync($"hotels-city-{hotel.City.ToLower()}");

            await _cache.RemoveAsync("hotels-all");
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
                h.Rooms.Select(ToRoomDto).ToList(),
                h.OwnerId,
                h.OwnerEmail,
                h.OwnerName,
                h.ApprovalStatus
            );

        private static RoomDto ToRoomDto(Room r) =>
            new(
                r.Id,
                r.HotelId,
                r.Type,
                r.PricePerNight,
                r.MaxOccupancy,
                r.IsAvailable,
                r.Description,
                r.ApprovalStatus
            );
    }
}
