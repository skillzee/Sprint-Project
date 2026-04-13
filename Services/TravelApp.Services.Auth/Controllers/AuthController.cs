using Microsoft.AspNetCore.Mvc;
using TravelApp.Services.Auth.DTOs;
using TravelApp.Services.Auth.Interfaces;

namespace TravelApp.Services.Auth.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    // Registers a new user account
    [HttpPost("register")]
    public async Task<ActionResult<AuthResponseDto>> Register(RegisterDto dto)
    {
        try
        {
            var result = await _authService.RegisterAsync(dto);
            return result switch
            {
                RegisterResult.Success s => Ok(s.Response),
                RegisterResult.RoleForbidden f => BadRequest(new { message = $"Role '{f.Role}' cannot be self-assigned." }),
                RegisterResult.EmailAlreadyExists => BadRequest(new { message = "Registration failed. Email might already exist." }),
                _ => BadRequest(new { message = "Registration failed." })
            };
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while registering the user.", error = ex.Message });
        }
    }

    // Authenticates a user and returns a JWT token
    [HttpPost("login")]
    public async Task<ActionResult<AuthResponseDto>> Login(LoginDto dto)
    {
        try
        {
            var result = await _authService.LoginAsync(dto);
            if (result == null)
                return Unauthorized(new { message = "Invalid email or password." });

            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred during login.", error = ex.Message });
        }
    }

    // Authenticates a user via Google login and returns a JWT token
    [HttpPost("google-login")]
    public async Task<ActionResult<AuthResponseDto>> GoogleLogin(GoogleLoginDto dto)
    {
        try
        {
            var result = await _authService.GoogleLoginAsync(dto.IdToken);
            if (result == null)
                return Unauthorized(new { message = "Invalid Google token." });

            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred during Google login.", error = ex.Message });
        }
    }
}
