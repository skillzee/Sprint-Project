using Microsoft.EntityFrameworkCore;
using TravelApp.Services.Booking.Data;
using TravelApp.Services.Booking.Interfaces;

namespace TravelApp.Services.Booking.Repositories
{
    /// <summary>
    /// Implements data access for <see cref="Models.Booking"/> entities using Entity Framework Core.
    /// </summary>
    public class BookingRepository : IBookingRepository
    {
        private readonly BookingDbContext _db;

        /// <summary>
        /// Initializes a new instance of <see cref="BookingRepository"/>.
        /// </summary>
        /// <param name="db">The EF Core database context for the Booking service.</param>
        public BookingRepository(BookingDbContext db)
        {
            _db = db;
        }

        /// <summary>
        /// Stages a new booking for insertion (not yet persisted until <see cref="SaveChangesAsync"/> is called).
        /// </summary>
        /// <param name="booking">The booking entity to add.</param>
        public async Task AddAsync(Models.Booking booking)
        {
            await _db.Bookings.AddAsync(booking);
        }

        /// <summary>
        /// Retrieves all bookings ordered by creation date, newest first.
        /// </summary>
        /// <returns>All bookings in the system.</returns>
        public async Task<IEnumerable<Models.Booking>> GetAllAsync()
        {
            return await _db.Bookings.OrderByDescending(b => b.CreatedAt).ToListAsync();
        }

        /// <summary>
        /// Finds a booking by its primary key.
        /// </summary>
        /// <param name="id">The booking ID to look up.</param>
        /// <returns>The matching <see cref="Models.Booking"/>, or <c>null</c> if not found.</returns>
        public async Task<Models.Booking?> GetByIdAsync(int id)
        {
            return await _db.Bookings.FindAsync(id);
        }

        /// <summary>
        /// Retrieves all bookings for a given user, ordered newest first.
        /// </summary>
        /// <param name="userId">The user's ID to filter by.</param>
        /// <returns>A collection of that user's bookings.</returns>
        public async Task<IEnumerable<Models.Booking>> GetByUserIdAsync(int userId)
        {
            return await _db.Bookings.Where(t => t.UserId == userId).OrderByDescending(item => item.CreatedAt).ToListAsync();
        }

        /// <summary>
        /// Determines whether the user has an existing confirmed booking for the same room with overlapping dates.
        /// Uses strict overlap logic: existing check-in is before requested check-out AND existing check-out is after requested check-in.
        /// </summary>
        /// <param name="userId">The user's ID.</param>
        /// <param name="roomId">The room's ID.</param>
        /// <param name="checkInDate">The requested check-in date.</param>
        /// <param name="checkOutDate">The requested check-out date.</param>
        /// <returns><c>true</c> if an overlapping confirmed booking exists; otherwise, <c>false</c>.</returns>
        public async Task<bool> HasOverlappingBookingAsync(int userId, int roomId, DateTime checkInDate, DateTime checkOutDate)
        {
            return await _db.Bookings.AnyAsync(b =>
                b.UserId == userId &&
                b.RoomId == roomId &&
                b.Status == "Confirmed" &&
                b.CheckInDate < checkOutDate &&
                b.CheckOutDate > checkInDate);
        }

        /// <summary>
        /// Commits all pending EF Core changes to the database.
        /// </summary>
        public async Task SaveChangesAsync()
        {
            await _db.SaveChangesAsync();
        }
    }
}
