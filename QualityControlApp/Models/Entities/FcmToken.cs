using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QualityControlApp.Models.Entities
{
    public class FcmToken
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [StringLength(500)]
        public string Token { get; set; } = string.Empty;

        [StringLength(450)]
        public string? UserId { get; set; } // For authenticated users

        [StringLength(100)]
        public string? DeviceId { get; set; } // For anonymous users

        [StringLength(50)]
        public string? DeviceType { get; set; } // "web", "android", "ios"

        [StringLength(200)]
        public string? BrowserInfo { get; set; }

        [StringLength(100)]
        public string? UserAgent { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime LastUsedAt { get; set; } = DateTime.UtcNow;

        public DateTime? ExpiresAt { get; set; }

        // Navigation properties
        [ForeignKey("UserId")]
        public virtual ApplicationUser? User { get; set; }
    }
}
