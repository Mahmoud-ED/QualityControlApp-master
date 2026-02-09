using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QualityControlApp.Models.Entities
{
    public class FcmNotificationLog
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid NotificationId { get; set; }

        [Required]
        public Guid TokenId { get; set; }

        [StringLength(450)]
        public string? UserId { get; set; }

        [StringLength(100)]
        public string? DeviceId { get; set; }

        public DateTime SentAt { get; set; } = DateTime.UtcNow;

        public bool IsSuccess { get; set; }

        [StringLength(1000)]
        public string? ErrorMessage { get; set; }

        [StringLength(100)]
        public string? MessageId { get; set; } // FCM message ID

        // Navigation properties
        [ForeignKey("NotificationId")]
        public virtual FcmNotification Notification { get; set; } = null!;

        [ForeignKey("TokenId")]
        public virtual FcmToken Token { get; set; } = null!;

        [ForeignKey("UserId")]
        public virtual ApplicationUser? User { get; set; }
    }
}
