using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using QualityControlApp.Models.Entities;
using QualityControlApp.Services;
using QualityControlApp.ViewModels;
using System.Security.Claims;

namespace QualityControlApp.Controllers
{
    [Authorize]
    public class FcmController : BaseController
    {
        private readonly IFcmService _fcmService;
        private readonly UserManager<ApplicationUser> _userManager;

        public FcmController(IFcmService fcmService, UserManager<ApplicationUser> userManager, IWebHostEnvironment host, IConfiguration configuration) 
            : base(host, configuration)
        {
            _fcmService = fcmService;
            _userManager = userManager;
        }

        [HttpGet]
        [Authorize(Policy = "AdminOrProgPolicy")]
        public async Task<IActionResult> TestPage()
        {
            try
            {
                var viewModel = await GetTestPageViewModel();
                return View(viewModel);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error loading test page: {ex.Message}";
                return View(new FcmTestPageVM());
            }
        }

        [HttpPost]
        [Authorize(Policy = "AdminOrProgPolicy")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendTestNotification(FcmNotificationVM model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View("TestPage", await GetTestPageViewModel());
                }

                FcmNotification notification;

                // Determine how to send the notification based on selections
                if (model.SelectedTokens.Any())
                {
                    notification = await _fcmService.SendNotificationToTokensAsync(model, model.SelectedTokens);
                }
                else if (model.SelectedUserIds.Any())
                {
                    // Send to multiple users
                    var firstUserId = model.SelectedUserIds.First();
                    notification = await _fcmService.SendNotificationToUserAsync(model, firstUserId);
                    
                    // Send to remaining users
                    foreach (var userId in model.SelectedUserIds.Skip(1))
                    {
                        await _fcmService.SendNotificationToUserAsync(model, userId);
                    }
                }
                else if (model.SelectedDeviceIds.Any())
                {
                    // Send to multiple devices
                    var firstDeviceId = model.SelectedDeviceIds.First();
                    notification = await _fcmService.SendNotificationToDeviceAsync(model, firstDeviceId);
                    
                    // Send to remaining devices
                    foreach (var deviceId in model.SelectedDeviceIds.Skip(1))
                    {
                        await _fcmService.SendNotificationToDeviceAsync(model, deviceId);
                    }
                }
                else
                {
                    // Send based on target type
                    notification = await _fcmService.SendNotificationAsync(model);
                }

                TempData["SuccessMessage"] = $"Notification sent successfully! ID: {notification.Id}";
                return RedirectToAction(nameof(TestPage));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error sending notification: {ex.Message}";
                return View("TestPage", await GetTestPageViewModel());
            }
        }

        private async Task<FcmTestPageVM> GetTestPageViewModel()
        {
            var availableTokens = await _fcmService.GetActiveTokensAsync();
            var availableUsers = _userManager.Users.ToList();
            var availableDeviceIds = availableTokens
                .Where(t => !string.IsNullOrEmpty(t.DeviceId))
                .Select(t => t.DeviceId!)
                .Distinct()
                .ToList();

            return new FcmTestPageVM
            {
                Notification = new FcmNotificationVM(),
                AvailableTokens = availableTokens,
                AvailableUsers = availableUsers,
                AvailableDeviceIds = availableDeviceIds
            };
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> RegisterToken([FromBody] RegisterTokenRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.Token))
                {
                    return BadRequest(new { success = false, message = "Token is required" });
                }

                string? userId = null;
                if (User.Identity?.IsAuthenticated == true)
                {
                    userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                }

                var tokenId = await _fcmService.RegisterTokenAsync(
                    request.Token,
                    userId,
                    request.DeviceId,
                    request.DeviceType,
                    request.BrowserInfo,
                    request.UserAgent
                );

                return Ok(new { success = true, tokenId = tokenId });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> UnregisterToken([FromBody] UnregisterTokenRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.Token))
                {
                    return BadRequest(new { success = false, message = "Token is required" });
                }

                var result = await _fcmService.UnregisterTokenAsync(request.Token);
                return Ok(new { success = result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        [Authorize(Policy = "AdminOrProgPolicy")]
        public async Task<IActionResult> Tokens()
        {
            try
            {
                var tokens = await _fcmService.GetActiveTokensAsync();
                return View(tokens);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error loading tokens: {ex.Message}";
                return View(new List<FcmTokenVM>());
            }
        }

        [HttpPost]
        [Authorize(Policy = "AdminOrProgPolicy")]
        public async Task<IActionResult> CleanupExpiredTokens()
        {
            try
            {
                await _fcmService.CleanupExpiredTokensAsync();
                TempData["SuccessMessage"] = "Expired tokens cleaned up successfully";
                return RedirectToAction(nameof(Tokens));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error cleaning up tokens: {ex.Message}";
                return RedirectToAction(nameof(Tokens));
            }
        }

        [HttpGet]
        [Authorize(Policy = "AdminOrProgPolicy")]
        public async Task<IActionResult> Notifications()
        {
            try
            {
                // This would need to be implemented to show notification history
                return View();
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error loading notifications: {ex.Message}";
                return View();
            }
        }
    }

    public class RegisterTokenRequest
    {
        public string Token { get; set; } = string.Empty;
        public string? DeviceId { get; set; }
        public string? DeviceType { get; set; }
        public string? BrowserInfo { get; set; }
        public string? UserAgent { get; set; }
    }

    public class UnregisterTokenRequest
    {
        public string Token { get; set; } = string.Empty;
    }
}
