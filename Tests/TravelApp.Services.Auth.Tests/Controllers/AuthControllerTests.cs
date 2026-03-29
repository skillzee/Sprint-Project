using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using FluentAssertions;
using TravelApp.Services.Auth.Controllers;
using TravelApp.Services.Auth.Data;
using TravelApp.Services.Auth.DTOs;
using TravelApp.Services.Auth.Helpers;
using TravelApp.Services.Auth.Models;

namespace TravelApp.Services.Auth.Tests.Controllers;

public class AuthControllerTests
{
    private readonly AuthDbContext _db;
    private readonly Mock<IConfiguration> _configMock;
    private readonly JwtHelper _jwtHelper;
    private readonly AuthController _controller;

    public AuthControllerTests()
    {
        var options = new DbContextOptionsBuilder<AuthDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) 
            .Options;
        _db = new AuthDbContext(options);


        _configMock = new Mock<IConfiguration>();
        var mockSection = new Mock<IConfigurationSection>();
        mockSection.Setup(s => s["SecretKey"]).Returns("ThisIsAStrongSecretKeyForInternalTestsOnly");
        _configMock.Setup(c => c.GetSection("JwtSettings")).Returns(mockSection.Object);
        _jwtHelper = new JwtHelper(_configMock.Object);

        _controller = new AuthController(_db, _jwtHelper);
    }

    [Fact]
    public async Task Register_ShouldCreateUser_WhenEmailIsUnique()
    {
        // Arrange
        var dto = new RegisterDto("Test User", "test@example.com", "Password123!", "User");

        // Act
        var result = await _controller.Register(dto);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var response = okResult.Value.Should().BeOfType<AuthResponseDto>().Subject;
        
        response.Email.Should().Be(dto.Email);
        _db.Users.Should().Contain(u => u.Email == dto.Email);
    }

    [Fact]
    public async Task Register_ShouldReturnBadRequest_WhenEmailAlreadyExists()
    {
        // Arrange
        var existingUser = new User { Name = "Old", Email = "duplicate@example.com", PasswordHash = "...", Role = "User" };
        _db.Users.Add(existingUser);
        await _db.SaveChangesAsync();

        var dto = new RegisterDto("New", "duplicate@example.com", "Password123!", "User");

        // Act
        var result = await _controller.Register(dto);

        // Assert
        result.Result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task Login_ShouldReturnOk_WhenCredentialsAreValid()
    {
        // Arrange
        var password = "Password123!";
        var user = new User 
        { 
            Name = "John", 
            Email = "john@example.com", 
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(password), 
            Role = "User" 
        };
        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        var dto = new LoginDto("john@example.com", password);

        // Act
        var result = await _controller.Login(dto);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var response = okResult.Value.Should().BeOfType<AuthResponseDto>().Subject;
        
        response.Token.Should().NotBeNullOrEmpty();
    }
}
