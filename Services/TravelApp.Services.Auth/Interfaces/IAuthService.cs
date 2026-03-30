using TravelApp.Services.Auth.DTOs;

namespace TravelApp.Services.Auth.Interfaces;

public interface IAuthService
{
    Task<AuthResponseDto?> RegisterAsync(RegisterDto dto);
    Task<AuthResponseDto?> LoginAsync(LoginDto dto);
}
