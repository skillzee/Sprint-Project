using MassTransit;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;
using TravelApp.Services.Hotel.DTOs;
using TravelApp.Services.Hotel.Interfaces;
using TravelApp.Services.Hotel.Models;
using TravelApp.Shared;

namespace TravelApp.Services.Hotel.Services
{
    /// <summary>
    /// Implements hotel and room management business logic, including Redis caching and RabbitMQ event publishing.
    /// </summary>
    public class HotelService : IHotelService
    {
        private readonly IHotelRepository _repo;
        private readonly IDistributedCache _cache;
        private readonly ILogger<HotelService> _logger;
        private readonly IPublishEndpoint _publishEndpoint;

        /// <summary>
        /// Initializes a new instance of <see cref="HotelService"/>.
        /// </summary>
        /// <param name="repo">The hotel data access repository.</param>
        /// <param name="cache">The distributed Redis cache.</param>
        /// <param name="logger">The logger instance.</param>
        /// <param name="publishEndpoint">The MassTransit publish endpoint for raising domain events.</param>
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

        /// <summary>
        /// Creates a new hotel in <c>Pending</c> status and invalidates related cache entries.
        /// </summary>
        /// <param name="dto">The hotel creation payload.</param>
        /// <param name="ownerId">The authenticated manager's user ID.</param>
        /// <param name="ownerEmail">The manager's email address.</param>
        /// <param name="ownerName">The manager's display name.</param>
        /// <returns>The created <see cref="HotelDto"/>.</returns>
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

        /// <summary>
        /// Adds a new room in <c>Pending</c> status to a hotel, enforcing ownership. Invalidates related cache entries.
        /// </summary>
        /// <param name="hotelId">The hotel to add the room to.</param>
        /// <param name="dto">The room creation payload.</param>
        /// <param name="requestingUserId">The user ID of the Hotel Manager making the request.</param>
        /// <returns>The created <see cref="RoomDto"/>, or <c>null</c> if the user does not own the hotel.</returns>
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

        /// <summary>
        /// Deletes a hotel and invalidates all related cache entries.
        /// </summary>
        /// <param name="id">The ID of the hotel to delete.</param>
        /// <returns><c>true</c> if deletion succeeded; <c>false</c> if the hotel was not found.</returns>
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

        /// <summary>
        /// Retrieves a specific hotel by ID from cache or database. Only returns approved hotels.
        /// </summary>
        /// <param name="id">The hotel's unique identifier.</param>
        /// <returns>The <see cref="HotelDto"/> if found and approved; otherwise, <c>null</c>.</returns>
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

        /// <summary>
        /// Retrieves all approved hotels, optionally filtered by city. Results are served from Redis cache when available.
        /// </summary>
        /// <param name="city">Optional city name to filter hotels. Pass <c>null</c> to get all hotels.</param>
        /// <returns>A collection of approved <see cref="HotelDto"/> objects.</returns>
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

        /// <summary>
        /// Retrieves all hotels owned by a specific Hotel Manager.
        /// </summary>
        /// <param name="ownerId">The owner's user ID.</param>
        /// <returns>A collection of <see cref="HotelDto"/> owned by that manager.</returns>
        public async Task<IEnumerable<HotelDto>> GetHotelsByOwnerAsync(int ownerId)
        {
            var hotels = await _repo.GetHotelsByOwnerAsync(ownerId);
            return hotels.Select(ToDto);
        }

        /// <summary>
        /// Retrieves all hotels currently in <c>Pending</c> approval status.
        /// </summary>
        /// <returns>A collection of pending <see cref="HotelDto"/> objects.</returns>
        public async Task<IEnumerable<HotelDto>> GetPendingHotelsAsync()
        {
            var hotels = await _repo.GetPendingHotelsAsync();
            return hotels.Select(ToDto);
        }

        /// <summary>
        /// Retrieves all rooms currently in <c>Pending</c> approval status across all hotels.
        /// </summary>
        /// <returns>A collection of pending <see cref="RoomDto"/> objects.</returns>
        public async Task<IEnumerable<RoomDto>> GetPendingRoomsAsync()
        {
            var rooms = await _repo.GetPendingRoomsAsync();
            return rooms.Select(ToRoomDto);
        }

        /// <summary>
        /// Approves a pending hotel, making it publicly visible, and publishes a <c>HotelApprovedEvent</c>.
        /// </summary>
        /// <param name="id">The ID of the hotel to approve.</param>
        /// <returns>The updated <see cref="HotelDto"/>, or <c>null</c> if not found.</returns>
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

        /// <summary>
        /// Rejects a pending hotel with an optional reason and publishes a <c>HotelRejectedEvent</c>.
        /// </summary>
        /// <param name="id">The ID of the hotel to reject.</param>
        /// <param name="reason">An optional human-readable rejection reason.</param>
        /// <returns>The updated <see cref="HotelDto"/>, or <c>null</c> if not found.</returns>
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

        /// <summary>
        /// Approves a specific pending room within a hotel and invalidates related cache entries.
        /// </summary>
        /// <param name="hotelId">The ID of the hotel that owns the room.</param>
        /// <param name="roomId">The ID of the room to approve.</param>
        /// <returns>The updated <see cref="RoomDto"/>, or <c>null</c> if not found.</returns>
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

        /// <summary>
        /// Rejects a specific pending room within a hotel and invalidates related cache entries.
        /// </summary>
        /// <param name="hotelId">The ID of the hotel that owns the room.</param>
        /// <param name="roomId">The ID of the room to reject.</param>
        /// <returns>The updated <see cref="RoomDto"/>, or <c>null</c> if not found.</returns>
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

        /// <summary>
        /// Retrieves the current status and details of a specific room by ID.
        /// Used internally by the Booking service to verify room availability before confirming a booking.
        /// </summary>
        /// <param name="roomId">The room's unique identifier.</param>
        /// <returns>The <see cref="RoomDto"/> if found; otherwise, <c>null</c>.</returns>
        public async Task<RoomDto?> GetRoomStatusAsync(int roomId)
        {
            var room = await _repo.GetRoomByIdAsync(roomId);
            return room == null ? null : ToRoomDto(room);
        }

        /// <summary>
        /// Invalidates all cache keys related to a hotel (by ID and city) and the global hotels list.
        /// </summary>
        /// <param name="hotel">The hotel entity (used to derive the city cache key). Can be <c>null</c> if only ID is known.</param>
        /// <param name="hotelId">Fallback hotel ID when <paramref name="hotel"/> is <c>null</c>.</param>
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
