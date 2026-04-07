using TravelApp.Services.Flight.DTOs;

namespace TravelApp.Services.Flight.Interfaces
{
    public interface IFlightRepository
    {

        Task<SerpApiFlightResponse?> GetFlightsRawAsync(string origin, string destination, string date, int adults, int travelClass);

    }
}
