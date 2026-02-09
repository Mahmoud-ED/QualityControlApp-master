using QualityControlApp.Models.Entities;
using System.Text.Json;

namespace QualityControlApp.Services
{
    public class LoggingService : ILoggingService
    {
        private readonly ILogger<LoggingService> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public LoggingService(ILogger<LoggingService> logger, IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task LogLandingCreatedAsync(Landing landing, string userId)
        {
            try
            {
                var logData = new
                {
                    Action = "LandingCreated",
                    LandingId = landing.Id,
                    OperatorName = landing.OperatorName,
                    AircraftRegistration = landing.AircraftRegistration,
                    FlightDate = landing.DateOfFlights,
                    UserId = userId,
                    Timestamp = DateTime.UtcNow,
                    IPAddress = GetClientIPAddress(),
                    UserAgent = GetUserAgent()
                };

                _logger.LogInformation("Landing created: {LogData}", JsonSerializer.Serialize(logData));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error logging landing creation for ID: {LandingId}", landing.Id);
            }
        }

        public async Task LogLandingUpdatedAsync(Landing landing, string userId, string changes)
        {
            try
            {
                var logData = new
                {
                    Action = "LandingUpdated",
                    LandingId = landing.Id,
                    OperatorName = landing.OperatorName,
                    AircraftRegistration = landing.AircraftRegistration,
                    Changes = changes,
                    UserId = userId,
                    Timestamp = DateTime.UtcNow,
                    IPAddress = GetClientIPAddress(),
                    UserAgent = GetUserAgent()
                };

                _logger.LogInformation("Landing updated: {LogData}", JsonSerializer.Serialize(logData));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error logging landing update for ID: {LandingId}", landing.Id);
            }
        }

        public async Task LogLandingDeletedAsync(Guid landingId, string userId)
        {
            try
            {
                var logData = new
                {
                    Action = "LandingDeleted",
                    LandingId = landingId,
                    UserId = userId,
                    Timestamp = DateTime.UtcNow,
                    IPAddress = GetClientIPAddress(),
                    UserAgent = GetUserAgent()
                };

                _logger.LogWarning("Landing deleted: {LogData}", JsonSerializer.Serialize(logData));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error logging landing deletion for ID: {LandingId}", landingId);
            }
        }

        public async Task LogLandingStatusChangedAsync(Guid landingId, string oldStatus, string newStatus, string userId)
        {
            try
            {
                var logData = new
                {
                    Action = "LandingStatusChanged",
                    LandingId = landingId,
                    OldStatus = oldStatus,
                    NewStatus = newStatus,
                    UserId = userId,
                    Timestamp = DateTime.UtcNow,
                    IPAddress = GetClientIPAddress(),
                    UserAgent = GetUserAgent()
                };

                _logger.LogInformation("Landing status changed: {LogData}", JsonSerializer.Serialize(logData));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error logging status change for landing ID: {LandingId}", landingId);
            }
        }

        public async Task LogFileUploadedAsync(string fileName, string filePath, string userId)
        {
            try
            {
                var logData = new
                {
                    Action = "FileUploaded",
                    FileName = fileName,
                    FilePath = filePath,
                    UserId = userId,
                    Timestamp = DateTime.UtcNow,
                    IPAddress = GetClientIPAddress(),
                    UserAgent = GetUserAgent()
                };

                _logger.LogInformation("File uploaded: {LogData}", JsonSerializer.Serialize(logData));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error logging file upload: {FileName}", fileName);
            }
        }

        public async Task LogFileDeletedAsync(string filePath, string userId)
        {
            try
            {
                var logData = new
                {
                    Action = "FileDeleted",
                    FilePath = filePath,
                    UserId = userId,
                    Timestamp = DateTime.UtcNow,
                    IPAddress = GetClientIPAddress(),
                    UserAgent = GetUserAgent()
                };

                _logger.LogInformation("File deleted: {LogData}", JsonSerializer.Serialize(logData));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error logging file deletion: {FilePath}", filePath);
            }
        }

        public async Task LogErrorAsync(string message, Exception? exception = null, string? userId = null)
        {
            try
            {
                var logData = new
                {
                    Action = "Error",
                    Message = message,
                    Exception = exception?.ToString(),
                    UserId = userId,
                    Timestamp = DateTime.UtcNow,
                    IPAddress = GetClientIPAddress(),
                    UserAgent = GetUserAgent()
                };

                _logger.LogError(exception, "Application error: {LogData}", JsonSerializer.Serialize(logData));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error logging application error: {Message}", message);
            }
        }

        public async Task LogSecurityEventAsync(string eventType, string description, string? userId = null)
        {
            try
            {
                var logData = new
                {
                    Action = "SecurityEvent",
                    EventType = eventType,
                    Description = description,
                    UserId = userId,
                    Timestamp = DateTime.UtcNow,
                    IPAddress = GetClientIPAddress(),
                    UserAgent = GetUserAgent()
                };

                _logger.LogWarning("Security event: {LogData}", JsonSerializer.Serialize(logData));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error logging security event: {EventType}", eventType);
            }
        }

        private string? GetClientIPAddress()
        {
            try
            {
                var httpContext = _httpContextAccessor.HttpContext;
                if (httpContext == null) return null;

                // Check for forwarded IP first
                var forwardedFor = httpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault();
                if (!string.IsNullOrEmpty(forwardedFor))
                {
                    return forwardedFor.Split(',')[0].Trim();
                }

                // Check for real IP
                var realIp = httpContext.Request.Headers["X-Real-IP"].FirstOrDefault();
                if (!string.IsNullOrEmpty(realIp))
                {
                    return realIp;
                }

                // Fall back to connection remote IP
                return httpContext.Connection.RemoteIpAddress?.ToString();
            }
            catch
            {
                return null;
            }
        }

        private string? GetUserAgent()
        {
            try
            {
                var httpContext = _httpContextAccessor.HttpContext;
                return httpContext?.Request.Headers["User-Agent"].FirstOrDefault();
            }
            catch
            {
                return null;
            }
        }
    }
}
