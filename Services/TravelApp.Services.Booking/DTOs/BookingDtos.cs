namespace TravelApp.Services.Booking.DTOs
{
    public record CreateBookingDto(
        int RoomId,
        string RoomType,
        string HotelName,
        DateTime CheckInDate,
        DateTime CheckOutDate,
        decimal PricePerNight
    );


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
