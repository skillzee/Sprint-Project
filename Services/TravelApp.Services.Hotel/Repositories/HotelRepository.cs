using Microsoft.EntityFrameworkCore;
using TravelApp.Services.Hotel.Data;
using TravelApp.Services.Hotel.Interfaces;
using TravelApp.Services.Hotel.Models;

namespace TravelApp.Services.Hotel.Repositories
{
    /// <summary>
    /// Implements data access for <see cref="Models.Hotel"/> and <see cref="Room"/> entities
    /// using Entity Framework Core against the Hotel SQL database.
    /// </summary>
    public class HotelRepository : IHotelRepository
    {
        private readonly HotelDbContext _db;

        /// <summary>
        /// Initializes a new instance of <see cref="HotelRepository"/>.
        /// </summary>
        /// <param name="db">The EF Core database context for the Hotel service.</param>
        public HotelRepository(HotelDbContext db)
        {
            _db = db;
        }

        /// <summary>
        /// Stages a new hotel for insertion (call <see cref="SaveChangesAsync"/> to persist).
        /// </summary>
        /// <param name="hotel">The hotel entity to add.</param>
        public async Task AddHotelAsync(Models.Hotel hotel)
        {
            await _db.Hotels.AddAsync(hotel);
        }

        /// <summary>
        /// Stages a new room for insertion (call <see cref="SaveChangesAsync"/> to persist).
        /// </summary>
        /// <param name="room">The room entity to add.</param>
        public async Task AddRoomAsync(Room room)
        {
            await _db.Rooms.AddAsync(room);
        }

        /// <summary>
        /// Removes a hotel entity from the database (call <see cref="SaveChangesAsync"/> to persist).
        /// </summary>
        /// <param name="hotel">The hotel entity to delete.</param>
        public async Task DeleteHotelAsync(Models.Hotel hotel)
        {
            _db.Hotels.Remove(hotel);
        }

        /// <summary>
        /// Retrieves all hotels with their rooms eagerly loaded, optionally filtered by city (case-insensitive contains match).
        /// </summary>
        /// <param name="city">Optional city name filter.</param>
        /// <returns>A collection of <see cref="Models.Hotel"/> entities with rooms.</returns>
        public async Task<IEnumerable<Models.Hotel>> GetAllWithRoomsAsync(string? city)
        {
            var query = _db.Hotels.Include(h => h.Rooms).AsQueryable();

            if (!string.IsNullOrEmpty(city))
            {
                query = query.Where(h => h.City.ToLower().Contains(city.ToLower()));
            }

            return await query.ToListAsync();
        }

        /// <summary>
        /// Retrieves a hotel by its primary key without loading rooms.
        /// </summary>
        /// <param name="id">The hotel's primary key.</param>
        /// <returns>The <see cref="Models.Hotel"/>, or <c>null</c> if not found.</returns>
        public async Task<Models.Hotel?> GetByIdAsync(int id)
        {
            return await _db.Hotels.FindAsync(id);
        }

        /// <summary>
        /// Retrieves a hotel by its primary key with all rooms eagerly loaded.
        /// </summary>
        /// <param name="id">The hotel's primary key.</param>
        /// <returns>The <see cref="Models.Hotel"/> with rooms, or <c>null</c> if not found.</returns>
        public async Task<Models.Hotel?> GetByIdWithRoomsAsync(int id)
        {
            return await _db.Hotels.Include(h => h.Rooms).FirstOrDefaultAsync(h => h.Id == id);
        }

        /// <summary>
        /// Commits all pending EF Core changes to the database.
        /// </summary>
        public async Task SaveChangesAsync()
        {
            await _db.SaveChangesAsync();
        }

        /// <summary>
        /// Retrieves all hotels with an <c>ApprovalStatus</c> of <c>"Pending"</c>, with rooms eagerly loaded.
        /// </summary>
        /// <returns>A collection of pending <see cref="Models.Hotel"/> entities.</returns>
        public async Task<IEnumerable<Models.Hotel>> GetPendingHotelsAsync()
        {
            return await _db.Hotels
                .Include(h => h.Rooms)
                .Where(h => h.ApprovalStatus == "Pending")
                .ToListAsync();
        }

        /// <summary>
        /// Retrieves all rooms with an <c>ApprovalStatus</c> of <c>"Pending"</c>, with their parent hotel eagerly loaded.
        /// </summary>
        /// <returns>A collection of pending <see cref="Room"/> entities.</returns>
        public async Task<IEnumerable<Room>> GetPendingRoomsAsync()
        {
            return await _db.Rooms
                .Include(r => r.Hotel)
                .Where(r => r.ApprovalStatus == "Pending")
                .ToListAsync();
        }

        /// <summary>
        /// Retrieves all hotels owned by a specific manager, with rooms eagerly loaded.
        /// </summary>
        /// <param name="ownerId">The owner's user ID.</param>
        /// <returns>A collection of <see cref="Models.Hotel"/> entities owned by that user.</returns>
        public async Task<IEnumerable<Models.Hotel>> GetHotelsByOwnerAsync(int ownerId)
        {
            return await _db.Hotels
                .Include(h => h.Rooms)
                .Where(h => h.OwnerId == ownerId)
                .ToListAsync();
        }

        /// <summary>
        /// Retrieves a single room by its unique ID.
        /// </summary>
        /// <param name="roomId">The room's primary key.</param>
        /// <returns>The <see cref="Room"/> entity, or <c>null</c> if not found.</returns>
        public async Task<Room?> GetRoomByIdAsync(int roomId)
        {
            return await _db.Rooms.FindAsync(roomId);
        }
    }
}
