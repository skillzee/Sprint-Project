namespace TravelApp.Services.Booking.DTOs
{
    /// <summary>
    /// Payload for creating a new hotel room booking.
    /// </summary>
    /// <param name="RoomId">The ID of the room to book.</param>
    /// <param name="RoomType">The type/category of the room (e.g., "Deluxe", "Suite").</param>
    /// <param name="HotelName">The name of the hotel where the room is located.</param>
    /// <param name="CheckInDate">The desired check-in date.</param>
    /// <param name="CheckOutDate">The desired check-out date.</param>
    /// <param name="PricePerNight">The nightly rate used to calculate the total price.</param>
    public record CreateBookingDto(
        int RoomId,
        string RoomType,
        string HotelName,
        DateTime CheckInDate,
        DateTime CheckOutDate,
        decimal PricePerNight
    );

    /// <summary>
    /// Represents a booking returned to the client.
    /// </summary>
    /// <param name="Id">The unique booking identifier.</param>
    /// <param name="UserId">The ID of the user who made the booking.</param>
    /// <param name="UserName">The display name of the user at the time of booking.</param>
    /// <param name="RoomId">The ID of the booked room.</param>
    /// <param name="RoomType">The type/category of the booked room.</param>
    /// <param name="HotelName">The name of the hotel.</param>
    /// <param name="CheckInDate">The check-in date.</param>
    /// <param name="CheckOutDate">The check-out date.</param>
    /// <param name="TotalPrice">The total calculated price for the stay.</param>
    /// <param name="Status">The current booking status (<c>"Confirmed"</c> or <c>"Cancelled"</c>).</param>
    /// <param name="BookingRef">The unique human-readable booking reference (e.g., <c>STY-2026-123456</c>).</param>
    /// <param name="CreatedAt">The timestamp when the booking was created.</param>
    public record BookingDto(
        int Id,
        int UserId,
        string UserName,
        int RoomId,
        string RoomType,
        string HotelName,
        DateTime CheckInDate,
        DateTime CheckOutDate,
        decimal TotalPrice,
        string Status,
        string BookingRef,
        DateTime CreatedAt
    );
}
