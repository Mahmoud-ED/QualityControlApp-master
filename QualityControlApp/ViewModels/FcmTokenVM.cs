using System.ComponentModel.DataAnnotations;

namespace QualityControlApp.ViewModels
{
    public class FcmTokenVM
    {
        public Guid Id { get; set; }
        
        [Required]
        [StringLength(500)]
        public string Token { get; set; } = string.Empty;
        
        [StringLength(450)]
        public string? UserId { get; set; }
        
        [StringLength(100)]
        public string? DeviceId { get; set; }
        
        [StringLength(50)]
        public string? DeviceType { get; set; }
        
        [StringLength(200)]
        public string? BrowserInfo { get; set; }
        
        [StringLength(100)]
        public string? UserAgent { get; set; }
        
        public bool IsActive { get; set; }
        
        public DateTime CreatedAt { get; set; }
        
        public DateTime LastUsedAt { get; set; }
        
        public DateTime? ExpiresAt { get; set; }
        
        // User information for display
        public string? UserName { get; set; }
        public string? UserEmail { get; set; }
    }
}
