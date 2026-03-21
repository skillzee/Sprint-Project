namespace TravelApp.Services.Payment.Models
{
    public class Payment
    {

        public int Id { get; set; }
        public int BookingId { get; set; }
        public int UserId { get; set; }
        public string UserEmail { get; set; } = "";
        public decimal Amount { get; set; }
        public string Method { get; set; } = "Stripe";
        public string Status { get; set; } = "Pending";
        public string TransactionRef { get; set; } = "";
        public string? StripeSessionId { get; set; }
        public string? StripePaymentIntentId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    }
}
