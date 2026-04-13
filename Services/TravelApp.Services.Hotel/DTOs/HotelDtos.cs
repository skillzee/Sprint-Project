namespace TravelApp.Services.Hotel.DTOs
{
    /// <summary>
    /// Represents a hotel returned to the client, including its rooms.
    /// </summary>
    /// <param name="Id">The unique hotel identifier.</param>
    /// <param name="Name">The display name of the hotel.</param>
    /// <param name="City">The city where the hotel is located.</param>
    /// <param name="Address">The full street address.</param>
    /// <param name="Description">A marketing description of the hotel.</param>
    /// <param name="StarRating">The star rating (e.g., 3.5, 5.0).</param>
    /// <param name="Amenities">A comma-separated or JSON list of amenities.</param>
    /// <param name="Rooms">The list of rooms associated with this hotel.</param>
    /// <param name="OwnerId">The user ID of the Hotel Manager who owns this listing.</param>
    /// <param name="OwnerEmail">The email of the Hotel Manager.</param>
    /// <param name="OwnerName">The name of the Hotel Manager.</param>
    /// <param name="ApprovalStatus">The current approval status (<c>"Pending"</c>, <c>"Approved"</c>, or <c>"Rejected"</c>).</param>
    public record HotelDto(
     int Id,
     string Name,
     string City,
     string Address,
     string Description,
     double StarRating,
     string Amenities,
     List<RoomDto> Rooms,
     int OwnerId,
     string OwnerEmail,
     string OwnerName,
     string ApprovalStatus
 );

    /// <summary>
    /// Represents a hotel room returned to the client.
    /// </summary>
    /// <param name="Id">The unique room identifier.</param>
    /// <param name="HotelId">The ID of the hotel this room belongs to.</param>
    /// <param name="Type">The room type (e.g., <c>"Standard"</c>, <c>"Deluxe"</c>, <c>"Suite"</c>).</param>
    /// <param name="PricePerNight">The nightly rate in INR.</param>
    /// <param name="MaxOccupancy">The maximum number of guests.</param>
    /// <param name="IsAvailable">Whether the room is currently available for booking.</param>
    /// <param name="Description">A description of the room's features.</param>
    /// <param name="ApprovalStatus">The room's approval status (<c>"Pending"</c>, <c>"Approved"</c>, or <c>"Rejected"</c>).</param>
    public record RoomDto(
        int Id,
        int HotelId,
        string Type,
        decimal PricePerNight,
        int MaxOccupancy,
        bool IsAvailable,
        string Description,
        string ApprovalStatus
    );

    /// <summary>
    /// Payload for creating a new hotel listing.
    /// </summary>
    /// <param name="Name">The hotel's display name.</param>
    /// <param name="City">The city where the hotel is located.</param>
    /// <param name="Address">The full street address.</param>
    /// <param name="Description">A marketing description.</param>
    /// <param name="StarRating">The star rating.</param>
    /// <param name="Amenities">A comma-separated or JSON list of amenities.</param>
    public record CreateHotelDto(
        string Name,
        string City,
        string Address,
        string Description,
        double StarRating,
        string Amenities
    );

    /// <summary>
    /// Payload for adding a new room to an existing hotel.
    /// </summary>
    /// <param name="HotelId">The ID of the hotel to add the room to.</param>
    /// <param name="Type">The room type (e.g., <c>"Standard"</c>, <c>"Deluxe"</c>).</param>
    /// <param name="PricePerNight">The nightly rate in INR.</param>
    /// <param name="MaxOccupancy">The maximum number of guests.</param>
    /// <param name="Description">A description of the room's features.</param>
    public record CreateRoomDto(
        int HotelId,
        string Type,
        decimal PricePerNight,
        int MaxOccupancy,
        string Description
    );
}
