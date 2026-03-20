namespace TravelApp.Services.Flight.DTOs
{
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
        string BookingUrl          // Google Flights deeplink
);
}
