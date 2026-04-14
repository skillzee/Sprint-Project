using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TravelApp.Services.Hotel.DTOs;
using TravelApp.Services.Hotel.Interfaces;
using TravelApp.Shared.Exceptions;
using Microsoft.Extensions.Logging;

namespace TravelApp.Services.Hotel.Controllers
{
    [Route("api/hotels")]
    [ApiController]
    public class HotelsController : ControllerBase
    {
        private readonly IHotelService _hotelService;
        private readonly ILogger<HotelsController> _logger;

        public HotelsController(IHotelService hotelService, ILogger<HotelsController> logger)
        {
            _hotelService = hotelService;
            _logger = logger;
        }

        // ── Public endpoints ──────────────────────────────────────────────────

        /// <summary>
        /// Retrieves all approved hotels (optionally filtered by city)
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<HotelDto>>> GetAll([FromQuery] string? city)
        {
            try
            {
                var result = await _hotelService.GetHotelsAsync(city);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving hotels for city: {City}", city);
                throw;
            }
        }

        /// <summary>
        /// Retrieves details for a specific hotel by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<HotelDto>> Get(int id)
        {
            try
            {
                var result = await _hotelService.GetHotelByIdAsync(id);
                if (result == null)
                    return NotFound();
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving hotel with id: {Id}", id);
                throw;
            }
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
            try
            {
                var ownerId = GetUserId();
                var ownerEmail = User.FindFirstValue(ClaimTypes.Email) ?? "";
                var ownerName = User.FindFirstValue(ClaimTypes.Name) ?? "";

                var result = await _hotelService.CreateHotelAsync(dto, ownerId, ownerEmail, ownerName);
                return Ok(result);
            }
            catch (UnauthorizedException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating hotel");
                throw;
            }
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

        /// <summary>
        /// Deletes a hotel
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                var result = await _hotelService.DeleteHotelAsync(id);
                if (!result)
                    return BadRequest();
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting hotel with id: {Id}", id);
                throw;
            }
        }

        private int GetUserId()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userIdClaim))
                throw new UnauthorizedException("User ID not found in token");

            if (!int.TryParse(userIdClaim, out int userId))
                throw new UnauthorizedException("Invalid User ID format in token");

            return userId;
        }
    }

    public record RejectDto(string? Reason);
}
