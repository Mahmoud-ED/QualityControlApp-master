using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QualityControlApp.Models.Entities
{
    public class FcmNotification
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [StringLength(1000)]
        public string Body { get; set; } = string.Empty;

        [StringLength(500)]
        public string? ImageUrl { get; set; }

        [StringLength(500)]
        public string? ClickAction { get; set; }

        [StringLength(100)]
        public string? Data { get; set; } // JSON data

        public NotificationType Type { get; set; } = NotificationType.General;

        public NotificationTarget Target { get; set; } = NotificationTarget.All;

        [StringLength(450)]
        public string? TargetUserId { get; set; } // For specific user

        [StringLength(100)]
        public string? TargetDeviceId { get; set; } // For specific device

        public bool IsScheduled { get; set; } = false;

        public DateTime? ScheduledAt { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? SentAt { get; set; }

        public NotificationStatus Status { get; set; } = NotificationStatus.Pending;

        [StringLength(1000)]
        public string? ErrorMessage { get; set; }

        public int SuccessCount { get; set; } = 0;

        public int FailureCount { get; set; } = 0;

        // Navigation properties
        [ForeignKey("TargetUserId")]
        public virtual ApplicationUser? TargetUser { get; set; }
    }

    public enum NotificationType
    {
        General = 0,
        System = 1,
        Marketing = 2,
        Alert = 3,
        Reminder = 4
    }

    public enum NotificationTarget
    {
        All = 0,
        AuthenticatedUsers = 1,
        AnonymousUsers = 2,
        SpecificUser = 3,
        SpecificDevice = 4,
        RoleBased = 5
    }

    public enum NotificationStatus
    {
        Pending = 0,
        Sent = 1,
        Failed = 2,
        Cancelled = 3
    }
}
