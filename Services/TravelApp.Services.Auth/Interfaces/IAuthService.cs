using TravelApp.Services.Auth.DTOs;

namespace TravelApp.Services.Auth.Interfaces;

public interface IAuthService
{
    Task<RegisterResult> RegisterAsync(RegisterDto dto);
    Task<AuthResponseDto?> LoginAsync(LoginDto dto);
    Task<AuthResponseDto?> GoogleLoginAsync(string idToken);
}
