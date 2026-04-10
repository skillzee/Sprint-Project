using System.Text.Json;
using TravelApp.Services.Booking.DTOs;

namespace TravelApp.Services.Booking.Clients;

public class HotelClient : IHotelClient
{
    private readonly HttpClient _http;
    private readonly ILogger<HotelClient> _logger;

    public HotelClient(HttpClient http, ILogger<HotelClient> logger)
    {
        _http = http;
        _logger = logger;
    }

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
