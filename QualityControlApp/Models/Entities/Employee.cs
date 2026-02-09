// QualityControlApp.Models.Entities.Employee.cs

using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QualityControlApp.Models.Entities
{
    // Enum للحالة الاجتماعية
    public enum MaritalStatus
    {
        [Display(Name = "أعزب/عزباء")]
        Single,
        [Display(Name = "متزوج/متزوجة")]
        Married,
        [Display(Name = "مطلق/مطلقة")]
        Divorced,
        [Display(Name = "أرمل/أرملة")]
        Widowed
    }

    // Enum للجنس
    public enum Gender
    {
        [Display(Name = "ذكر")]
        Male,
        [Display(Name = "أنثى")]
        Female
        // يمكنك إضافة Other إذا لزم الأمر
        // [Display(Name = "آخر")]
        // Other
    }

    public class Employee : BaseEntity
    {
        [Required(ErrorMessage = "اسم الموظف مطلوب")]
        [StringLength(100, ErrorMessage = "يجب ألا يتجاوز اسم الموظف 100 حرف")]
        [Display(Name = "الاسم")]
        public string Name { get; set; }

        [Display(Name = "رقم الهاتف")]
        // يمكنك إلغاء تعليق RegularExpression إذا كنت تريد تنسيقًا محددًا
        // [RegularExpression(@"^\d{9,15}$", ErrorMessage = "رقم الهاتف غير صالح")]
        public string PhoneNumber { get; set; }

        [StringLength(200, ErrorMessage = "يجب ألا يتجاوز العنوان 200 حرف")]
        [Display(Name = "العنوان")]
        public string Address { get; set; }

        [Range(0, 60, ErrorMessage = "سنوات الخبرة يجب أن تكون بين 0 و 60")]
        [Display(Name = "سنوات الخبرة")]
        public int YearsOfExperience { get; set; }

        [StringLength(100, ErrorMessage = "يجب ألا يتجاوز التخصص 100 حرف")]
        [Display(Name = "التخصص")]
        public string Specialization { get; set; }

        [StringLength(500, ErrorMessage = "يجب ألا تتجاوز النبذة التعريفية 500 حرف")]
        [Display(Name = "نبذة تعريفية")]
        public string Bio { get; set; }

        [ValidateNever]
        public ApplicationUser ApplicationUser { get; set; }

        [ForeignKey("ApplicationUser")]
        public string? UserId { get; set; } // يمكن أن يكون مرتبطاً بحساب مستخدم أو لا

        // --- الحقول الجديدة ---

        [StringLength(100, ErrorMessage = "يجب ألا يتجاوز اسم الأم 100 حرف")]
        [Display(Name = "اسم الأم")]
        public string? MotherName { get; set; } // جعلته اختياريًا، يمكنك جعله Required إذا لزم الأمر

        [Required(ErrorMessage = "تاريخ الميلاد مطلوب")]
        [DataType(DataType.Date)]
        [Display(Name = "تاريخ الميلاد")]
        // يمكنك إضافة تحقق مخصص للتأكد أن تاريخ الميلاد ليس في المستقبل
        public DateTime DateOfBirth { get; set; }

        [Required(ErrorMessage = "الحالة الاجتماعية مطلوبة")]
        [Display(Name = "الحالة الاجتماعية")]
        public MaritalStatus MaritalStatus { get; set; }

        [Required(ErrorMessage = "الجنس مطلوب")]
        [Display(Name = "الجنس")]
        public Gender Gender { get; set; }

        // --- نهاية الحقول الجديدة ---

        // علاقة بالملف الصحي (الموظف الواحد لديه العديد من سجلات الملف الصحي)
        [ValidateNever]
        public virtual ICollection<HealthRecord> HealthRecords { get; set; }

        // Constructor لتهيئة الـ Collections
        public Employee()
        {
            HealthRecords = new HashSet<HealthRecord>();
        }
    }
}