using Microsoft.Extensions.Logging;
using Moq;
using QualityControlApp.Models.Entities;
using QualityControlApp.Services;
using Xunit;

namespace QualityControlApp.Tests.Services
{
    public class ValidationServiceTests
    {
        private readonly Mock<ILogger<ValidationService>> _mockLogger;
        private readonly ValidationService _validationService;

        public ValidationServiceTests()
        {
            _mockLogger = new Mock<ILogger<ValidationService>>();
            _validationService = new ValidationService(_mockLogger.Object);
        }

        [Fact]
        public async Task ValidateLandingAsync_ValidLanding_ReturnsTrue()
        {
            // Arrange
            var landing = new Landing
            {
                Email = "test@example.com",
                OperatorName = "Test Operator",
                AircraftType = "Boeing 737",
                AircraftRegistration = "N12345",
                Route = "JFK-LAX",
                AirportOfLanding = "LAX",
                DateOfFlights = DateTime.Today.AddDays(1),
                ETA = DateTime.Today.AddDays(1).AddHours(10),
                ETD = DateTime.Today.AddDays(1).AddHours(12),
                RequestStatus = "Pending"
            };

            // Act
            var (isValid, errors) = await _validationService.ValidateLandingAsync(landing);

            // Assert
            Assert.True(isValid);
            Assert.Empty(errors);
        }

        [Fact]
        public async Task ValidateLandingAsync_InvalidEmail_ReturnsFalse()
        {
            // Arrange
            var landing = new Landing
            {
                Email = "invalid-email",
                OperatorName = "Test Operator",
                AircraftType = "Boeing 737",
                AircraftRegistration = "N12345",
                Route = "JFK-LAX",
                AirportOfLanding = "LAX",
                DateOfFlights = DateTime.Today.AddDays(1),
                ETA = DateTime.Today.AddDays(1).AddHours(10),
                ETD = DateTime.Today.AddDays(1).AddHours(12),
                RequestStatus = "Pending"
            };

            // Act
            var (isValid, errors) = await _validationService.ValidateLandingAsync(landing);

            // Assert
            Assert.False(isValid);
            Assert.Contains("Invalid email format", errors);
        }

        [Fact]
        public async Task ValidateLandingAsync_InvalidAircraftRegistration_ReturnsFalse()
        {
            // Arrange
            var landing = new Landing
            {
                Email = "test@example.com",
                OperatorName = "Test Operator",
                AircraftType = "Boeing 737",
                AircraftRegistration = "invalid-reg",
                Route = "JFK-LAX",
                AirportOfLanding = "LAX",
                DateOfFlights = DateTime.Today.AddDays(1),
                ETA = DateTime.Today.AddDays(1).AddHours(10),
                ETD = DateTime.Today.AddDays(1).AddHours(12),
                RequestStatus = "Pending"
            };

            // Act
            var (isValid, errors) = await _validationService.ValidateLandingAsync(landing);

            // Assert
            Assert.False(isValid);
            Assert.Contains("Invalid aircraft registration format", errors);
        }

        [Fact]
        public async Task ValidateLandingAsync_ETABeforeETD_ReturnsFalse()
        {
            // Arrange
            var landing = new Landing
            {
                Email = "test@example.com",
                OperatorName = "Test Operator",
                AircraftType = "Boeing 737",
                AircraftRegistration = "N12345",
                Route = "JFK-LAX",
                AirportOfLanding = "LAX",
                DateOfFlights = DateTime.Today.AddDays(1),
                ETA = DateTime.Today.AddDays(1).AddHours(12),
                ETD = DateTime.Today.AddDays(1).AddHours(10), // ETD before ETA
                RequestStatus = "Pending"
            };

            // Act
            var (isValid, errors) = await _validationService.ValidateLandingAsync(landing);

            // Assert
            Assert.False(isValid);
            Assert.Contains("Estimated departure time must be after estimated arrival time", errors);
        }

        [Fact]
        public async Task ValidateEmailAsync_ValidEmail_ReturnsTrue()
        {
            // Act
            var result = await _validationService.ValidateEmailAsync("test@example.com");

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task ValidateEmailAsync_InvalidEmail_ReturnsFalse()
        {
            // Act
            var result = await _validationService.ValidateEmailAsync("invalid-email");

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task ValidateAircraftRegistrationAsync_ValidRegistration_ReturnsTrue()
        {
            // Act
            var result = await _validationService.ValidateAircraftRegistrationAsync("N12345");

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task ValidateAircraftRegistrationAsync_InvalidRegistration_ReturnsFalse()
        {
            // Act
            var result = await _validationService.ValidateAircraftRegistrationAsync("invalid-reg");

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task ValidateDateRangeAsync_ValidRange_ReturnsTrue()
        {
            // Arrange
            var startDate = DateTime.Today.AddDays(1).AddHours(10);
            var endDate = DateTime.Today.AddDays(1).AddHours(12);

            // Act
            var result = await _validationService.ValidateDateRangeAsync(startDate, endDate);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task ValidateDateRangeAsync_InvalidRange_ReturnsFalse()
        {
            // Arrange
            var startDate = DateTime.Today.AddDays(1).AddHours(12);
            var endDate = DateTime.Today.AddDays(1).AddHours(10); // End before start

            // Act
            var result = await _validationService.ValidateDateRangeAsync(startDate, endDate);

            // Assert
            Assert.False(result);
        }
    }
}
