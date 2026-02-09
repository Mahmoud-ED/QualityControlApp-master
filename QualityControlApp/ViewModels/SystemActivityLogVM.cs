using System.ComponentModel.DataAnnotations;

namespace QualityControlApp.ViewModels
{
    public class SystemActivityLogVM
    {
        public Guid Id { get; set; } // للـ Edit

        [Required]
        [Display(Name = "Activity Date")]
        public DateTime ActivityDate { get; set; } = DateTime.UtcNow;

        [Required]
        [StringLength(500, ErrorMessage = "Description cannot be longer than 500 characters.")]
        public string Description { get; set; }

        [StringLength(100)]
        [Display(Name = "Activity Type")]
        public string? ActivityType { get; set; }

        [Display(Name = "Related Entity Type")]
        public string? RelatedEntityType { get; set; }

        [Display(Name = "Related Entity ID")]
        public string? RelatedEntityId { get; set; }

        [Display(Name = "Related Entity Description")]
        public string? RelatedEntityDescription { get; set; }

        [Display(Name = "User ID")]
        public string? UserId { get; set; }

        [Display(Name = "User Name")]
        public string? UserName { get; set; }

        // لا حاجة لـ IsRead أو TargetUserGroupId في نموذج إدارة بسيط
        // إلا إذا كنت تريد التحكم بها من هنا.
    }
}