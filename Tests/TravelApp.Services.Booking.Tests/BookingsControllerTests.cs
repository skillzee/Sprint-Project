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
using TravelApp.Shared;


// AAA methodology -> Arrangemnent -> Act -> Assret

namespace TravelApp.Services.Booking.Tests
{
    public class BookingsControllerTests
    {

        private readonly BookingDbContext _db;
        private readonly Mock<IPublishEndpoint> _busMock;
        private readonly BookingsController _controller;

        public BookingsControllerTests()
        {
            var options = new DbContextOptionsBuilder<BookingDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
            

            // Db Mock
            _db = new BookingDbContext(options);

            //Bus RabbitMQ Mock
            _busMock = new Mock<IPublishEndpoint>();

            //Controller
            _controller = new BookingsController(_db, _busMock.Object);

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
        public async Task Create_ShouldSaveBooking_AndPublishEvent_WhenValid()
        {
            // Arrange
            var dto = new CreateBookingDto(1, "Deluxe", "Grand Hotel", DateTime.Today.AddDays(1), DateTime.Today.AddDays(3), 100);
            // Act
            var result = await _controller.Create(dto);
            // Assert
            result.Should().BeOfType<OkObjectResult>();
            _db.Bookings.Count().Should().Be(1);

            // Verify that MassTransit actually "sent" the message
            _busMock.Verify(x => x.Publish(It.IsAny<BookingConfirmedEvent>(), default), Times.Once);
        }
    }

}

