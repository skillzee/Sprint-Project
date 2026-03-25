using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TravelApp.Services.Auth.Data;
using TravelApp.Services.Auth.DTOs;
using TravelApp.Services.Auth.Helpers;
using TravelApp.Services.Auth.Models;

namespace TravelApp.Services.Auth.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly AuthDbContext _db;
    private readonly JwtHelper _jwtHelper;

    public AuthController(AuthDbContext db, JwtHelper jwtHelper)
    {
        _db = db;
        _jwtHelper = jwtHelper;
    }

    [HttpPost("register")]
    public async Task<ActionResult<AuthResponseDto>> Register(RegisterDto dto)
    {
        if (await _db.Users.AnyAsync(u => u.Email == dto.Email)){
            return BadRequest(new { message = "Email already exists." });
        }
        var user = new User
        {
            Name = dto.Name,
            Email = dto.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            Role = dto.Role
        };

        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        var token = _jwtHelper.GenerateToken(user);

        return Ok(new AuthResponseDto(user.Id, user.Name, user.Email, user.Role, token));
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponseDto>> Login(LoginDto dto)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
        if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
            return Unauthorized(new { message = "Invalid email or password." });

        var token = _jwtHelper.GenerateToken(user);

        return Ok(new AuthResponseDto(user.Id, user.Name, user.Email, user.Role, token));
    }
}
