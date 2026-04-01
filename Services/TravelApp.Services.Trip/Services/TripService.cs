using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;
using TravelApp.Services.Trip.AI;
using TravelApp.Services.Trip.DTOs;
using TravelApp.Services.Trip.Interfaces;

namespace TravelApp.Services.Trip.Services
{
    public class TripService : ITripService
    {


        private readonly ITripRepository _repo;
        private readonly IGeminiService _gemini;
        private readonly IDistributedCache _cache;
        private readonly ILogger<TripService> _logger;

        public TripService(ITripRepository repo, IGeminiService gemini, IDistributedCache cache, ILogger<TripService> logger)
        {
            _repo = repo;
            _gemini = gemini;
            _cache = cache;
            _logger = logger;
        }


        public async Task<TripDto?> CreateTripAsync(CreateTripDto dto, int userId)
        {
            if(dto.EndDate <= dto.StartDate)
            {
                return null;
            }

            var trip = new Models.Trip()
            {
                UserId = userId,
                Destination = dto.Destination,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                CreatedAt = DateTime.UtcNow
            };


            await _repo.AddTripAsync(trip);
            await _repo.SaveChangesAsync();
            await _cache.RemoveAsync($"trips-user-{userId}");

            return MapTrip(trip);
        }

        public async Task<bool> DeleteTripAsync(int id, int userId)
        {
            var trip = await _repo.GetTripByIdAsync(id);
            if (trip == null || trip.UserId != userId)
            {
                return false;
            }
            await _repo.DeleteTripAsync(trip);
            await _repo.SaveChangesAsync();
            await _cache.RemoveAsync($"trip-{id}-{userId}");
            await _cache.RemoveAsync($"trips-user-{userId}");
            return true;
        }

        public async Task<TripDto?> GenerateItineraryAsync(GenerateItineraryDto dto, int userId)
        {
            var trip = await _repo.GetTripByIdAsync(dto.TripId);
            if(trip == null || trip.UserId != userId)
            {
                return null;
            }

            await _repo.ClearItinerariesAsync(trip.Id);

            var items = await _gemini.GetItineraryAsync(trip.Destination, trip.StartDate, trip.EndDate, dto.Preferences);

            foreach(var item in items)
            {
                item.TripId = trip.Id;
            }


            await _repo.AddItinerariesAsync(items);
            await _repo.SaveChangesAsync();


            var updated = await _repo.GetTripByIdAsync(trip.Id);

            await _cache.RemoveAsync($"trip-{trip.Id}-{userId}");
            await _cache.RemoveAsync($"trips-user-{userId}");
            return updated != null ? MapTrip(updated) : null;
        }

        public async Task<TripDto?> GetTripByIdAsync(int id, int userId)
        {
            var cacheKey = $"trip-{id}-{userId}";
            var cachedData = await _cache.GetStringAsync(cacheKey);

            //Cache Hit
            if (!string.IsNullOrEmpty(cachedData))
            {
                _logger.LogInformation("Cache hit for GetTripById: {CacheKey}", cacheKey);
                return JsonSerializer.Deserialize<TripDto>(cachedData);
            }

            _logger.LogInformation("Cache miss for GetTripById: {CacheKey}. Fetching from DB...", cacheKey);

            //Cache Miss
            var trip = await _repo.GetTripByIdAsync(id);
            if (trip == null || trip.UserId != userId)
            {
                return null;
            }
            var dto = MapTrip(trip);

            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30)
            };
            await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(dto), options);
            _logger.LogInformation("Saved trip to cache: {CacheKey}", cacheKey);

            return dto;
        }

        public async Task<IEnumerable<TripDto>> GetUserTripsAsync(int userId)
        {
            var cacheKey = $"trips-user-{userId}";
            var cachedData = await _cache.GetStringAsync(cacheKey);

            //Cache Hit
            if (!string.IsNullOrEmpty(cachedData))
            {
                _logger.LogInformation("Cache hit for GetUserTrips: {CacheKey}", cacheKey);
                return JsonSerializer.Deserialize<IEnumerable<TripDto>>(cachedData) ?? new List<TripDto>();
            }

            _logger.LogInformation("Cache miss for GetUserTrips: {CacheKey}. Fetching from DB...", cacheKey);

            //Cache Miss
            var trips = await _repo.GetUserTripsAsync(userId);
            var dto = trips.Select(MapTrip).ToList();

            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30)
            };

            await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(dto) , options);
            _logger.LogInformation("Saved {Count} trips to cache: {CacheKey}", dto.Count, cacheKey);

            return dto;
        }


        private static TripDto MapTrip(Models.Trip t) => new(
        t.Id, t.UserId, t.Destination, t.StartDate, t.EndDate, t.CreatedAt,
        t.Itineraries.OrderBy(i => i.DayNumber)
            .Select(i => new ItineraryDto(i.Id, i.TripId, i.DayNumber, i.Activity, i.Location))
            .ToList());
    }
}
