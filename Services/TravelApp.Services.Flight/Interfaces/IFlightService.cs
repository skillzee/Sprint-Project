using TravelApp.Services.Flight.DTOs;

namespace TravelApp.Services.Flight.Interfaces
{
    public interface IFlightService
    {
        Task<List<FlightOfferDto>> SearchFlightsAsync(string origin, string destination, string date, int adults, string cabinClass);
    }
}
