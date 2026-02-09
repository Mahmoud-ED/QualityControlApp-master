// QualityControlApp.Models.Entities.HealthRecordMedication.cs

using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QualityControlApp.Models.Entities
{
    /// <summary>
    /// نموذج ربط الدواء بالسجل الصحي - يمثل دواء موصوف لحالة صحية معينة
    /// </summary>
    public class HealthRecordMedication : BaseEntity
    {
        // علاقة مع السجل الصحي
        [Required(ErrorMessage = "السجل الصحي مطلوب")]
        [Display(Name = "السجل الصحي")]
        public Guid HealthRecordId { get; set; }

        [ValidateNever]
        [ForeignKey("HealthRecordId")]
        public virtual HealthRecord HealthRecord { get; set; }

        // علاقة مع الدواء
        [Required(ErrorMessage = "الدواء مطلوب")]
        [Display(Name = "الدواء")]
        public Guid MedicineId { get; set; }

        [ValidateNever]
        [ForeignKey("MedicineId")]
        public virtual Medicine Medicine { get; set; }

        [Required(ErrorMessage = "الجرعة مطلوبة")]
        [StringLength(100, ErrorMessage = "يجب ألا تتجاوز الجرعة 100 حرف")]
        [Display(Name = "الجرعة (مثل: 500mg، 10ml)")]
        public string Dosage { get; set; }

        [Required(ErrorMessage = "التكرار مطلوب")]
        [StringLength(100, ErrorMessage = "يجب ألا يتجاوز التكرار 100 حرف")]
        [Display(Name = "التكرار (مثل: مرتين يومياً، كل 8 ساعات)")]
        public string Frequency { get; set; }

        [Display(Name = "تاريخ البدء")]
        [DataType(DataType.Date)]
        public DateTime? StartDate { get; set; }

        [Display(Name = "تاريخ الانتهاء")]
        [DataType(DataType.Date)]
        public DateTime? EndDate { get; set; }

        [StringLength(500, ErrorMessage = "يجب ألا تتجاوز التعليمات 500 حرف")]
        [Display(Name = "تعليمات الاستخدام")]
        public string? Instructions { get; set; }

        [Display(Name = "نشط حالياً")]
        public bool IsActive { get; set; } = true;

        [StringLength(500, ErrorMessage = "يجب ألا تتجاوز الملاحظات 500 حرف")]
        [Display(Name = "ملاحظات")]
        public string? Notes { get; set; }
    }
}
