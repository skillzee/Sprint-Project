using Moq;
using FluentAssertions;
using MassTransit;
using TravelApp.Services.Booking.DTOs;
using TravelApp.Services.Booking.Interfaces;
using TravelApp.Services.Booking.Services;
using TravelApp.Shared;

namespace TravelApp.Services.Booking.Tests;

public class BookingServiceTests
{
    private readonly Mock<IBookingRepository> _repoMock;
    private readonly Mock<IPublishEndpoint> _busMock;
    private readonly BookingService _service;

    public BookingServiceTests()
    {
        _repoMock = new Mock<IBookingRepository>();
        _busMock = new Mock<IPublishEndpoint>();
        _service = new BookingService(_repoMock.Object, _busMock.Object);
    }

    [Fact]
    public async Task CreateBookingAsync_ShouldCalculateCorrectPrice_AndPublishEvent()
    {
        // Arrange
        var dto = new CreateBookingDto(1, "Deluxe", "Hotel", DateTime.Today.AddDays(1), DateTime.Today.AddDays(3), 100);
        // 2 nights * 100 = 200

        // Act
        var result = await _service.CreateBookingAsync(dto, 1, "User", "user@test.com");

        // Assert
        result.Should().NotBeNull();
        result!.TotalPrice.Should().Be(200);
        _repoMock.Verify(r => r.AddAsync(It.IsAny<Models.Booking>()), Times.Once);
        _repoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        _busMock.Verify(b => b.Publish(It.IsAny<BookingConfirmedEvent>(), default), Times.Once);
        
    }

    [Fact]
    public async Task CreateBookingAsync_ShouldReturnNull_WhenDatesAreInvalid()
    {
        // Arrange (Check-out before check-in)
        var dto = new CreateBookingDto(1, "Deluxe", "Hotel", DateTime.Today.AddDays(3), DateTime.Today.AddDays(1), 100);

        // Act
        var result = await _service.CreateBookingAsync(dto, 1, "User", "user@test.com");

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task CreateBookingAsync_ShouldReturnNull_WhenDuplicateBookingExists()
    {
        // Arrange
        var dto = new CreateBookingDto(1, "Deluxe", "Hotel", DateTime.Today.AddDays(1), DateTime.Today.AddDays(3), 100);
        _repoMock.Setup(r => r.HasOverlappingBookingAsync(1, dto.RoomId, dto.CheckInDate, dto.CheckOutDate)).ReturnsAsync(true);

        // Act
        var result = await _service.CreateBookingAsync(dto, 1, "User", "user@test.com");

        // Assert
        result.Should().BeNull();
        _repoMock.Verify(r => r.AddAsync(It.IsAny<Models.Booking>()), Times.Never);
        _repoMock.Verify(r => r.SaveChangesAsync(), Times.Never);
        _busMock.Verify(b => b.Publish(It.IsAny<BookingConfirmedEvent>(), default), Times.Never);
    }

    [Fact]
    public async Task CancelBookingAsync_ShouldReturnFalse_WhenBookingNotFound()
    {
        //Arrange
        _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Models.Booking?)null);

        //Act
        var result =await _service.CancelBookingAsync(1, 1, "User");

        //Assert
        result.Should().Be(false);
        _repoMock.Verify(r => r.SaveChangesAsync(), Times.Never);
        _busMock.Verify(b => b.Publish(It.IsAny<BookingCancelledEvent>(), default), Times.Never);

    }



    [Fact]
    public async Task CancelBookingAsync_ShouldReturnFalse_WhenUserIsUnAuthorized()
    {
        //Arramge
        var booking = new Models.Booking
        {
            Id = 1,
            UserId = 2
        };
        _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(booking);


        //Act
        var result = await _service.CancelBookingAsync(1, 1, "User");

        //Assert
        result.Should().Be(false);
        _repoMock.Verify(r => r.SaveChangesAsync(), Times.Never);
        _busMock.Verify(b => b.Publish(It.IsAny<BookingCancelledEvent>(), default), Times.Never);


    }




}
