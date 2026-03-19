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
        public double Rating { get; set; }
        public string ImageUrl { get; set; } = "";
        public ICollection<Room> Rooms = new List<Room>();



    }



    public class Room
    {
        public int Id { get; set; }
        public int HotelId { get; set; }
        [ForeignKey("HotelId")]
        public Hotel Hotel { get; set; } = null!;
        public string RoomType { get; set; } = "";
        public decimal PricePerNight {  get; set; }
        public int capacity { get; set; }
        public bool IsAvailable { get; set; } = true;

    }
}
