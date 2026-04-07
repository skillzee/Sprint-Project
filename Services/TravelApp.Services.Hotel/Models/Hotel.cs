using System.ComponentModel.DataAnnotations.Schema;

namespace TravelApp.Services.Hotel.Models
{
    public class Hotel
    {

        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string City { get; set; } = "";
        public string Address { get; set; } = "";
        public string Description { get; set; } = "";
        public double StarRating { get; set; }
        public string Amenities { get; set; } = "";
        public List<Room> Rooms { get; set; } = new();



    }



    public class Room
    {
        public int Id { get; set; }
        public int HotelId { get; set; }
        [ForeignKey("HotelId")]
        public Hotel Hotel { get; set; } = null!;
        public string Type { get; set; } = "";
        public decimal PricePerNight {  get; set; }
        public int MaxOccupancy { get; set; }
        public string Description { get; set; } = "";
        public bool IsAvailable { get; set; } = true;

    }
}
