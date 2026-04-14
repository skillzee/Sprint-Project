using Microsoft.AspNetCore.Mvc;
using TravelApp.Services.Auth.DTOs;
using TravelApp.Services.Auth.Interfaces;
using TravelApp.Shared.Exceptions;

namespace TravelApp.Services.Auth.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ILogger<AuthController> _logger;

    /// <summary>
    /// Constructor for AuthController which injects authService and logger
    /// </summary>
    /// <param name="authService">Authentication service</param>
    /// <param name="logger">Logger instance</param>
    public AuthController(IAuthService authService, ILogger<AuthController> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    /// <summary>
    /// Registers a new user account
    /// </summary>
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
            _logger.LogError(ex, "Error during user registration for email: {Email}", dto.Email);
            throw;
        }
    }

    /// <summary>
    /// Authenticates a user and returns a JWT token
    /// </summary>
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
            _logger.LogError(ex, "Error during user login for email: {Email}", dto.Email);
            throw;
        }
    }

    /// <summary>
    /// Authenticates a user via Google login and returns a JWT token
    /// </summary>
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
            _logger.LogError(ex, "Error during Google login");
            throw;
        }
    }
}
