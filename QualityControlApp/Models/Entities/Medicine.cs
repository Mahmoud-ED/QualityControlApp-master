// QualityControlApp.Models.Entities.Medicine.cs

using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace QualityControlApp.Models.Entities
{
    /// <summary>
    /// نموذج الدواء - يمثل دواء في النظام
    /// </summary>
    public class Medicine : BaseEntity
    {
        [Required(ErrorMessage = "اسم الدواء مطلوب")]
        [StringLength(200, ErrorMessage = "يجب ألا يتجاوز اسم الدواء 200 حرف")]
        [Display(Name = "اسم الدواء")]
        public string Name { get; set; }

        [StringLength(100, ErrorMessage = "يجب ألا يتجاوز الاسم العلمي 100 حرف")]
        [Display(Name = "الاسم العلمي (Generic Name)")]
        public string? GenericName { get; set; }

        [StringLength(100, ErrorMessage = "يجب ألا يتجاوز اسم الشركة المصنعة 100 حرف")]
        [Display(Name = "الشركة المصنعة")]
        public string? Manufacturer { get; set; }

        [StringLength(50, ErrorMessage = "يجب ألا يتجاوز نوع الدواء 50 حرف")]
        [Display(Name = "نوع الدواء (أقراص، شراب، حقن، إلخ)")]
        public string? Type { get; set; } // مثل: أقراص، شراب، حقن، كبسولات

        [StringLength(1000, ErrorMessage = "يجب ألا يتجاوز الوصف 1000 حرف")]
        [Display(Name = "وصف الدواء")]
        public string? Description { get; set; }

        [Display(Name = "متوفر في المخزون")]
        public bool IsAvailable { get; set; } = true;

        [StringLength(500, ErrorMessage = "يجب ألا تتجاوز الملاحظات 500 حرف")]
        [Display(Name = "ملاحظات")]
        public string? Notes { get; set; }

        // علاقة مع السجلات الصحية (الدواء الواحد يمكن أن يكون في عدة سجلات صحية)
        [ValidateNever]
        public virtual ICollection<HealthRecordMedication> HealthRecordMedications { get; set; }

        public Medicine()
        {
            HealthRecordMedications = new HashSet<HealthRecordMedication>();
        }
    }
}
