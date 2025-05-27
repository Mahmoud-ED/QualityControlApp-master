// QualityControlApp.Models.Entities.ChronicDisease.cs

using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation; // لـ ValidateNever إذا أضفت collections

namespace QualityControlApp.Models.Entities
{
    public class ChronicDisease : BaseEntity
    {
        [Required(ErrorMessage = "اسم المرض المزمن مطلوب")]
        [StringLength(150, ErrorMessage = "يجب ألا يتجاوز اسم المرض 150 حرفًا")]
        [Display(Name = "اسم المرض المزمن")]
        public string Name { get; set; }

        [StringLength(500)]
        [Display(Name = "وصف (اختياري)")]
        public string? Description { get; set; } // وصف اختياري للمرض

        // علاقة بالملف الصحي (المرض الواحد يمكن أن يكون في العديد من سجلات الملف الصحي)
        [ValidateNever]
        public virtual ICollection<HealthRecord> HealthRecords { get; set; }

        // Constructor لتهيئة الـ Collections
        public ChronicDisease()
        {
            HealthRecords = new HashSet<HealthRecord>();
        }
    }
}