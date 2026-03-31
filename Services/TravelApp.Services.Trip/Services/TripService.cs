using TravelApp.Services.Trip.AI;
using TravelApp.Services.Trip.DTOs;
using TravelApp.Services.Trip.Interfaces;

namespace TravelApp.Services.Trip.Services
{
    public class TripService : ITripService
    {


        private readonly ITripRepository _repo;
        private readonly IGeminiService _gemini;
        public TripService(ITripRepository repo, IGeminiService gemini)
        {
            _repo = repo;
            _gemini = gemini;
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
            return updated != null ? MapTrip(updated) : null;
        }

        public async Task<TripDto?> GetTripByIdAsync(int id, int userId)
        {
            var trip = await _repo.GetTripByIdAsync(id);
            if (trip == null || trip.UserId != userId)
            {
                return null;
            }
            return MapTrip(trip);
        }

        public async Task<IEnumerable<TripDto>> GetUserTripsAsync(int userId)
        {
            var trips = await _repo.GetUserTripsAsync(userId);
            return trips.Select(MapTrip);

        }


        private static TripDto MapTrip(Models.Trip t) => new(
        t.Id, t.UserId, t.Destination, t.StartDate, t.EndDate, t.CreatedAt,
        t.Itineraries.OrderBy(i => i.DayNumber)
            .Select(i => new ItineraryDto(i.Id, i.TripId, i.DayNumber, i.Activity, i.Location))
            .ToList());
    }
}
