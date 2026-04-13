using System.Text.Json;
using TravelApp.Services.Booking.DTOs;

namespace TravelApp.Services.Booking.Clients;

/// <summary>
/// HTTP client implementation for communicating with the Hotel microservice.
/// Calls the internal room status endpoint to verify room availability before confirming a booking.
/// Treats any network failure or non-success response as the room being unavailable (fail-safe).
/// </summary>
public class HotelClient : IHotelClient
{
    private readonly HttpClient _http;
    private readonly ILogger<HotelClient> _logger;

    /// <summary>
    /// Initializes a new instance of <see cref="HotelClient"/>.
    /// </summary>
    /// <param name="http">The HTTP client pre-configured with the Hotel service base address.</param>
    /// <param name="logger">The logger instance.</param>
    public HotelClient(HttpClient http, ILogger<HotelClient> logger)
    {
        _http = http;
        _logger = logger;
    }

    /// <summary>
    /// Calls <c>GET /api/hotels/rooms/{roomId}/status</c> on the Hotel service and deserializes the result.
    /// Returns <c>null</c> on any failure, treating the room as unavailable.
    /// </summary>
    /// <param name="roomId">The ID of the room to query.</param>
    /// <returns>A <see cref="RoomStatusDto"/> if the call succeeds; otherwise, <c>null</c>.</returns>
    public async Task<RoomStatusDto?> GetRoomAsync(int roomId)
    {
        try
        {
            // Reuse the existing hotel detail endpoint — fetch room status via hotel rooms list
            var response = await _http.GetAsync($"/api/hotels/rooms/{roomId}/status");
            if (!response.IsSuccessStatusCode)
                return null;

            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<RoomStatusDto>(json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to reach Hotel service for room {RoomId} — treating as unavailable", roomId);
            return null;
        }
    }
}
