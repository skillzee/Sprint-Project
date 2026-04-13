using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TravelApp.Services.Hotel.DTOs;
using TravelApp.Services.Hotel.Interfaces;

namespace TravelApp.Services.Hotel.Controllers
{
    [Route("api/hotels")]
    [ApiController]
    public class HotelsController : ControllerBase
    {
        private readonly IHotelService _hotelService;

        public HotelsController(IHotelService hotelService)
        {
            _hotelService = hotelService;
        }

        // ── Public endpoints ──────────────────────────────────────────────────

        /// <summary>
        /// Retrieves all approved hotels (optionally filtered by city)
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<HotelDto>>> GetAll([FromQuery] string? city)
        {
            var result = await _hotelService.GetHotelsAsync(city);
            return Ok(result);
        }

        /// <summary>
        /// Retrieves details for a specific hotel by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<HotelDto>> Get(int id)
        {
            var result = await _hotelService.GetHotelByIdAsync(id);
            if (result == null)
                return NotFound();
            return Ok(result);
        }

        // ── Internal service-to-service endpoint ─────────────────────────────

        /// <summary>
        /// Retrieves the status of a specific room (Internal use)
        /// </summary>
        [HttpGet("rooms/{roomId}/status")]
        public async Task<ActionResult> GetRoomStatus(int roomId)
        {
            var room = await _hotelService.GetRoomStatusAsync(roomId);
            if (room == null)
                return NotFound();
            return Ok(room);
        }

        // ── HotelManager endpoints ────────────────────────────────────────────

        /// <summary>
        /// Creates a new hotel as a Hotel Manager
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "HotelManager")]
        public async Task<ActionResult> Create(CreateHotelDto dto)
        {
            var ownerId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var ownerEmail = User.FindFirstValue(ClaimTypes.Email) ?? "";
            var ownerName = User.FindFirstValue(ClaimTypes.Name) ?? "";

            var result = await _hotelService.CreateHotelAsync(dto, ownerId, ownerEmail, ownerName);
            return Ok(result);
        }

        /// <summary>
        /// Adds a room to an existing hotel owned by the manager
        /// </summary>
        [HttpPost("{id}/rooms")]
        [Authorize(Roles = "HotelManager")]
        public async Task<ActionResult> AddRoom(int id, CreateRoomDto dto)
        {
            var requestingUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var result = await _hotelService.AddRoomToHotelAsync(id, dto, requestingUserId);

            if (result == null)
                return Forbid();

            return Ok(result);
        }

        /// <summary>
        /// Retrieves all hotels owned by the current Hotel Manager
        /// </summary>
        [HttpGet("my")]
        [Authorize(Roles = "HotelManager")]
        public async Task<ActionResult<IEnumerable<HotelDto>>> GetMyHotels()
        {
            var ownerId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var result = await _hotelService.GetHotelsByOwnerAsync(ownerId);
            return Ok(result);
        }

        // ── Admin endpoints ───────────────────────────────────────────────────

        /// <summary>
        /// Retrieves all pending hotels awaiting admin approval
        /// </summary>
        [HttpGet("pending")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<HotelDto>>> GetPending()
        {
            var result = await _hotelService.GetPendingHotelsAsync();
            return Ok(result);
        }

        /// <summary>
        /// Retrieves all pending rooms awaiting admin approval
        /// </summary>
        [HttpGet("rooms/pending")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<RoomDto>>> GetPendingRooms()
        {
            var result = await _hotelService.GetPendingRoomsAsync();
            return Ok(result);
        }

        /// <summary>
        /// Approves a pending hotel
        /// </summary>
        [HttpPut("{id}/approve")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<HotelDto>> ApproveHotel(int id)
        {
            var result = await _hotelService.ApproveHotelAsync(id);
            if (result == null)
                return NotFound();
            return Ok(result);
        }

        /// <summary>
        /// Rejects a pending hotel with a reason
        /// </summary>
        [HttpPut("{id}/reject")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<HotelDto>> RejectHotel(int id, [FromBody] RejectDto dto)
        {
            var result = await _hotelService.RejectHotelAsync(id, dto.Reason);
            if (result == null)
                return NotFound();
            return Ok(result);
        }

        /// <summary>
        /// Approves a pending room within a hotel
        /// </summary>
        [HttpPut("{hotelId}/rooms/{roomId}/approve")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<RoomDto>> ApproveRoom(int hotelId, int roomId)
        {
            var result = await _hotelService.ApproveRoomAsync(hotelId, roomId);
            if (result == null)
                return NotFound();
            return Ok(result);
        }

        /// <summary>
        /// Rejects a pending room within a hotel
        /// </summary>
        [HttpPut("{hotelId}/rooms/{roomId}/reject")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<RoomDto>> RejectRoom(int hotelId, int roomId)
        {
            var result = await _hotelService.RejectRoomAsync(hotelId, roomId);
            if (result == null)
                return NotFound();
            return Ok(result);
        }

        // ── Existing Admin endpoint ───────────────────────────────────────────

        /// <summary>
        /// Deletes a hotel
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Delete(int id)
        {
            var result = await _hotelService.DeleteHotelAsync(id);
            if (!result)
                return BadRequest();
            return Ok();
        }
    }

    public record RejectDto(string? Reason);
}
