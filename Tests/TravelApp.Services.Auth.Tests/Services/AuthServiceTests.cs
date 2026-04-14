using Moq;
using FluentAssertions;
using TravelApp.Services.Auth.DTOs;
using TravelApp.Services.Auth.Interfaces;
using TravelApp.Services.Auth.Models;
using TravelApp.Services.Auth.Services;
using TravelApp.Services.Auth.Helpers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace TravelApp.Services.Auth.Tests.Services;

public class AuthServiceTests
{
    private readonly Mock<IAuthRepository> _authRepoMock;
    private readonly Mock<IConfiguration> _configMock;
    private readonly Mock<ILogger<AuthService>> _loggerMock;
    private readonly JwtHelper _jwtHelper;
    private readonly AuthService _authService;

    public AuthServiceTests()
    {
        _authRepoMock = new Mock<IAuthRepository>();
        
        _configMock = new Mock<IConfiguration>();
        _loggerMock = new Mock<ILogger<AuthService>>();
        var mockSection = new Mock<IConfigurationSection>();
        mockSection.Setup(s => s["SecretKey"]).Returns("ThisIsAStrongSecretKeyForInternalTestsOnly");
        _configMock.Setup(c => c.GetSection("JwtSettings")).Returns(mockSection.Object);
        _jwtHelper = new JwtHelper(_configMock.Object);

        _authService = new AuthService(_authRepoMock.Object, _jwtHelper, _configMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task RegisterAsync_ShouldReturnResponse_WhenUserDoesNotExist()
    {
        // Arrange
        var dto = new RegisterDto("Test User", "test@example.com", "Password123!", "Customer");
        _authRepoMock.Setup(r => r.UserExistsAsync(dto.Email)).ReturnsAsync(false);
        _authRepoMock.Setup(r => r.CreateUserAsync(It.IsAny<User>()))
            .ReturnsAsync(new User { Id = 1, Name = dto.Name, Email = dto.Email, Role = dto.Role });

        // Act
        var result = await _authService.RegisterAsync(dto);

        // Assert
        result.Should().BeOfType<RegisterResult.Success>();
        var success = result as RegisterResult.Success;
        success!.Response.Email.Should().Be(dto.Email);
        success.Response.Token.Should().NotBeNullOrEmpty();
        _authRepoMock.Verify(r => r.CreateUserAsync(It.IsAny<User>()), Times.Once);
    }

    [Fact]
    public async Task RegisterAsync_ShouldReturnNull_WhenUserAlreadyExists()
    {
        // Arrange
        var dto = new RegisterDto("Test User", "test@example.com", "Password123!", "Customer");
        _authRepoMock.Setup(r => r.UserExistsAsync(dto.Email)).ReturnsAsync(true);

        // Act
        var result = await _authService.RegisterAsync(dto);

        // Assert
        result.Should().BeOfType<RegisterResult.EmailAlreadyExists>();
        _authRepoMock.Verify(r => r.CreateUserAsync(It.IsAny<User>()), Times.Never);
    }

    [Fact]
    public async Task LoginAsync_ShouldReturnResponse_WhenCredentialsAreValid()
    {
        // Arrange
        var password = "Password123!";
        var user = new User 
        { 
            Id = 1,
            Name = "John", 
            Email = "john@example.com", 
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(password), 
            Role = "User" 
        };
        var dto = new LoginDto(user.Email, password);

        _authRepoMock.Setup(r => r.GetUserByEmailAsync(dto.Email)).ReturnsAsync(user);

        // Act
        var result = await _authService.LoginAsync(dto);

        // Assert
        result.Should().NotBeNull();
        result!.Token.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task LoginAsync_ShouldReturnNull_WhenPasswordIsIncorrect()
    {
        // Arrange
        var user = new User 
        { 
            Email = "john@example.com", 
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("CorrectPassword"), 
            Role = "User" 
        };
        var dto = new LoginDto(user.Email, "WrongPassword");

        _authRepoMock.Setup(r => r.GetUserByEmailAsync(dto.Email)).ReturnsAsync(user);

        // Act
        var result = await _authService.LoginAsync(dto);

        // Assert
        result.Should().BeNull();
    }
}
