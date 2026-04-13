using TravelApp.Services.Flight.DTOs;

namespace TravelApp.Services.Flight.Interfaces
{
    /// <summary>
    /// Defines the contract for flight search business logic, including caching.
    /// </summary>
    public interface IFlightService
    {
        /// <summary>
        /// Searches for available flight offers between two airports on a given date.
        /// Results are cached in Redis for 1 hour to reduce external API usage.
        /// </summary>
        /// <param name="origin">The IATA code of the departure airport (e.g., <c>"DEL"</c>).</param>
        /// <param name="destination">The IATA code of the arrival airport (e.g., <c>"BOM"</c>).</param>
        /// <param name="date">The departure date in <c>YYYY-MM-DD</c> format.</param>
        /// <param name="adults">The number of adult passengers.</param>
        /// <param name="cabinClass">The cabin class (e.g., <c>"economy"</c>, <c>"business"</c>, <c>"first"</c>).</param>
        /// <returns>A list of matching <see cref="FlightOfferDto"/> results, up to 15 offers.</returns>
        Task<List<FlightOfferDto>> SearchFlightsAsync(string origin, string destination, string date, int adults, string cabinClass);
    }
}
