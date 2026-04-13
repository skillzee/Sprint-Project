namespace TravelApp.Services.Booking.Models
{
    /// <summary>
    /// Represents a hotel room booking made by a user in the TravelApp system.
    /// </summary>
    public class Booking
    {
        /// <summary>Gets or sets the unique identifier for this booking.</summary>
        public int Id { get; set; }

        /// <summary>Gets or sets the ID of the user who made the booking.</summary>
        public int UserId { get; set; }

        /// <summary>Gets or sets the display name of the user at the time of booking.</summary>
        public string UserName { get; set; } = "";

        /// <summary>Gets or sets the email of the user, used for sending confirmation and cancellation notifications.</summary>
        public string UserEmail { get; set; } = "";

        /// <summary>Gets or sets the ID of the room that was booked.</summary>
        public int RoomId { get; set; }

        /// <summary>Gets or sets the type/category of the booked room (e.g., "Deluxe", "Suite").</summary>
        public string RoomType { get; set; } = "";

        /// <summary>Gets or sets the name of the hotel where the room is located.</summary>
        public string HotelName { get; set; } = "";

        /// <summary>Gets or sets the check-in date for this booking.</summary>
        public DateTime CheckInDate { get; set; }

        /// <summary>Gets or sets the check-out date for this booking.</summary>
        public DateTime CheckOutDate { get; set; }

        /// <summary>Gets or sets the total calculated price for the entire stay.</summary>
        public decimal TotalPrice { get; set; }

        /// <summary>Gets or sets the current status of the booking. Defaults to <c>"Confirmed"</c>; can be <c>"Cancelled"</c>.</summary>
        public string Status { get; set; } = "Confirmed";

        /// <summary>Gets or sets the unique human-readable booking reference (e.g., <c>STY-2026-123456</c>).</summary>
        public string BookingRef { get; set; } = "";

        /// <summary>Gets or sets the local timestamp when this booking record was created.</summary>
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
