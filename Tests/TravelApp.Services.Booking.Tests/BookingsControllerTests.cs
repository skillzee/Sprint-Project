using FluentAssertions;
using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using TravelApp.Services.Booking.Controllers;
using TravelApp.Services.Booking.Data;
using TravelApp.Services.Booking.DTOs;
using TravelApp.Services.Booking.Interfaces;
using TravelApp.Shared;


// AAA methodology -> Arrangemnent -> Act -> Assret

namespace TravelApp.Services.Booking.Tests
{
    public class BookingsControllerTests
    {

        private readonly Mock<IBookingService> _serviceMock;
        private readonly BookingsController _controller;

        public BookingsControllerTests()
        {
            _serviceMock = new Mock<IBookingService>();
            _controller = new BookingsController(_serviceMock.Object);

            

            //Mocking the logged in user
            var user = new ClaimsPrincipal(new ClaimsIdentity(
                new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, "1"),
                    new Claim(ClaimTypes.Name, "Test User"),
                    new Claim(ClaimTypes.Email, "test@example.com"),
                    new Claim(ClaimTypes.Role, "User")
                }
                ));


            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };

        }


        [Fact]
        public async Task Create_ShouldReturnOk_AndCallService_WhenValid()
        {
            // Arrange
            var dto = new CreateBookingDto(1, "Deluxe", "Grand Hotel", DateTime.Today.AddDays(1), DateTime.Today.AddDays(3), 100);
            var expectedResponse = new BookingDto(1, 1, "Test User", 1, "Deluxe", "Grand Hotel", dto.CheckInDate, dto.CheckOutDate, 200, "Confirmed", "REF123", DateTime.UtcNow);
            _serviceMock.Setup(s => s.CreateBookingAsync(dto, 1, "Test User", "test@example.com")).ReturnsAsync((expectedResponse, null));
            // Act
            var result = await _controller.Create(dto);
            // Assert
            result.Should().BeOfType<OkObjectResult>();
            _serviceMock.Verify(s=> s.CreateBookingAsync(dto, 1, "Test User", "test@example.com"), Times.Once);
        }
    }

}

