using TravelApp.Services.Flight.DTOs;

namespace TravelApp.Services.Flight.Interfaces
{
    /// <summary>
    /// Defines the data access contract for fetching raw flight data from the SerpApi external API.
    /// </summary>
    public interface IFlightRepository
    {
        /// <summary>
        /// Fetches raw flight search results from the SerpApi Google Flights endpoint.
        /// </summary>
        /// <param name="origin">The IATA departure airport code.</param>
        /// <param name="destination">The IATA arrival airport code.</param>
        /// <param name="date">The travel date in <c>YYYY-MM-DD</c> format.</param>
        /// <param name="adults">The number of passengers.</param>
        /// <param name="travelClass">The numeric SerpApi travel class (1=Economy, 2=Premium Economy, 3=Business, 4=First).</param>
        /// <returns>A <see cref="SerpApiFlightResponse"/> with raw flight data, or <c>null</c> if the API call fails.</returns>
        Task<SerpApiFlightResponse?> GetFlightsRawAsync(string origin, string destination, string date, int adults, int travelClass);
    }
}
