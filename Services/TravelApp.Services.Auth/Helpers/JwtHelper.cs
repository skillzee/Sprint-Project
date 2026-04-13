using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using TravelApp.Services.Auth.Models;

namespace TravelApp.Services.Auth.Helpers;

/// <summary>
/// Provides JWT token generation functionality for authenticated users.
/// </summary>
public class JwtHelper
{
    private readonly IConfiguration _config;

    /// <summary>
    /// Initializes a new instance of <see cref="JwtHelper"/> with the application configuration.
    /// </summary>
    /// <param name="config">The application configuration used to read JWT settings.</param>
    public JwtHelper(IConfiguration config)
    {
        _config = config;
    }

    /// <summary>
    /// Generates a signed JWT token for the given user, including their ID, name, email, and role as claims.
    /// The token is valid for 2 hours from the time of generation.
    /// </summary>
    /// <param name="user">The authenticated <see cref="User"/> for whom to generate the token.</param>
    /// <returns>A signed JWT token string.</returns>
    public string GenerateToken(User user)
    {
        var jwtSettings = _config.GetSection("JwtSettings");
        var key = Encoding.ASCII.GetBytes(jwtSettings["SecretKey"]!);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role)
            }),
            Expires = DateTime.UtcNow.AddHours(2),
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature)
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }
}
