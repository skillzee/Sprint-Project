using Microsoft.AspNetCore.Mvc;
using Moq;
using FluentAssertions;
using TravelApp.Services.Auth.Controllers;
using TravelApp.Services.Auth.DTOs;
using TravelApp.Services.Auth.Interfaces;

namespace TravelApp.Services.Auth.Tests.Controllers;

public class AuthControllerTests
{
    private readonly Mock<IAuthService> _authServiceMock;
    private readonly AuthController _controller;

    public AuthControllerTests()
    {
        _authServiceMock = new Mock<IAuthService>();
        _controller = new AuthController(_authServiceMock.Object);
    }

    [Fact]
    public async Task Register_ShouldReturnOk_WhenServiceSucceeds()
    {
        // Arrange
        var dto = new RegisterDto("Test User", "test@example.com", "Password123!", "User");
        var expectedResponse = new AuthResponseDto(1, "Test User", "test@example.com", "User", "fake-jwt-token");

        _authServiceMock.Setup(s => s.RegisterAsync(dto))
            .ReturnsAsync(new RegisterResult.Success(expectedResponse));

        // Act
        var result = await _controller.Register(dto);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var response = okResult.Value.Should().BeOfType<AuthResponseDto>().Subject;
        
        response.Email.Should().Be(dto.Email);
        response.Token.Should().Be("fake-jwt-token");
        _authServiceMock.Verify(s => s.RegisterAsync(dto), Times.Once);
    }

    [Fact]
    public async Task Register_ShouldReturnBadRequest_WhenServiceReturnsNull()
    {
        // Arrange
        var dto = new RegisterDto("New", "duplicate@example.com", "Password123!", "User");
        _authServiceMock.Setup(s => s.RegisterAsync(dto))
            .ReturnsAsync(new RegisterResult.EmailAlreadyExists());

        // Act
        var result = await _controller.Register(dto);

        // Assert
        result.Result.Should().BeOfType<BadRequestObjectResult>();
        _authServiceMock.Verify(s => s.RegisterAsync(dto), Times.Once);
    }

    [Fact]
    public async Task Login_ShouldReturnOk_WhenCredentialsAreValid()
    {
        // Arrange
        var dto = new LoginDto("john@example.com", "Password123!");
        var expectedResponse = new AuthResponseDto(1, "John", "john@example.com", "User", "valid-token");

        _authServiceMock.Setup(s => s.LoginAsync(dto))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _controller.Login(dto);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var response = okResult.Value.Should().BeOfType<AuthResponseDto>().Subject;
        
        response.Token.Should().Be("valid-token");
        _authServiceMock.Verify(s => s.LoginAsync(dto), Times.Once);
    }

    [Fact]
    public async Task Login_ShouldReturnUnauthorized_WhenServiceReturnsNull()
    {
        // Arrange
        var dto = new LoginDto("wrong@example.com", "wrongpass");
        _authServiceMock.Setup(s => s.LoginAsync(dto))
            .ReturnsAsync((AuthResponseDto?)null);

        // Act
        var result = await _controller.Login(dto);

        // Assert
        result.Result.Should().BeOfType<UnauthorizedObjectResult>();
        _authServiceMock.Verify(s => s.LoginAsync(dto), Times.Once);
    }
}
