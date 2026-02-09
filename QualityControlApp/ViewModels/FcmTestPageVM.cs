using QualityControlApp.Models.Entities;
using Microsoft.AspNetCore.Identity;

namespace QualityControlApp.ViewModels
{
    public class FcmTestPageVM
    {
        public FcmNotificationVM Notification { get; set; } = new FcmNotificationVM();
        
        public List<ApplicationUser> AvailableUsers { get; set; } = new List<ApplicationUser>();
        
        public List<FcmTokenVM> AvailableTokens { get; set; } = new List<FcmTokenVM>();
        
        public List<string> AvailableDeviceIds { get; set; } = new List<string>();
    }
}
