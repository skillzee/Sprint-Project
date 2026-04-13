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

        // Retrieves all approved hotels (optionally filtered by city)
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
                return StatusCode(500, new { message = "An error occurred fetching all hotels.", error = ex.Message });
            }
        }

        // Retrieves details for a specific hotel by ID
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
                return StatusCode(500, new { message = "An error occurred fetching hotel details.", error = ex.Message });
            }
        }

        // ── Internal service-to-service endpoint ─────────────────────────────

        // Retrieves the status of a specific room (Internal use)
        [HttpGet("rooms/{roomId}/status")]
        public async Task<ActionResult> GetRoomStatus(int roomId)
        {
            try
            {
                var room = await _hotelService.GetRoomStatusAsync(roomId);
                if (room == null)
                    return NotFound();
                return Ok(room);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred fetching room status.", error = ex.Message });
            }
        }

        // ── HotelManager endpoints ────────────────────────────────────────────

        // Creates a new hotel as a Hotel Manager
        [HttpPost]
        [Authorize(Roles = "HotelManager")]
        public async Task<ActionResult> Create(CreateHotelDto dto)
        {
            try
            {
                var ownerId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
                var ownerEmail = User.FindFirstValue(ClaimTypes.Email) ?? "";
                var ownerName = User.FindFirstValue(ClaimTypes.Name) ?? "";

                var result = await _hotelService.CreateHotelAsync(dto, ownerId, ownerEmail, ownerName);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred creating hotel.", error = ex.Message });
            }
        }

        // Adds a room to an existing hotel owned by the manager
        [HttpPost("{id}/rooms")]
        [Authorize(Roles = "HotelManager")]
        public async Task<ActionResult> AddRoom(int id, CreateRoomDto dto)
        {
            try
            {
                var requestingUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
                var result = await _hotelService.AddRoomToHotelAsync(id, dto, requestingUserId);

                if (result == null)
                    return Forbid();

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred adding room.", error = ex.Message });
            }
        }

        // Retrieves all hotels owned by the current Hotel Manager
        [HttpGet("my")]
        [Authorize(Roles = "HotelManager")]
        public async Task<ActionResult<IEnumerable<HotelDto>>> GetMyHotels()
        {
            try
            {
                var ownerId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
                var result = await _hotelService.GetHotelsByOwnerAsync(ownerId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred fetching your hotels.", error = ex.Message });
            }
        }

        // ── Admin endpoints ───────────────────────────────────────────────────

        // Retrieves all pending hotels awaiting admin approval
        [HttpGet("pending")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<HotelDto>>> GetPending()
        {
            try
            {
                var result = await _hotelService.GetPendingHotelsAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred fetching pending hotels.", error = ex.Message });
            }
        }

        // Retrieves all pending rooms awaiting admin approval
        [HttpGet("rooms/pending")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<RoomDto>>> GetPendingRooms()
        {
            try
            {
                var result = await _hotelService.GetPendingRoomsAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred fetching pending rooms.", error = ex.Message });
            }
        }

        // Approves a pending hotel
        [HttpPut("{id}/approve")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<HotelDto>> ApproveHotel(int id)
        {
            try
            {
                var result = await _hotelService.ApproveHotelAsync(id);
                if (result == null)
                    return NotFound();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred approving hotel.", error = ex.Message });
            }
        }

        // Rejects a pending hotel with a reason
        [HttpPut("{id}/reject")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<HotelDto>> RejectHotel(int id, [FromBody] RejectDto dto)
        {
            try
            {
                var result = await _hotelService.RejectHotelAsync(id, dto.Reason);
                if (result == null)
                    return NotFound();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred rejecting hotel.", error = ex.Message });
            }
        }

        // Approves a pending room within a hotel
        [HttpPut("{hotelId}/rooms/{roomId}/approve")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<RoomDto>> ApproveRoom(int hotelId, int roomId)
        {
            try
            {
                var result = await _hotelService.ApproveRoomAsync(hotelId, roomId);
                if (result == null)
                    return NotFound();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred approving room.", error = ex.Message });
            }
        }

        // Rejects a pending room within a hotel
        [HttpPut("{hotelId}/rooms/{roomId}/reject")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<RoomDto>> RejectRoom(int hotelId, int roomId)
        {
            try
            {
                var result = await _hotelService.RejectRoomAsync(hotelId, roomId);
                if (result == null)
                    return NotFound();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred rejecting room.", error = ex.Message });
            }
        }

        // ── Existing Admin endpoint ───────────────────────────────────────────

        // Deletes a hotel
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
                return StatusCode(500, new { message = "An error occurred deleting hotel.", error = ex.Message });
            }
        }
    }

    public record RejectDto(string? Reason);
}
