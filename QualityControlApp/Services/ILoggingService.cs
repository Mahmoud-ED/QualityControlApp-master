using QualityControlApp.Models.Entities;

namespace QualityControlApp.Services
{
    public interface ILoggingService
    {
        Task LogLandingCreatedAsync(Landing landing, string userId);
        Task LogLandingUpdatedAsync(Landing landing, string userId, string changes);
        Task LogLandingDeletedAsync(Guid landingId, string userId);
        Task LogLandingStatusChangedAsync(Guid landingId, string oldStatus, string newStatus, string userId);
        Task LogFileUploadedAsync(string fileName, string filePath, string userId);
        Task LogFileDeletedAsync(string filePath, string userId);
        Task LogErrorAsync(string message, Exception? exception = null, string? userId = null);
        Task LogSecurityEventAsync(string eventType, string description, string? userId = null);
    }
}
