using QualityControlApp.Models.Entities;
using QualityControlApp.ViewModels;

namespace QualityControlApp.Services
{
    public interface IFcmService
    {
        Task<string> RegisterTokenAsync(string token, string? userId = null, string? deviceId = null, string? deviceType = null, string? browserInfo = null, string? userAgent = null);
        Task<bool> UnregisterTokenAsync(string token);
        Task<bool> UnregisterUserTokensAsync(string userId);
        Task<FcmNotification> SendNotificationAsync(FcmNotificationVM notification);
        Task<FcmNotification> SendNotificationToTokensAsync(FcmNotificationVM notification, List<string> tokens);
        Task<FcmNotification> SendNotificationToUserAsync(FcmNotificationVM notification, string userId);
        Task<FcmNotification> SendNotificationToDeviceAsync(FcmNotificationVM notification, string deviceId);
        Task<List<FcmTokenVM>> GetActiveTokensAsync();
        Task<List<FcmTokenVM>> GetUserTokensAsync(string userId);
        Task<List<FcmTokenVM>> GetDeviceTokensAsync(string deviceId);
        Task<bool> IsTokenValidAsync(string token);
        Task CleanupExpiredTokensAsync();
    }
}
