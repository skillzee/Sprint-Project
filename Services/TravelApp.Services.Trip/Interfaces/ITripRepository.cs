using TravelApp.Services.Trip.Models;

namespace TravelApp.Services.Trip.Interfaces
{
    public interface ITripRepository
    {
        Task<IEnumerable<Models.Trip>> GetUserTripsAsync(int userId);
        Task<Models.Trip?> GetTripByIdAsync(int id);
        Task AddTripAsync(Models.Trip trip);
        Task DeleteTripAsync(Models.Trip trip);
        Task ClearItinerariesAsync(int tripId);
        Task AddItinerariesAsync(IEnumerable<Itinerary> items);
        Task SaveChangesAsync();
    }
}
