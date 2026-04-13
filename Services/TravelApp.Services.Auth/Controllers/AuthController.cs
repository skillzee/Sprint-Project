using Microsoft.AspNetCore.Mvc;
using TravelApp.Services.Auth.DTOs;
using TravelApp.Services.Auth.Interfaces;

namespace TravelApp.Services.Auth.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    /// <summary>
    /// Constructor for AuthController which injects authService
    /// </summary>
    /// <param name="authService"></param>
    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    /// <summary>
    /// Registers a new user account
    /// </summary>
    [HttpPost("register")]
    public async Task<ActionResult<AuthResponseDto>> Register(RegisterDto dto)
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

    /// <summary>
    /// Authenticates a user and returns a JWT token
    /// </summary>
    [HttpPost("login")]
    public async Task<ActionResult<AuthResponseDto>> Login(LoginDto dto)
    {
        var result = await _authService.LoginAsync(dto);
        if (result == null)
            return Unauthorized(new { message = "Invalid email or password." });

        return Ok(result);
    }

    /// <summary>
    /// Authenticates a user via Google login and returns a JWT token
    /// </summary>
    [HttpPost("google-login")]
    public async Task<ActionResult<AuthResponseDto>> GoogleLogin(GoogleLoginDto dto)
    {
        var result = await _authService.GoogleLoginAsync(dto.IdToken);
        if (result == null)
            return Unauthorized(new { message = "Invalid Google token." });

        return Ok(result);
    }
}
