namespace TravelApp.Shared;

/// <summary>
/// Published by the Booking service when a hotel room booking is successfully confirmed.
/// Consumed by the Notification service to send a confirmation email to the user.
/// </summary>
public class BookingConfirmedEvent
{
    /// <summary>Gets or sets the unique identifier of the confirmed booking.</summary>
    public int BookingId { get; set; }

    /// <summary>Gets or sets the ID of the user who made the booking.</summary>
    public int UserId { get; set; }

    /// <summary>Gets or sets the display name of the user.</summary>
    public string UserName { get; set; } = "";

    /// <summary>Gets or sets the email address of the user for sending the confirmation.</summary>
    public string UserEmail { get; set; } = "";

    /// <summary>Gets or sets the name of the hotel that was booked.</summary>
    public string HotelName { get; set; } = "";

    /// <summary>Gets or sets the type/category of the booked room.</summary>
    public string RoomType { get; set; } = "";

    /// <summary>Gets or sets the check-in date for the booking.</summary>
    public DateTime CheckInDate { get; set; }

    /// <summary>Gets or sets the check-out date for the booking.</summary>
    public DateTime CheckOutDate { get; set; }

    /// <summary>Gets or sets the total price paid for the stay.</summary>
    public decimal TotalPrice { get; set; }

    /// <summary>Gets or sets the unique human-readable booking reference (e.g., <c>STY-2026-123456</c>).</summary>
    public string BookingRef { get; set; } = "";
}

/// <summary>
/// Published by the Booking service when a booking is cancelled.
/// Consumed by the Notification service to send a cancellation email to the user.
/// </summary>
public class BookingCancelledEvent
{
    /// <summary>Gets or sets the unique identifier of the cancelled booking.</summary>
    public int BookingId { get; set; }

    /// <summary>Gets or sets the email address of the user for sending the cancellation notice.</summary>
    public string UserEmail { get; set; } = "";

    /// <summary>Gets or sets the display name of the user.</summary>
    public string UserName { get; set; } = "";

    /// <summary>Gets or sets the name of the hotel associated with the cancelled booking.</summary>
    public string HotelName { get; set; } = "";

    /// <summary>Gets or sets the booking reference of the cancelled booking.</summary>
    public string BookingRef { get; set; } = "";
}

/// <summary>
/// Published by the Hotel service when an admin approves a hotel listing.
/// Consumed by the Notification service to send an approval email to the Hotel Manager.
/// </summary>
public class HotelApprovedEvent
{
    /// <summary>Gets or sets the unique identifier of the approved hotel.</summary>
    public int HotelId { get; set; }

    /// <summary>Gets or sets the name of the approved hotel.</summary>
    public string HotelName { get; set; } = "";

    /// <summary>Gets or sets the email address of the Hotel Manager to notify.</summary>
    public string OwnerEmail { get; set; } = "";

    /// <summary>Gets or sets the display name of the Hotel Manager.</summary>
    public string OwnerName { get; set; } = "";
}

/// <summary>
/// Published by the Hotel service when an admin rejects a hotel listing.
/// Consumed by the Notification service to send a rejection email to the Hotel Manager.
/// </summary>
public class HotelRejectedEvent
{
    /// <summary>Gets or sets the unique identifier of the rejected hotel.</summary>
    public int HotelId { get; set; }

    /// <summary>Gets or sets the name of the rejected hotel.</summary>
    public string HotelName { get; set; } = "";

    /// <summary>Gets or sets the email address of the Hotel Manager to notify.</summary>
    public string OwnerEmail { get; set; } = "";

    /// <summary>Gets or sets the display name of the Hotel Manager.</summary>
    public string OwnerName { get; set; } = "";

    /// <summary>Gets or sets the optional reason provided by the admin for the rejection.</summary>
    public string? Reason { get; set; }
}
