using System.ComponentModel.DataAnnotations.Schema;

namespace TravelApp.Services.Hotel.Models
{
    /// <summary>
    /// Represents a hotel registered in the TravelApp system by a Hotel Manager.
    /// Hotels go through an admin approval workflow before becoming publicly visible.
    /// </summary>
    public class Hotel
    {
        /// <summary>Gets or sets the unique identifier for this hotel.</summary>
        public int Id { get; set; }

        /// <summary>Gets or sets the display name of the hotel.</summary>
        public string Name { get; set; } = "";

        /// <summary>Gets or sets the city where the hotel is located, used for city-based filtering.</summary>
        public string City { get; set; } = "";

        /// <summary>Gets or sets the full street address of the hotel.</summary>
        public string Address { get; set; } = "";

        /// <summary>Gets or sets a marketing description of the hotel.</summary>
        public string Description { get; set; } = "";

        /// <summary>Gets or sets the star rating of the hotel (e.g., 3.5, 5.0).</summary>
        public double StarRating { get; set; }

        /// <summary>Gets or sets a comma-separated or JSON list of the hotel's amenities.</summary>
        public string Amenities { get; set; } = "";

        /// <summary>Gets or sets the collection of rooms associated with this hotel.</summary>
        public List<Room> Rooms { get; set; } = new();

        /// <summary>Gets or sets the user ID of the Hotel Manager who owns this listing.</summary>
        public int OwnerId { get; set; } = 0;

        /// <summary>Gets or sets the email of the Hotel Manager who owns this listing.</summary>
        public string OwnerEmail { get; set; } = "";

        /// <summary>Gets or sets the name of the Hotel Manager who owns this listing.</summary>
        public string OwnerName { get; set; } = "";

        /// <summary>Gets or sets the approval status. Defaults to <c>"Pending"</c>. Can be <c>"Approved"</c> or <c>"Rejected"</c>.</summary>
        public string ApprovalStatus { get; set; } = "Pending";

        /// <summary>Gets or sets the optional reason provided by an Admin when rejecting this hotel.</summary>
        public string? RejectionReason { get; set; }
    }

    /// <summary>
    /// Represents a bookable hotel room that belongs to a <see cref="Hotel"/>.
    /// Rooms also go through an admin approval workflow before becoming bookable.
    /// </summary>
    public class Room
    {
        /// <summary>Gets or sets the unique identifier for this room.</summary>
        public int Id { get; set; }

        /// <summary>Gets or sets the ID of the hotel this room belongs to.</summary>
        public int HotelId { get; set; }

        /// <summary>Gets or sets the parent hotel navigation property.</summary>
        [ForeignKey("HotelId")]
        public Hotel Hotel { get; set; } = null!;

        /// <summary>Gets or sets the room type (e.g., <c>"Standard"</c>, <c>"Deluxe"</c>, <c>"Suite"</c>).</summary>
        public string Type { get; set; } = "";

        /// <summary>Gets or sets the nightly rate for this room in INR.</summary>
        public decimal PricePerNight { get; set; }

        /// <summary>Gets or sets the maximum number of guests this room can accommodate.</summary>
        public int MaxOccupancy { get; set; }

        /// <summary>Gets or sets a description of the room's features and furnishings.</summary>
        public string Description { get; set; } = "";

        /// <summary>Gets or sets whether this room is currently available for booking. Defaults to <c>true</c>.</summary>
        public bool IsAvailable { get; set; } = true;

        /// <summary>Gets or sets the room's approval status. Defaults to <c>"Pending"</c>. Only <c>"Approved"</c> rooms can be booked.</summary>
        public string ApprovalStatus { get; set; } = "Pending";
    }
}
