using Moq;
using FluentAssertions;
using TravelApp.Services.Hotel.DTOs;
using TravelApp.Services.Hotel.Interfaces;
using TravelApp.Services.Hotel.Models;
using TravelApp.Services.Hotel.Services;

namespace TravelApp.Services.Hotel.Tests;

public class HotelServiceTests
{
    private readonly Mock<IHotelRepository> _repoMock;
    private readonly HotelService _service;

    public HotelServiceTests()
    {
        _repoMock = new Mock<IHotelRepository>();
        _service = new HotelService(_repoMock.Object);
    }

    [Fact]
    public async Task GetHotelsAsync_ShouldReturnMappedDtos()
    {
        // Arrange
        var hotels = new List<Models.Hotel>
        {
            new Models.Hotel { Id = 1, Name = "Hotel 1", City = "Delhi", Rooms = new List<Room>() },
            new Models.Hotel { Id = 2, Name = "Hotel 2", City = "Mumbai", Rooms = new List<Room>() }
        };
        _repoMock.Setup(r => r.GetAllWithRoomsAsync(null)).ReturnsAsync(hotels);

        // Act
        var result = await _service.GetHotelsAsync(null);

        // Assert
        result.Should().HaveCount(2);
        result.First().Name.Should().Be("Hotel 1");
    }

    [Fact]
    public async Task GetHotelByIdAsync_ShouldReturnDto_WhenFound()
    {
        // Arrange
        var hotel = new Models.Hotel { Id = 1, Name = "Hotel 1", Rooms = new List<Room>() };
        _repoMock.Setup(r => r.GetByIdWithRoomsAsync(1)).ReturnsAsync(hotel);

        // Act
        var result = await _service.GetHotelByIdAsync(1);

        // Assert
        result.Should().NotBeNull();
        result!.Name.Should().Be("Hotel 1");
    }

    [Fact]
    public async Task CreateHotelAsync_ShouldCallAddAndSave()
    {
        // Arrange
        var dto = new CreateHotelDto("New Hotel", "Delhi", "Addr", "Desc", 5, "WiFi");

        // Act
        var result = await _service.CreateHotelAsync(dto);

        // Assert
        result.Name.Should().Be(dto.Name);
        _repoMock.Verify(r => r.AddHotelAsync(It.IsAny<Models.  Hotel>()), Times.Once);
        _repoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task AddRoomToHotelAsync_ShouldReturnNull_WhenHotelNotFound()
    {
        // Arrange
        _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Models.Hotel?)null);

        // Act
        var result = await _service.AddRoomToHotelAsync(1, new CreateRoomDto(1,"Deluxe", 100, 2, "Desc"));

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task DeleteHotelAsync_ShouldReturnTrue_WhenSuccessful()
    {
        // Arrange
        var hotel = new Models.Hotel { Id = 1 };
        _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(hotel);

        // Act
        var result = await _service.DeleteHotelAsync(1);

        // Assert
        result.Should().BeTrue();
        _repoMock.Verify(r => r.DeleteHotelAsync(hotel), Times.Once);
        _repoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }
}
