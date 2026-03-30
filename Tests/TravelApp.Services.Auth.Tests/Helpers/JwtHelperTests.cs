using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Moq;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using TravelApp.Services.Auth.Helpers;
using TravelApp.Services.Auth.Models;

namespace TravelApp.Services.Auth.Tests.Helpers
{
    public class JwtHelperTests
    {

        private readonly Mock<IConfiguration> _configMock; // Jwt depends on configuration therefore we are mocking configuration
        private readonly JwtHelper _jwtHelper;

        public JwtHelperTests()
        {
            _configMock = new Mock<IConfiguration>();
            var mockSection = new Mock<IConfigurationSection>();
            mockSection.Setup(s => s["SecretKey"]).Returns("ThisIsAStrongSecretKeyForInternalTestsOnly");
            _configMock.Setup(c => c.GetSection("JwtSettings")).Returns(mockSection.Object);

            _jwtHelper = new JwtHelper(_configMock.Object);
        }



        [Fact]
        public void GenerateToken_ShouldReturnValidJwtToken()
        {
            var user = new User
            {
                Id = 1,
                Name = "John Doe",
                Email = "john@example.com",
                Role = "Admin"
            };

            var token = _jwtHelper.GenerateToken(user);

            token.Should().NotBeNullOrEmpty();

            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadJwtToken(token);

            jsonToken.Claims.Should().Contain(c => c.Value == "1");

           
            jsonToken.Claims.Should().Contain(c => c.Value == "John Doe");
            jsonToken.Claims.Should().Contain(c => c.Value == "john@example.com");
            jsonToken.Claims.Should().Contain(c => c.Value == "Admin");
        }

    }
}
