using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TravelApp.Services.Trip.Models
{
    public class Trip
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        [Required]
        public string Destination { get; set; } = "";
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public ICollection<Itinerary> Itineraries { get; set; } = new List<Itinerary>();
    }


    public class Itinerary
    {
        public int Id { get; set; }
        public int TripId { get; set; }
        [ForeignKey("TripId")]
        public Trip Trip { get; set; } = null!;
        public int DayNumber { get; set; }
        [Required, Column(TypeName = "nvarchar(max)")]
        public string Activity { get; set; } = "";
        [MaxLength(200)]
        public string Location { get; set; } = "";

    }
}
