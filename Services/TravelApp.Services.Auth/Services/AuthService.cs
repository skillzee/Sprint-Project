using TravelApp.Services.Auth.DTOs;
using TravelApp.Services.Auth.Helpers;
using TravelApp.Services.Auth.Interfaces;
using TravelApp.Services.Auth.Models;

namespace TravelApp.Services.Auth.Services;

public class AuthService : IAuthService
{
    private readonly IAuthRepository _authRepo;
    private readonly JwtHelper _jwtHelper;

    public AuthService(IAuthRepository authRepo, JwtHelper jwtHelper)
    {
        _authRepo = authRepo;
        _jwtHelper = jwtHelper;
    }

    public async Task<AuthResponseDto?> RegisterAsync(RegisterDto dto)
    {
        // 1. Business Check: Does user exist?
        if (await _authRepo.UserExistsAsync(dto.Email))
        {
            return null; // For this demo, returning null indicates failure (e.g., email exists)
        }

        // 2. Business Logic: Hash password
        var user = new User
        {
            Name = dto.Name,
            Email = dto.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            Role = dto.Role
        };

        // 3. Data Access
        await _authRepo.CreateUserAsync(user);

        // 4. Token Generation
        var token = _jwtHelper.GenerateToken(user);

        return new AuthResponseDto(user.Id, user.Name, user.Email, user.Role, token);
    }

    public async Task<AuthResponseDto?> LoginAsync(LoginDto dto)
    {
        // 1. Data Access
        var user = await _authRepo.GetUserByEmailAsync(dto.Email);

        // 2. Business Logic: Verify Password
        if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
        {
            return null;
        }

        // 3. Token Generation
        var token = _jwtHelper.GenerateToken(user);

        return new AuthResponseDto(user.Id, user.Name, user.Email, user.Role, token);
    }
}
