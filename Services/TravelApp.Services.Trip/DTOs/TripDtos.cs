namespace TravelApp.Services.Trip.DTOs
{
    public record CreateTripDto(
        string Destination,
        DateTime StartDate,
        DateTime EndDate
    );


    public record GenerateItineraryDto(
        int TripId,
        string? Preferences
    );

    public record ItineraryDto(
        int Id,
        int TripId,
        int DayNumber,
        string Activity,
        string Location
    );

    public record TripDto(
    int Id,
    int UserId,
    string Destination,
    DateTime StartDate,
    DateTime EndDate,
    DateTime CreatedAt,
    List<ItineraryDto> Itineraries
);
}
