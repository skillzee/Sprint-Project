using TravelApp.Services.Trip.DTOs;
using TravelApp.Services.Trip.Models;

namespace TravelApp.Services.Trip.Interfaces
{
    public interface ITripService
    {

        Task<IEnumerable<TripDto>> GetUserTripsAsync(int userId);
        Task<TripDto?> GetTripByIdAsync(int id, int userId);
        Task<TripDto?> CreateTripAsync(CreateTripDto dto, int userId);
        Task<TripDto?> GenerateItineraryAsync(GenerateItineraryDto dto, int userId);
        Task<bool> DeleteTripAsync(int id, int userId);

    }
}
