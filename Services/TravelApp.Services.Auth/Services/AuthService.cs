using Google.Apis.Auth;
using Microsoft.Extensions.Configuration;
using TravelApp.Services.Auth.DTOs;
using TravelApp.Services.Auth.Helpers;
using TravelApp.Services.Auth.Interfaces;
using TravelApp.Services.Auth.Models;

namespace TravelApp.Services.Auth.Services;

/// <summary>
/// Implements the authentication business logic including registration, login, and Google OAuth sign-in.
/// </summary>
public class AuthService : IAuthService
{
    private static readonly HashSet<string> AllowedSelfAssignRoles = ["Customer", "HotelManager"];

    private readonly IAuthRepository _authRepo;
    private readonly JwtHelper _jwtHelper;
    private readonly IConfiguration _config;

    /// <summary>
    /// Initializes a new instance of <see cref="AuthService"/>.
    /// </summary>
    /// <param name="authRepo">The repository for user data access.</param>
    /// <param name="jwtHelper">The helper for generating JWT tokens.</param>
    /// <param name="config">The application configuration for Google settings.</param>
    public AuthService(IAuthRepository authRepo, JwtHelper jwtHelper, IConfiguration config)
    {
        _authRepo = authRepo;
        _jwtHelper = jwtHelper;
        _config = config;
    }

    /// <summary>
    /// Registers a new user. Only <c>Customer</c> and <c>HotelManager</c> roles may self-register.
    /// Returns a discriminated union result for clean, exception-free error handling.
    /// </summary>
    /// <param name="dto">The registration data including name, email, password, and optional role.</param>
    /// <returns>
    /// A <see cref="RegisterResult.Success"/> with a JWT token,
    /// <see cref="RegisterResult.EmailAlreadyExists"/> if the email is taken,
    /// or <see cref="RegisterResult.RoleForbidden"/> if the role is not self-assignable.
    /// </returns>
    public async Task<RegisterResult> RegisterAsync(RegisterDto dto)
    {
        // 0. Role allowlist guard
        if (!AllowedSelfAssignRoles.Contains(dto.Role))
            return new RegisterResult.RoleForbidden(dto.Role);

        // 1. Business Check: Does user exist?
        if (await _authRepo.UserExistsAsync(dto.Email))
        {
            return new RegisterResult.EmailAlreadyExists();
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

        return new RegisterResult.Success(new AuthResponseDto(user.Id, user.Name, user.Email, user.Role, token));
    }

    /// <summary>
    /// Authenticates a user by verifying their email/password combination and returns a JWT token.
    /// </summary>
    /// <param name="dto">The login credentials.</param>
    /// <returns>An <see cref="AuthResponseDto"/> with a JWT, or <c>null</c> if credentials are invalid.</returns>
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

    /// <summary>
    /// Validates a Google ID token and logs in or auto-registers the corresponding user.
    /// </summary>
    /// <param name="idToken">The Google-issued ID token to validate.</param>
    /// <returns>An <see cref="AuthResponseDto"/> with a JWT, or <c>null</c> if the Google token is invalid.</returns>
    public async Task<AuthResponseDto?> GoogleLoginAsync(string idToken)
    {
        try
        {
            var clientId = _config["GoogleSettings:ClientId"];
            var settings = new GoogleJsonWebSignature.ValidationSettings()
            {
                Audience = new List<string>() { clientId! }
            };

            var payload = await GoogleJsonWebSignature.ValidateAsync(idToken, settings);

            var user = await _authRepo.GetUserByEmailAsync(payload.Email);

            if (user == null)
            {
                // Create new user if they don't exist
                user = new User
                {
                    Name = payload.Name,
                    Email = payload.Email,
                    PasswordHash = "GOOGLE_USER_" + Guid.NewGuid().ToString(), // Placeholder, not used for login
                    Role = "Customer"
                };
                await _authRepo.CreateUserAsync(user);
            }

            var token = _jwtHelper.GenerateToken(user);
            return new AuthResponseDto(user.Id, user.Name, user.Email, user.Role, token);
        }
        catch (Exception)
        {
            return null;
        }
    }
}
