using System.ComponentModel.DataAnnotations;

namespace TravelApp.Services.Auth.Models;

/// <summary>
/// Represents a registered application user stored in the Auth database.
/// </summary>
public class User
{
    /// <summary>Gets or sets the unique identifier for this user.</summary>
    public int Id { get; set; }

    /// <summary>Gets or sets the full display name of the user.</summary>
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    /// <summary>Gets or sets the user's email address, used as the login credential.</summary>
    [Required]
    [EmailAddress]
    [MaxLength(150)]
    public string Email { get; set; } = string.Empty;

    /// <summary>Gets or sets the BCrypt-hashed password for this user. Not used for Google-authenticated accounts.</summary>
    [Required]
    public string PasswordHash { get; set; } = string.Empty;

    /// <summary>Gets or sets the user's role. Defaults to <c>"Customer"</c>. Can be <c>"Customer"</c>, <c>"HotelManager"</c>, or <c>"Admin"</c>.</summary>
    [Required]
    [MaxLength(20)]
    public string Role { get; set; } = "Customer";

    /// <summary>Gets or sets the UTC timestamp when this user registered.</summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
