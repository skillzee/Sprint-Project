namespace TravelApp.Services.Trip.DTOs
{
    /// <summary>
    /// Payload for creating a new trip.
    /// </summary>
    /// <param name="Destination">The destination city or country for the trip.</param>
    /// <param name="StartDate">The planned start date of the trip.</param>
    /// <param name="EndDate">The planned end date of the trip. Must be after <paramref name="StartDate"/>.</param>
    public record CreateTripDto(
        string Destination,
        DateTime StartDate,
        DateTime EndDate
    );

    /// <summary>
    /// Payload for requesting an AI-generated itinerary for an existing trip.
    /// </summary>
    /// <param name="TripId">The ID of the trip to generate an itinerary for.</param>
    /// <param name="Preferences">Optional user preferences to guide the AI (e.g., "adventure", "food", "budget").</param>
    public record GenerateItineraryDto(
        int TripId,
        string? Preferences
    );

    /// <summary>
    /// Represents a single day's planned activity within a trip.
    /// </summary>
    /// <param name="Id">The unique identifier for this itinerary item.</param>
    /// <param name="TripId">The ID of the parent trip.</param>
    /// <param name="DayNumber">The day number within the trip (e.g., 1, 2, 3).</param>
    /// <param name="Activity">A description of the planned activity for this day.</param>
    /// <param name="Location">The location or venue for this day's activity.</param>
    public record ItineraryDto(
        int Id,
        int TripId,
        int DayNumber,
        string Activity,
        string Location
    );

    /// <summary>
    /// Represents a trip returned to the client, including its AI-generated itinerary items.
    /// </summary>
    /// <param name="Id">The unique trip identifier.</param>
    /// <param name="UserId">The ID of the user who owns this trip.</param>
    /// <param name="Destination">The destination city or country.</param>
    /// <param name="StartDate">The planned start date.</param>
    /// <param name="EndDate">The planned end date.</param>
    /// <param name="CreatedAt">The UTC timestamp when the trip was created.</param>
    /// <param name="Itineraries">The ordered list of day-by-day itinerary items.</param>
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
