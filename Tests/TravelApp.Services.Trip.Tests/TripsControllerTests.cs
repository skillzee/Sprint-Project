using Moq;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Collections.Generic;
using TravelApp.Services.Trip.Controllers;
using TravelApp.Services.Trip.DTOs;
using TravelApp.Services.Trip.Interfaces;

namespace TravelApp.Services.Trip.Tests;

public class TripsControllerTests
{
    private readonly Mock<ITripService> _serviceMock;
    private readonly TripsController _controller;

    public TripsControllerTests()
    {
        _serviceMock = new Mock<ITripService>();
        _controller = new TripsController(_serviceMock.Object);

        // Mocking the logged in user
        var user = new ClaimsPrincipal(new ClaimsIdentity(
            new[]
            {
                new Claim(ClaimTypes.NameIdentifier, "1"),
                new Claim(ClaimTypes.Name, "Test User"),
                new Claim(ClaimTypes.Email, "test@example.com")
            }
        ));

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = user }
        };
    }

    [Fact]
    public async Task GetMine_ShouldReturnOk_WithTrips()
    {
        // Arrange
        var trips = new List<TripDto> 
        { 
            new TripDto(1, 1, "Paris", DateTime.Today, DateTime.Today.AddDays(7), DateTime.UtcNow, new List<ItineraryDto>()) 
        };
        _serviceMock.Setup(s => s.GetUserTripsAsync(1)).ReturnsAsync(trips);

        // Act
        var result = await _controller.GetMine();

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var value = okResult.Value.Should().BeAssignableTo<IEnumerable<TripDto>>().Subject;
        value.Should().HaveCount(1);
    }

    [Fact]
    public async Task GetById_ShouldReturnNotFound_WhenServiceReturnsNull()
    {
        // Arrange
        _serviceMock.Setup(s => s.GetTripByIdAsync(1, 1)).ReturnsAsync((TripDto?)null);

        // Act
        var result = await _controller.GetById(1);

        // Assert
        result.Result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task Create_ShouldReturnOk_WhenSuccessful()
    {
        // Arrange
        var dto = new CreateTripDto("Paris", DateTime.Today, DateTime.Today.AddDays(7));
        var response = new TripDto(1, 1, "Paris", dto.StartDate, dto.EndDate, DateTime.UtcNow, new List<ItineraryDto>());
        _serviceMock.Setup(s => s.CreateTripAsync(dto, 1)).ReturnsAsync(response);

        // Act
        var result = await _controller.Create(dto);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().Be(response);
    }

    [Fact]
    public async Task Delete_ShouldReturnNoContent_WhenSuccessful()
    {
        // Arrange
        _serviceMock.Setup(s => s.DeleteTripAsync(1, 1)).ReturnsAsync(true);

        // Act
        var result = await _controller.Delete(1);

        // Assert
        result.Should().BeOfType<NoContentResult>();
    }
}
