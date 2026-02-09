using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;
using Microsoft.EntityFrameworkCore;
using QualityControlApp.Models;
using QualityControlApp.Models.Entities;
using QualityControlApp.ViewModels;
using System.Text.Json;

namespace QualityControlApp.Services
{
    public class FcmService : IFcmService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<FcmService> _logger;
        private readonly IConfiguration _configuration;

        public FcmService(ApplicationDbContext context, ILogger<FcmService> logger, IConfiguration configuration)
        {
            _context = context;
            _logger = logger;
            _configuration = configuration;
            InitializeFirebase();
        }

        private void InitializeFirebase()
        {
            try
            {
                if (FirebaseApp.DefaultInstance == null)
                {
                    var firebaseConfigPath = _configuration["Firebase:ConfigPath"];
                    var firebaseConfigJson = _configuration["Firebase:ConfigJson"];

                    if (!string.IsNullOrEmpty(firebaseConfigPath) && File.Exists(firebaseConfigPath))
                    {
                        FirebaseApp.Create(new AppOptions()
                        {
                            Credential = GoogleCredential.FromFile(firebaseConfigPath)
                        });
                    }
                    else if (!string.IsNullOrEmpty(firebaseConfigJson))
                    {
                        var credential = GoogleCredential.FromJson(firebaseConfigJson);
                        FirebaseApp.Create(new AppOptions()
                        {
                            Credential = credential
                        });
                    }
                    else
                    {
                        _logger.LogWarning("Firebase configuration not found. FCM will not work properly.");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to initialize Firebase");
            }
        }

        public async Task<string> RegisterTokenAsync(string token, string? userId = null, string? deviceId = null, string? deviceType = null, string? browserInfo = null, string? userAgent = null)
        {
            try
            {
                // Check if token already exists
                var existingToken = await _context.FcmTokens
                    .FirstOrDefaultAsync(t => t.Token == token);

                if (existingToken != null)
                {
                    // Update existing token
                    existingToken.UserId = userId;
                    existingToken.DeviceId = deviceId;
                    existingToken.DeviceType = deviceType;
                    existingToken.BrowserInfo = browserInfo;
                    existingToken.UserAgent = userAgent;
                    existingToken.IsActive = true;
                    existingToken.LastUsedAt = DateTime.UtcNow;
                    existingToken.ExpiresAt = DateTime.UtcNow.AddDays(30); // Token expires in 30 days

                    await _context.SaveChangesAsync();
                    return existingToken.Id.ToString();
                }
                else
                {
                    // Create new token
                    var fcmToken = new FcmToken
                    {
                        Token = token,
                        UserId = userId,
                        DeviceId = deviceId,
                        DeviceType = deviceType ?? "web",
                        BrowserInfo = browserInfo,
                        UserAgent = userAgent,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow,
                        LastUsedAt = DateTime.UtcNow,
                        ExpiresAt = DateTime.UtcNow.AddDays(30)
                    };

                    _context.FcmTokens.Add(fcmToken);
                    await _context.SaveChangesAsync();
                    return fcmToken.Id.ToString();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to register FCM token");
                throw;
            }
        }

        public async Task<bool> UnregisterTokenAsync(string token)
        {
            try
            {
                var fcmToken = await _context.FcmTokens
                    .FirstOrDefaultAsync(t => t.Token == token);

                if (fcmToken != null)
                {
                    fcmToken.IsActive = false;
                    await _context.SaveChangesAsync();
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to unregister FCM token");
                return false;
            }
        }

        public async Task<bool> UnregisterUserTokensAsync(string userId)
        {
            try
            {
                var tokens = await _context.FcmTokens
                    .Where(t => t.UserId == userId)
                    .ToListAsync();

                foreach (var token in tokens)
                {
                    token.IsActive = false;
                }

                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to unregister user FCM tokens");
                return false;
            }
        }

        public async Task<FcmNotification> SendNotificationAsync(FcmNotificationVM notification)
        {
            try
            {
                var fcmNotification = new FcmNotification
                {
                    Title = notification.Title,
                    Body = notification.Body,
                    ImageUrl = notification.ImageUrl,
                    ClickAction = notification.ClickAction,
                    Data = notification.Data,
                    Type = notification.Type,
                    Target = notification.Target,
                    TargetUserId = notification.TargetUserId,
                    TargetDeviceId = notification.TargetDeviceId,
                    IsScheduled = notification.IsScheduled,
                    ScheduledAt = notification.ScheduledAt,
                    CreatedAt = DateTime.UtcNow,
                    Status = NotificationStatus.Pending
                };

                _context.FcmNotifications.Add(fcmNotification);
                await _context.SaveChangesAsync();

                // Send notification based on target
                switch (notification.Target)
                {
                    case NotificationTarget.All:
                        await SendToAllTokensAsync(fcmNotification);
                        break;
                    case NotificationTarget.AuthenticatedUsers:
                        await SendToAuthenticatedUsersAsync(fcmNotification);
                        break;
                    case NotificationTarget.AnonymousUsers:
                        await SendToAnonymousUsersAsync(fcmNotification);
                        break;
                    case NotificationTarget.SpecificUser:
                        if (!string.IsNullOrEmpty(notification.TargetUserId))
                            await SendToUserAsync(fcmNotification, notification.TargetUserId);
                        break;
                    case NotificationTarget.SpecificDevice:
                        if (!string.IsNullOrEmpty(notification.TargetDeviceId))
                            await SendToDeviceAsync(fcmNotification, notification.TargetDeviceId);
                        break;
                }

                return fcmNotification;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send notification");
                throw;
            }
        }

        public async Task<FcmNotification> SendNotificationToTokensAsync(FcmNotificationVM notification, List<string> tokens)
        {
            try
            {
                var fcmNotification = new FcmNotification
                {
                    Title = notification.Title,
                    Body = notification.Body,
                    ImageUrl = notification.ImageUrl,
                    ClickAction = notification.ClickAction,
                    Data = notification.Data,
                    Type = notification.Type,
                    Target = NotificationTarget.SpecificDevice,
                    CreatedAt = DateTime.UtcNow,
                    Status = NotificationStatus.Pending
                };

                _context.FcmNotifications.Add(fcmNotification);
                await _context.SaveChangesAsync();

                await SendToSpecificTokensAsync(fcmNotification, tokens);
                return fcmNotification;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send notification to specific tokens");
                throw;
            }
        }

        public async Task<FcmNotification> SendNotificationToUserAsync(FcmNotificationVM notification, string userId)
        {
            try
            {
                var fcmNotification = new FcmNotification
                {
                    Title = notification.Title,
                    Body = notification.Body,
                    ImageUrl = notification.ImageUrl,
                    ClickAction = notification.ClickAction,
                    Data = notification.Data,
                    Type = notification.Type,
                    Target = NotificationTarget.SpecificUser,
                    TargetUserId = userId,
                    CreatedAt = DateTime.UtcNow,
                    Status = NotificationStatus.Pending
                };

                _context.FcmNotifications.Add(fcmNotification);
                await _context.SaveChangesAsync();

                await SendToUserAsync(fcmNotification, userId);
                return fcmNotification;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send notification to user");
                throw;
            }
        }

        public async Task<FcmNotification> SendNotificationToDeviceAsync(FcmNotificationVM notification, string deviceId)
        {
            try
            {
                var fcmNotification = new FcmNotification
                {
                    Title = notification.Title,
                    Body = notification.Body,
                    ImageUrl = notification.ImageUrl,
                    ClickAction = notification.ClickAction,
                    Data = notification.Data,
                    Type = notification.Type,
                    Target = NotificationTarget.SpecificDevice,
                    TargetDeviceId = deviceId,
                    CreatedAt = DateTime.UtcNow,
                    Status = NotificationStatus.Pending
                };

                _context.FcmNotifications.Add(fcmNotification);
                await _context.SaveChangesAsync();

                await SendToDeviceAsync(fcmNotification, deviceId);
                return fcmNotification;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send notification to device");
                throw;
            }
        }

        private async Task SendToAllTokensAsync(FcmNotification notification)
        {
            var tokens = await _context.FcmTokens
                .Where(t => t.IsActive && t.ExpiresAt > DateTime.UtcNow)
                .Select(t => t.Token)
                .ToListAsync();

            await SendToSpecificTokensAsync(notification, tokens);
        }

        private async Task SendToAuthenticatedUsersAsync(FcmNotification notification)
        {
            var tokens = await _context.FcmTokens
                .Where(t => t.IsActive && t.ExpiresAt > DateTime.UtcNow && !string.IsNullOrEmpty(t.UserId))
                .Select(t => t.Token)
                .ToListAsync();

            await SendToSpecificTokensAsync(notification, tokens);
        }

        private async Task SendToAnonymousUsersAsync(FcmNotification notification)
        {
            var tokens = await _context.FcmTokens
                .Where(t => t.IsActive && t.ExpiresAt > DateTime.UtcNow && string.IsNullOrEmpty(t.UserId))
                .Select(t => t.Token)
                .ToListAsync();

            await SendToSpecificTokensAsync(notification, tokens);
        }

        private async Task SendToUserAsync(FcmNotification notification, string userId)
        {
            var tokens = await _context.FcmTokens
                .Where(t => t.IsActive && t.ExpiresAt > DateTime.UtcNow && t.UserId == userId)
                .Select(t => t.Token)
                .ToListAsync();

            await SendToSpecificTokensAsync(notification, tokens);
        }

        private async Task SendToDeviceAsync(FcmNotification notification, string deviceId)
        {
            var tokens = await _context.FcmTokens
                .Where(t => t.IsActive && t.ExpiresAt > DateTime.UtcNow && t.DeviceId == deviceId)
                .Select(t => t.Token)
                .ToListAsync();

            await SendToSpecificTokensAsync(notification, tokens);
        }

        private async Task SendToSpecificTokensAsync(FcmNotification notification, List<string> tokens)
        {
            if (FirebaseApp.DefaultInstance == null)
            {
                _logger.LogWarning("Firebase not initialized. Cannot send notifications.");
                notification.Status = NotificationStatus.Failed;
                notification.ErrorMessage = "Firebase not initialized";
                await _context.SaveChangesAsync();
                return;
            }

            var messaging = FirebaseMessaging.DefaultInstance;
            var successCount = 0;
            var failureCount = 0;

            foreach (var token in tokens)
            {
                try
                {
                    var message = new Message
                    {
                        Token = token,
                        Notification = new FirebaseAdmin.Messaging.Notification
                        {
                            Title = notification.Title,
                            Body = notification.Body,
                            ImageUrl = notification.ImageUrl
                        },
                        Data = !string.IsNullOrEmpty(notification.Data) 
                            ? JsonSerializer.Deserialize<Dictionary<string, string>>(notification.Data) 
                            : new Dictionary<string, string>(),
                        Webpush = new WebpushConfig
                        {
                            FcmOptions = new WebpushFcmOptions
                            {
                                Link = notification.ClickAction
                            }
                        }
                    };

                    var response = await messaging.SendAsync(message);

                    // Log successful send
                    var tokenEntity = await _context.FcmTokens.FirstOrDefaultAsync(t => t.Token == token);
                    if (tokenEntity != null)
                    {
                        var log = new FcmNotificationLog
                        {
                            NotificationId = notification.Id,
                            TokenId = tokenEntity.Id,
                            UserId = tokenEntity.UserId,
                            DeviceId = tokenEntity.DeviceId,
                            SentAt = DateTime.UtcNow,
                            IsSuccess = true,
                            MessageId = response
                        };
                        _context.FcmNotificationLogs.Add(log);
                    }

                    successCount++;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to send notification to token: {Token}", token);
                    failureCount++;

                    // Log failed send
                    var tokenEntity = await _context.FcmTokens.FirstOrDefaultAsync(t => t.Token == token);
                    if (tokenEntity != null)
                    {
                        var log = new FcmNotificationLog
                        {
                            NotificationId = notification.Id,
                            TokenId = tokenEntity.Id,
                            UserId = tokenEntity.UserId,
                            DeviceId = tokenEntity.DeviceId,
                            SentAt = DateTime.UtcNow,
                            IsSuccess = false,
                            ErrorMessage = ex.Message
                        };
                        _context.FcmNotificationLogs.Add(log);
                    }
                }
            }

            // Update notification status
            notification.SentAt = DateTime.UtcNow;
            notification.Status = failureCount == 0 ? NotificationStatus.Sent : 
                                 successCount == 0 ? NotificationStatus.Failed : NotificationStatus.Sent;
            notification.SuccessCount = successCount;
            notification.FailureCount = failureCount;

            await _context.SaveChangesAsync();
        }

        public async Task<List<FcmTokenVM>> GetActiveTokensAsync()
        {
            return await _context.FcmTokens
                .Where(t => t.IsActive && t.ExpiresAt > DateTime.UtcNow)
                .Include(t => t.User)
                .Select(t => new FcmTokenVM
                {
                    Id = t.Id,
                    Token = t.Token,
                    UserId = t.UserId,
                    DeviceId = t.DeviceId,
                    DeviceType = t.DeviceType,
                    BrowserInfo = t.BrowserInfo,
                    UserAgent = t.UserAgent,
                    IsActive = t.IsActive,
                    CreatedAt = t.CreatedAt,
                    LastUsedAt = t.LastUsedAt,
                    UserName = t.User != null ? t.User.UserName : null,
                    UserEmail = t.User != null ? t.User.Email : null
                })
                .ToListAsync();
        }

        public async Task<List<FcmTokenVM>> GetUserTokensAsync(string userId)
        {
            return await _context.FcmTokens
                .Where(t => t.UserId == userId && t.IsActive && t.ExpiresAt > DateTime.UtcNow)
                .Include(t => t.User)
                .Select(t => new FcmTokenVM
                {
                    Id = t.Id,
                    Token = t.Token,
                    UserId = t.UserId,
                    DeviceId = t.DeviceId,
                    DeviceType = t.DeviceType,
                    BrowserInfo = t.BrowserInfo,
                    UserAgent = t.UserAgent,
                    IsActive = t.IsActive,
                    CreatedAt = t.CreatedAt,
                    LastUsedAt = t.LastUsedAt,
                    UserName = t.User != null ? t.User.UserName : null,
                    UserEmail = t.User != null ? t.User.Email : null
                })
                .ToListAsync();
        }

        public async Task<List<FcmTokenVM>> GetDeviceTokensAsync(string deviceId)
        {
            return await _context.FcmTokens
                .Where(t => t.DeviceId == deviceId && t.IsActive && t.ExpiresAt > DateTime.UtcNow)
                .Include(t => t.User)
                .Select(t => new FcmTokenVM
                {
                    Id = t.Id,
                    Token = t.Token,
                    UserId = t.UserId,
                    DeviceId = t.DeviceId,
                    DeviceType = t.DeviceType,
                    BrowserInfo = t.BrowserInfo,
                    UserAgent = t.UserAgent,
                    IsActive = t.IsActive,
                    CreatedAt = t.CreatedAt,
                    LastUsedAt = t.LastUsedAt,
                    UserName = t.User != null ? t.User.UserName : null,
                    UserEmail = t.User != null ? t.User.Email : null
                })
                .ToListAsync();
        }

        public async Task<bool> IsTokenValidAsync(string token)
        {
            return await _context.FcmTokens
                .AnyAsync(t => t.Token == token && t.IsActive && t.ExpiresAt > DateTime.UtcNow);
        }

        public async Task CleanupExpiredTokensAsync()
        {
            var expiredTokens = await _context.FcmTokens
                .Where(t => t.ExpiresAt <= DateTime.UtcNow)
                .ToListAsync();

            foreach (var token in expiredTokens)
            {
                token.IsActive = false;
            }

            await _context.SaveChangesAsync();
        }
    }
}
