using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using TravelApp.Services.Flight.DTOs;
using TravelApp.Services.Flight.Interfaces;
using TravelApp.Services.Flight.Services;
using Microsoft.Extensions.Caching.Distributed;

namespace TravelApp.Services.Flights.Tests
{
    public class FlightServiceTests
    {

        private readonly Mock<IFlightRepository> _repoMock;
        private readonly Mock<ILogger<FlightService>> _loggerMock;
        private readonly Mock<IDistributedCache> _cacheMock;
        private readonly FlightService _service;

        public FlightServiceTests()
        {
            _repoMock = new Mock<IFlightRepository>();
            _loggerMock = new Mock<ILogger<FlightService>>();
            _cacheMock = new Mock<IDistributedCache>();
            _service = new FlightService(_repoMock.Object, _loggerMock.Object, _cacheMock.Object);
        }



        [Fact]
        public async Task SearchFlightsAsync_ShouldReturnEmptyList_WhenRepoReturnsNull()
        {
            // Arrange
            _repoMock
                .Setup(r => r.GetFlightsRawAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync((SerpApiFlightResponse?)null);

            // Act
            var result = await _service.SearchFlightsAsync("DEL", "BOM", "2026-04-01");

            // Assert
            result.Should().BeEmpty();
        }

        [Fact]
        public async Task SearchFlightsAsync_ShouldReturnMappedFlights_WhenValidData()
        {
            // Arrange
            var mockResponse = new SerpApiFlightResponse
            {
                best_flights = new List<SerpApiFlightOption>
            {
            new SerpApiFlightOption
            {
                price = 5000,
                total_duration = 120,
                flights = new List<SerpApiFlight>
                {
                    new SerpApiFlight
                    {
                        airline = "Indigo",
                        flight_number = "6E123",
                        travel_class = "Economy",
                        departure_airport = new SerpApiAirport
                        {
                            name = "Delhi",
                            id = "DEL",
                            time = "10:00"
                        },
                        arrival_airport = new SerpApiAirport
                        {
                            name = "Mumbai",
                            id = "BOM",
                            time = "12:00"
                        }
                    }
                }
            }
        },
                search_metadata = new SerpApiSearchMetadata
                {
                    google_flights_url = "test-url"
                }
            };

            _repoMock
                .Setup(r => r.GetFlightsRawAsync("DEL", "BOM", "2026-04-01", 1, 1))
                .ReturnsAsync(mockResponse);

            // Act
            var result = await _service.SearchFlightsAsync("DEL", "BOM", "2026-04-01");

            // Assert
            result.Should().HaveCount(1);

            var flight = result.First();
            flight.Airline.Should().Be("Indigo");
            flight.OriginIata.Should().Be("DEL");
            flight.DestinationIata.Should().Be("BOM");
            flight.Price.Should().Be(5000);
            flight.IsNonStop.Should().BeTrue();
        }


        [Fact]
        public async Task SearchFlightsAsync_ShouldMapBusinessClassToCorrectInt()
        {
            // Act
            await _service.SearchFlightsAsync("DEL", "BOM", "2026-04-01", 1, "BUSINESS");

            // Assert - verify the repository was called with '3' for travel class
            _repoMock.Verify(r => r.GetFlightsRawAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), 3), Times.Once);
        }




    }
}
