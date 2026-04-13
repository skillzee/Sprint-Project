using TravelApp.Services.Auth.DTOs;

namespace TravelApp.Services.Auth.Interfaces;

/// <summary>
/// Defines the contract for authentication business logic operations.
/// </summary>
public interface IAuthService
{
    /// <summary>
    /// Registers a new user account with the provided details.
    /// </summary>
    /// <param name="dto">The registration payload containing name, email, password, and role.</param>
    /// <returns>A <see cref="RegisterResult"/> discriminated union indicating success, duplicate email, or a forbidden role.</returns>
    Task<RegisterResult> RegisterAsync(RegisterDto dto);

    /// <summary>
    /// Authenticates a user using email and password credentials.
    /// </summary>
    /// <param name="dto">The login payload containing email and password.</param>
    /// <returns>An <see cref="AuthResponseDto"/> with a JWT token, or <c>null</c> if credentials are invalid.</returns>
    Task<AuthResponseDto?> LoginAsync(LoginDto dto);

    /// <summary>
    /// Authenticates a user via a Google OAuth ID token, creating an account if one does not exist.
    /// </summary>
    /// <param name="idToken">The Google-issued ID token from the client.</param>
    /// <returns>An <see cref="AuthResponseDto"/> with a JWT token, or <c>null</c> if validation fails.</returns>
    Task<AuthResponseDto?> GoogleLoginAsync(string idToken);
}
