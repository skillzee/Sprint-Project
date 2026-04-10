using Moq;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using TravelApp.Services.Hotel.Controllers;
using TravelApp.Services.Hotel.DTOs;
using TravelApp.Services.Hotel.Interfaces;

namespace TravelApp.Services.Hotel.Tests;

public class HotelsControllerTests
{
    private readonly Mock<IHotelService> _serviceMock;
    private readonly HotelsController _controller;

    public HotelsControllerTests()
    {
        _serviceMock = new Mock<IHotelService>();
        _controller = new HotelsController(_serviceMock.Object);
    }

    [Fact]
    public async Task GetAll_ShouldReturnOk_WithData()
    {
        // Arrange
        var hotels = new List<HotelDto> 
        { 
            new HotelDto(1, "Hotel 1", "Delhi", "Addr", "Desc", 5, "WiFi", new List<RoomDto>(), 1, "owner@test.com", "Owner", "Approved") 
        };
        _serviceMock.Setup(s => s.GetHotelsAsync(null)).ReturnsAsync(hotels);

        // Act
        var result = await _controller.GetAll(null);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var value = okResult.Value.Should().BeAssignableTo<IEnumerable<HotelDto>>().Subject;
        value.Should().HaveCount(1);
    }

    [Fact]
    public async Task Get_ShouldReturnNotFound_WhenServiceReturnsNull()
    {
        // Arrange
        _serviceMock.Setup(s => s.GetHotelByIdAsync(1)).ReturnsAsync((HotelDto?)null);

        // Act
        var result = await _controller.Get(1);

        // Assert
        result.Result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task Create_ShouldReturnOk_WhenSuccessful()
    {
        // Arrange
        var dto = new CreateHotelDto("New", "City", "Addr", "Desc", 5, "WiFi");
        var response = new HotelDto(1, "New", "City", "Addr", "Desc", 5, "WiFi", new List<RoomDto>(), 1, "owner@test.com", "Owner", "Approved");
        _serviceMock.Setup(s => s.CreateHotelAsync(dto, 1, "owner@test.com", "Owner")).ReturnsAsync(response);

        var user = new System.Security.Claims.ClaimsPrincipal(new System.Security.Claims.ClaimsIdentity(new System.Security.Claims.Claim[]
        {
            new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.NameIdentifier, "1"),
            new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Email, "owner@test.com"),
            new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Name, "Owner")
        }, "mock"));

        _controller.ControllerContext = new ControllerContext()
        {
            HttpContext = new Microsoft.AspNetCore.Http.DefaultHttpContext() { User = user }
        };

        // Act
        var result = await _controller.Create(dto);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().Be(response);
    }

    [Fact]
    public async Task Delete_ShouldReturnOk_WhenFound()
    {
        // Arrange
        _serviceMock.Setup(s => s.DeleteHotelAsync(1)).ReturnsAsync(true);

        // Act
        var result = await _controller.Delete(1);

        // Assert
        result.Should().BeOfType<OkResult>();
    }
}
