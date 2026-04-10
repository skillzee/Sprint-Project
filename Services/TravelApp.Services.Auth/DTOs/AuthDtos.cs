using System.ComponentModel.DataAnnotations;

namespace TravelApp.Services.Auth.DTOs;

public record RegisterDto(
    [Required] string Name,
    [Required][EmailAddress] string Email,
    [Required][MinLength(6)] string Password,
    string Role = "Customer"
);

public record LoginDto(
    [Required][EmailAddress] string Email,
    [Required] string Password
);

public record AuthResponseDto(
    int Id,
    string Name,
    string Email,
    string Role,
    string Token
);

/// <summary>Discriminated union for RegisterAsync outcomes.</summary>
public abstract record RegisterResult
{
    public sealed record Success(AuthResponseDto Response) : RegisterResult;
    public sealed record EmailAlreadyExists() : RegisterResult;
    public sealed record RoleForbidden(string Role) : RegisterResult;
}
