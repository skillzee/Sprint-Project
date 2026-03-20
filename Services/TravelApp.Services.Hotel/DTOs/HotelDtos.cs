namespace TravelApp.Services.Hotel.DTOs
{
    public record HotelDto(
     int Id,
     string Name,
     string City,
     string Address,
     string Description,
     double StarRating,
     string Amenities,
     List<RoomDto> Rooms
 );

    public record RoomDto(
        int Id,
        int HotelId,
        string Type,
        decimal PricePerNight,
        int MaxOccupancy,
        bool IsAvailable,
        string Description
    );


    public record CreateHotelDto(
        string Name,
        string City,
        string Address,
        string Description,
        double StarRating,
        string Amenities
    );


    public record CreateRoomDto(
        int HotelId,
        string Type,
        decimal PricePerNight,
        int MaxOccupancy,
        string Description
    );
}
