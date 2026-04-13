namespace TravelApp.Services.Flight.DTOs
{
    /// <summary>
    /// Represents a single cleaned and normalized flight offer returned to the client.
    /// This is the public-facing DTO produced by mapping raw SerpApi data.
    /// </summary>
    /// <param name="Id">A unique GUID generated per offer for client-side identification.</param>
    /// <param name="Airline">The name of the operating airline.</param>
    /// <param name="AirlineCode">A 2-letter airline code derived from the airline name.</param>
    /// <param name="FlightNumber">The flight number (e.g., <c>AI-202</c>).</param>
    /// <param name="Origin">The full name of the departure airport.</param>
    /// <param name="OriginIata">The IATA code of the departure airport (e.g., <c>DEL</c>).</param>
    /// <param name="Destination">The full name of the arrival airport.</param>
    /// <param name="DestinationIata">The IATA code of the arrival airport (e.g., <c>BOM</c>).</param>
    /// <param name="DepartureTime">The departure date and time as a string.</param>
    /// <param name="ArrivalTime">The arrival date and time as a string.</param>
    /// <param name="Duration">The total flight duration in ISO 8601 period format (e.g., <c>PT120M</c>).</param>
    /// <param name="Price">The ticket price in INR.</param>
    /// <param name="Currency">The currency of the price (always <c>"INR"</c>).</param>
    /// <param name="SeatsAvailable">An estimated seat availability count (fixed at 9).</param>
    /// <param name="CabinClass">The cabin class as returned by the API (e.g., <c>"economy"</c>).</param>
    /// <param name="IsNonStop"><c>true</c> if the flight has no layovers; otherwise, <c>false</c>.</param>
    /// <param name="BookingUrl">A Google Flights deeplink URL for booking this specific itinerary.</param>
    public record FlightOfferDto(
        string Id,
        string Airline,
        string AirlineCode,
        string FlightNumber,
        string Origin,
        string OriginIata,
        string Destination,
        string DestinationIata,
        string DepartureTime,
        string ArrivalTime,
        string Duration,
        decimal Price,
        string Currency,
        int SeatsAvailable,
        string CabinClass,
        bool IsNonStop,
        string BookingUrl
    );
}
