using Moq;
using FluentAssertions;
using TravelApp.Services.Trip.AI;
using TravelApp.Services.Trip.DTOs;
using TravelApp.Services.Trip.Interfaces;
using TravelApp.Services.Trip.Models;
using TravelApp.Services.Trip.Services;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace TravelApp.Services.Trip.Tests;

public class TripServiceTests
{
    private readonly Mock<ITripRepository> _repoMock;
    private readonly Mock<IGeminiService> _geminiMock;
    private readonly Mock<IDistributedCache> _cacheMock;
    private readonly Mock<ILogger<TripService>> _loggerMock;
    private readonly TripService _service;

    public TripServiceTests()
    {
        _repoMock = new Mock<ITripRepository>();
        _geminiMock = new Mock<IGeminiService>();
        _cacheMock = new Mock<IDistributedCache>();
        _loggerMock = new Mock<ILogger<TripService>>();
        _service = new TripService(_repoMock.Object, _geminiMock.Object, _cacheMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task GetUserTripsAsync_ShouldReturnMappedDtos()
    {
        // Arrange
        var userId = 1;
        var trips = new List<Models.Trip>
        {
            new Models.Trip { Id = 1, UserId = userId, Destination = "Paris", Itineraries = new List<Itinerary>() }
        };
        _repoMock.Setup(r => r.GetUserTripsAsync(userId)).ReturnsAsync(trips);

        // Act
        var result = await _service.GetUserTripsAsync(userId);

        // Assert
        result.Should().HaveCount(1);
        result.First().Destination.Should().Be("Paris");
    }

    [Fact]
    public async Task CreateTripAsync_ShouldReturnNull_WhenEndDateIsBeforeStartDate()
    {
        // Arrange
        var dto = new CreateTripDto("Paris", DateTime.Today.AddDays(5), DateTime.Today.AddDays(2));

        // Act
        var result = await _service.CreateTripAsync(dto, 1);

        // Assert
        result.Should().BeNull();
        _repoMock.Verify(r => r.AddTripAsync(It.IsAny<Models.Trip>()), Times.Never);
    }

    [Fact]
    public async Task GenerateItineraryAsync_ShouldCallGeminiAndSave()
    {
        // Arrange
        var userId = 1;
        var trip = new Models.Trip { Id = 1, UserId = userId, Destination = "Paris", StartDate = DateTime.Today, EndDate = DateTime.Today.AddDays(2) };
        var itineraries = new List<Itinerary> { new Itinerary { DayNumber = 1, Activity = "Sightseeing" } };
        
        _repoMock.Setup(r => r.GetTripByIdAsync(1)).ReturnsAsync(trip);
        _geminiMock.Setup(g => g.GetItineraryAsync(It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<string>()))
            .ReturnsAsync(itineraries);

        // Act
        var result = await _service.GenerateItineraryAsync(new GenerateItineraryDto(1, "Relax"), userId);

        // Assert
        _geminiMock.Verify(g => g.GetItineraryAsync("Paris", trip.StartDate, trip.EndDate, "Relax"), Times.Once);
        _repoMock.Verify(r => r.ClearItinerariesAsync(1), Times.Once);
        _repoMock.Verify(r => r.AddItinerariesAsync(itineraries), Times.Once);
    }

    [Fact]
    public async Task DeleteTripAsync_ShouldReturnFalse_WhenUserDoesNotOwnTrip()
    {
        // Arrange
        var trip = new Models.Trip { Id = 1, UserId = 2 }; // Trip belongs to user 2
        _repoMock.Setup(r => r.GetTripByIdAsync(1)).ReturnsAsync(trip); 

        _repoMock.Setup(r => r.GetTripByIdAsync(1)).ReturnsAsync(trip);

        // Act
        var result = await _service.DeleteTripAsync(1, 1); // User 1 trying to delete

        // Assert
        result.Should().BeFalse();
        _repoMock.Verify(r => r.DeleteTripAsync(It.IsAny<Models.Trip>()), Times.Never);
    }
}
