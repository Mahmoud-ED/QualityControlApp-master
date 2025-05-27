// QualityControlApp.Models.Entities.HealthRecord.cs

using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace QualityControlApp.Models.Entities
{
    public class HealthRecord : BaseEntity
    {
        // مفتاح أجنبي للموظف
        [Required(ErrorMessage = "معرف الموظف مطلوب")]
        public Guid EmployeeId { get; set; }

        [ForeignKey("EmployeeId")]
        [ValidateNever]
        [Display(Name = "الموظف")]
        public virtual Employee Employee { get; set; }

        // مفتاح أجنبي للمرض المزمن
        [Required(ErrorMessage = "معرف المرض المزمن مطلوب")]
        public Guid ChronicDiseaseId { get; set; }

        [ForeignKey("ChronicDiseaseId")]
        [ValidateNever]
        [Display(Name = "المرض المزمن")]
        public virtual ChronicDisease ChronicDisease { get; set; }

        [Required(ErrorMessage = "تاريخ تسجيل المرض مطلوب")]
        [DataType(DataType.Date)]
        [Display(Name = "تاريخ تسجيل المرض/التشخيص")]
        public DateTime DiagnosisDate { get; set; } // تاريخ تشخيص هذا المرض لهذا الموظف

        [StringLength(1000)]
        [Display(Name = "ملاحظات")]
        public string? Notes { get; set; } // ملاحظات إضافية حول هذا السجل الصحي المحدد
    }
}