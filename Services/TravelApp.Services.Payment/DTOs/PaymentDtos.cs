namespace TravelApp.Services.Payment.DTOs
{
    public record CreateCheckoutSessionDto(
        int BookingId,
        decimal Amount,
        string HotelName,
        string RoomType,
        string Currency = "inr"
    );

    public record PaymentDto(
        int Id,
        int BookingId,
        int UserId,
        decimal Amount,
        string Method,
        string Status,
        string TransactionRef,
        string? StripeSessionId,
        DateTime CreatedAt
    );

    public record CheckoutSessionResponseDto(string SessionId, string Url);

}
