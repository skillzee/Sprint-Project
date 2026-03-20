using Microsoft.EntityFrameworkCore;
using TravelApp.Services.Hotel.Models;

namespace TravelApp.Services.Hotel.Data
{
    public class HotelDbContext: DbContext
    {
        public HotelDbContext(DbContextOptions<HotelDbContext> options) : base(options) { }


        public DbSet<Models.Hotel> Hotels => Set<Models.Hotel>();
        public DbSet<Room> Rooms => Set<Room>();

        protected override void OnModelCreating(ModelBuilder mb)
        {
            mb.Entity<Models.Hotel>().HasData(
                new Models.Hotel { Id = 1, Name = "The Taj Palace", City = "Delhi", Address = "2 Sardar Patel Marg, New Delhi", Description = "Iconic luxury hotel in the heart of Delhi", StarRating = 5, Amenities = "Pool,Spa,Gym,Restaurant,Bar,WiFi" },
                new Models.Hotel { Id = 2, Name = "Marina Bay Suites", City = "Mumbai", Address = "Marine Lines, Mumbai", Description = "Stunning sea-view suites in South Mumbai", StarRating = 4, Amenities = "Pool,Restaurant,WiFi,Gym" },
                new Models.Hotel { Id = 3, Name = "Spice Garden Resort", City = "Goa", Address = "Calangute Beach, North Goa", Description = "Beachfront resort with lush tropical gardens", StarRating = 4, Amenities = "Pool,Beach Access,Spa,Restaurant,Bar,WiFi" },
                new Models.Hotel { Id = 4, Name = "Himalayan Retreat", City = "Manali", Address = "Old Manali Road, Manali", Description = "Cozy mountain hotel with panoramic Himalayan views", StarRating = 3, Amenities = "Restaurant,WiFi,Bonfire" },
                new Models.Hotel { Id = 5, Name = "Heritage Haveli", City = "Jaipur", Address = "Pink City, Jaipur", Description = "Traditional Rajasthani haveli turned luxury hotel", StarRating = 5, Amenities = "Pool,Spa,Restaurant,Cultural Shows,WiFi" }
            );
            mb.Entity<Models.Room>().HasData(
                new Models.Room { Id = 1, HotelId = 1, Type = "Deluxe King", PricePerNight = 12000, IsAvailable = true, MaxOccupancy = 2, Description = "Spacious room with city view" },
                new Models.Room { Id = 2, HotelId = 1, Type = "Presidential Suite", PricePerNight = 45000, IsAvailable = true, MaxOccupancy = 4, Description = "Top-floor suite with panoramic Delhi views" },
                new Models.Room { Id = 3, HotelId = 2, Type = "Sea View Double", PricePerNight = 8500, IsAvailable = true, MaxOccupancy = 2, Description = "Room with stunning Arabian Sea views" },
                new Models.Room { Id = 4, HotelId = 2, Type = "Standard Twin", PricePerNight = 5500, IsAvailable = true, MaxOccupancy = 2, Description = "Comfortable twin room" },
                new Models.Room { Id = 5, HotelId = 3, Type = "Beach Cottage", PricePerNight = 9000, IsAvailable = true, MaxOccupancy = 2, Description = "Private cottage steps from the beach" },
                new Models.Room { Id = 6, HotelId = 3, Type = "Garden Suite", PricePerNight = 6500, IsAvailable = true, MaxOccupancy = 3, Description = "Suite overlooking tropical gardens" },
                new Models.Room { Id = 7, HotelId = 4, Type = "Mountain View Room", PricePerNight = 3500, IsAvailable = true, MaxOccupancy = 2, Description = "Cozy room with Himalayan vistas" },
                new Models.Room { Id = 8, HotelId = 5, Type = "Royal Suite", PricePerNight = 18000, IsAvailable = true, MaxOccupancy = 2, Description = "Opulent Rajasthani-themed royal suite" }
            );
        }


    }
}
