namespace TravelApp.Shared;

public class BookingConfirmedEvent
{
    public int BookingId { get; set; }
    public int UserId { get; set; }
    public string UserName { get; set; } = "";
    public string UserEmail { get; set; } = "";
    public string HotelName { get; set; } = "";
    public string RoomType { get; set; } = "";
    public DateTime CheckInDate { get; set; }
    public DateTime CheckOutDate { get; set; }
    public decimal TotalPrice { get; set; }
    public string BookingRef { get; set; } = "";
}

public class BookingCancelledEvent
{
    public int BookingId { get; set; }
    public string UserEmail { get; set; } = "";
    public string UserName { get; set; } = "";
    public string HotelName { get; set; } = "";
    public string BookingRef { get; set; } = "";
}

