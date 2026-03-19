namespace TravelApp.Services.Hotel.DTOs
{
    public record HotelDto(
        int Id,
        int Name,
        string City,
        string Address,
        string Description,
        double Rating,
        string ImageUrl
    );

    public record RoomDto(
        int Id,
        int HotelId,
        string HotelName,
        string RoomType,
        decimal PricePerNight,
        int Capacity,
        bool IsAvailable
    );


    public record CreateHotelDto(
        string Name,
        string City,
        string Address,
        string Description,
        double Rating,
        string ImageUrl
    );


    public record CreateRoomDto(
        int HotelId,
        string RoomType,
        decimal PricePerNight,
        int Capacity,
        bool IsAvailable
    );
}
