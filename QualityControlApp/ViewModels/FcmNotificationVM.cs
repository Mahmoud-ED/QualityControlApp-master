using QualityControlApp.Models.Entities;
using System.ComponentModel.DataAnnotations;

namespace QualityControlApp.ViewModels
{
    public class FcmNotificationVM
    {
        [Required(ErrorMessage = "Title is required")]
        [StringLength(200, ErrorMessage = "Title cannot exceed 200 characters")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Body is required")]
        [StringLength(1000, ErrorMessage = "Body cannot exceed 1000 characters")]
        public string Body { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Image URL cannot exceed 500 characters")]
        [Url(ErrorMessage = "Please enter a valid URL")]
        public string? ImageUrl { get; set; }

        [StringLength(500, ErrorMessage = "Click Action cannot exceed 500 characters")]
        public string? ClickAction { get; set; }

        [StringLength(1000, ErrorMessage = "Data cannot exceed 1000 characters")]
        public string? Data { get; set; }

        public NotificationType Type { get; set; } = NotificationType.General;

        public NotificationTarget Target { get; set; } = NotificationTarget.All;

        [StringLength(450)]
        public string? TargetUserId { get; set; }

        [StringLength(100)]
        public string? TargetDeviceId { get; set; }

        public bool IsScheduled { get; set; } = false;

        public DateTime? ScheduledAt { get; set; }

        // For test page
        public List<string> SelectedTokens { get; set; } = new List<string>();
        public List<string> SelectedUserIds { get; set; } = new List<string>();
        public List<string> SelectedDeviceIds { get; set; } = new List<string>();
    }
}
