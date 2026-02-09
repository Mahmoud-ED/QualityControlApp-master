using System.ComponentModel.DataAnnotations;

namespace QualityControlApp.Models.Entities
{
    public class SystemActivityLog : BaseEntity
    {
        [Required]
        public DateTime ActivityDate { get; set; }

        [Required]
        [StringLength(500)] // قد تحتاج وصفًا أطول
        public string Description { get; set; } // وصف النشاط/الإشعار

        [StringLength(100)]
        public string? ActivityType { get; set; } // نوع النشاط (مثال: NewRequest, StatusChange, UserLogin, SystemUpdate)

        public string? RelatedEntityType { get; set; } // (اختياري) نوع الكيان المرتبط (مثال: AirPortRequest, User, Landing)
        public string? RelatedEntityId { get; set; } // (اختياري) ID الكيان المرتبط (يمكن أن يكون Guid أو int أو string)
        public string? RelatedEntityDescription { get; set; } // (اختياري) وصف قصير للكيان المرتبط (مثال: Request #123)

        public string? UserId { get; set; } // (اختياري) ID المستخدم الذي تسبب في النشاط أو المرتبط به
        public string? UserName { get; set; } // (اختياري) اسم المستخدم

        public bool IsRead { get; set; } = false; // (اختياري) إذا كان هذا نظام إشعارات للمستخدمين، لتتبع ما إذا قرأه المستخدم أم لا
        public string? TargetUserGroupId { get; set; } // (اختياري) إذا كان الإشعار موجهًا لمجموعة معينة من المستخدمين

        public SystemActivityLog()
        {
            ActivityDate = DateTime.UtcNow;
        }
    }
}