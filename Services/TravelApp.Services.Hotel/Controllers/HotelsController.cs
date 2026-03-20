using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TravelApp.Services.Hotel.Data;
using TravelApp.Services.Hotel.DTOs;
using TravelApp.Services.Hotel.Models;

namespace TravelApp.Services.Hotel.Controllers
{
    [Route("api/hotels")]
    [ApiController]
    public class HotelsController : ControllerBase
    {

        private readonly HotelDbContext _db;
        public HotelsController(HotelDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<HotelDto>>> GetAll([FromQuery] string? city)
        {
            var query = _db.Hotels.Include(t => t.Rooms).AsQueryable();
            if (!string.IsNullOrEmpty(city))
            {
                query = query.Where(t => t.City.ToLower().Contains(city.ToLower()));

            }

            var hotels = await query.ToListAsync();
            return Ok(hotels.Select(ToDto));
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<HotelDto>> Get(int id)
        {
            var hotel = await _db.Hotels.Include(h => h.Rooms).FirstOrDefaultAsync(t => t.Id == id);

            if(hotel == null)
            {
                return NotFound();
            }

            return Ok(ToDto(hotel));
        }


        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Create(CreateHotelDto dto)
        {
            var hotel = new Models.Hotel()
            {
                Name = dto.Name,
                City = dto.City,
                Address = dto.Address,
                Description = dto.Description,
                StarRating = dto.StarRating,
                Amenities = dto.Amenities
            };
            _db.Hotels.Add(hotel);
            await _db.SaveChangesAsync();
            return Ok(ToDto(hotel));


        }

        [HttpPost("{id}/rooms")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> AddRoom(int id, CreateRoomDto dto)
        {
            var hotel = _db.Hotels.FindAsync(id);
            if(hotel == null)
            {
                return NotFound();
            }

            var room = new Room
            {
                HotelId = id,
                Type = dto.Type,
                PricePerNight = dto.PricePerNight,
                MaxOccupancy = dto.MaxOccupancy,
                Description = dto.Description
            };

            _db.Rooms.Add(room);

            await _db.SaveChangesAsync();
            return Ok(ToRoomDto(room));
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Delete(int id)
        {
            var h = await _db.Hotels.FindAsync(id);
            if (h == null) return NotFound();
            _db.Hotels.Remove(h);
            await _db.SaveChangesAsync();
            return Ok(new { message = "Hotel deleted" });
        }



        private static HotelDto ToDto(Models.Hotel h) =>
        new(
            h.Id,
            h.Name,
            h.City,
            h.Address,
            h.Description,
            h.StarRating,
            h.Amenities,
            h.Rooms.Select(ToRoomDto).ToList()
        );

        private static RoomDto ToRoomDto(Room r) =>
        new(
            r.Id,
            r.HotelId,
            r.Type,
            r.PricePerNight,
            r.MaxOccupancy,
            r.IsAvailable,
            r.Description
        );



    }
}
