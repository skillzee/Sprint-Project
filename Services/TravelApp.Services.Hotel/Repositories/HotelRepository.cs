using Microsoft.EntityFrameworkCore;
using TravelApp.Services.Hotel.Data;
using TravelApp.Services.Hotel.Interfaces;
using TravelApp.Services.Hotel.Models;

namespace TravelApp.Services.Hotel.Repositories
{
    public class HotelRepository : IHotelRepository
    {

        private readonly HotelDbContext _db;

        public HotelRepository(HotelDbContext db)
        {
            _db = db;
        }

        public async Task AddHotelAsync(Models.Hotel hotel)
        {
            await _db.Hotels.AddAsync(hotel);
        }

        public async Task AddRoomAsync(Room room)
        {
            await _db.Rooms.AddAsync(room);
        }

        public async Task DeleteHotelAsync(Models.Hotel hotel)
        {
            _db.Hotels.Remove(hotel);  
        }

        public async Task<IEnumerable<Models.Hotel>> GetAllWithRoomsAsync(string? city)
        {
            var query = _db.Hotels.Include(h => h.Rooms).AsQueryable();

            if (!string.IsNullOrEmpty(city))
            {
                query = query.Where(h => h.City.ToLower().Contains(city.ToLower()));
            }

            return await query.ToListAsync(); 
        }

        public async Task<Models.Hotel?> GetByIdAsync(int id)
        {
            return await _db.Hotels.FindAsync(id);
        }

        public async Task<Models.Hotel?> GetByIdWithRoomsAsync(int id)
        {
            return await _db.Hotels.Include(h => h.Rooms).FirstOrDefaultAsync(h =>  h.Id == id);
        }

        public async Task SaveChangesAsync()
        {
            await _db.SaveChangesAsync();
        }
    }
}
