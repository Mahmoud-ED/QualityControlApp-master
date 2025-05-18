using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace QualityControlApp.Models.Entities
{
    public class AirPortRequest : BaseEntity
    {
       

        [Required(ErrorMessage = "حقل الجهة مطلوب")]
        [Display(Name = "الجهة")]
        public string Department { get; set; }

        [Required(ErrorMessage = "حقل وقت الدخول مطلوب")]
        [Display(Name = "وقت الدخول")]
        public DateTime EntryTime { get; set; }

        [Required(ErrorMessage = "حقل وقت الطلب مطلوب")]
        [Display(Name = "وقت الطلب")]
        public DateTime RequestTime { get; set; }

        [Required(ErrorMessage = "حقل البريد الإلكتروني مطلوب")]
        [EmailAddress(ErrorMessage = "الرجاء إدخال بريد إلكتروني صحيح")]
        [Display(Name = "البريد الإلكتروني")]
        public string Email { get; set; }

        [Required(ErrorMessage = "حقل اسم المرسل مطلوب")]
        [Display(Name = "اسم المرسل")]
        public string SenderName { get; set; }

        [Required(ErrorMessage = "حقل اسم الشركة مطلوب")]
        [Display(Name = "اسم الشركة")]
        public string CompanyName { get; set; }

        [Required(ErrorMessage = "حقل تاريخ الرحلة مطلوب")]
        [Display(Name = "تاريخ الرحلة")]
        public DateTime FlightDate { get; set; }

        [Required(ErrorMessage = "حقل نوع الطائرة مطلوب")]
        [Display(Name = "نوع الطائرة")]
        public string AircraftType { get; set; }

        [Required(ErrorMessage = "حقل حروف تسجيل الطائرة مطلوب")]
        [Display(Name = "حروف تسجيل الطائرة")]
        public string AircraftRegistration { get; set; }

        [Required(ErrorMessage = "حقل علامة النداء مطلوب")]
        [Display(Name = "علامة النداء")]
        public string CallSign { get; set; }

        [Required(ErrorMessage = "حقل خط السير مطلوب")]
        [Display(Name = "خط السير")]
        public string FlightPath { get; set; }

        [Required(ErrorMessage = "حقل ساعة الهبوط والإقلاع مطلوب")]
        [Display(Name = "ساعة الهبوط والإقلاع")]
        public string LandingTakeoffTime { get; set; }

        [Required(ErrorMessage = "حقل الغرض من الرحلة مطلوب")]
        [Display(Name = "الغرض من الرحلة")]
        public string FlightPurpose { get; set; }

        [Required(ErrorMessage = "حقل نقاط دخول وخروج مطلوب")]
        [Display(Name = "نقاط دخول وخروج")]
        public string EntryExitPoints { get; set; }

        [Display(Name = "ملاحظات")]
        public string Notes { get; set; }

        [Required(ErrorMessage = "حقل حالة الطلب مطلوب")]
        [Display(Name = "حالة الطلب")]
        public string RequestStatus { get; set; }

        // العلاقة مع جدول المستخدمين
        [Display(Name = "المستخدم الذي قام بقبول الطلب")]

        [ForeignKey("ApproverUserId")]
        public ApplicationUser ApplicationUser { get; set; }

        [ForeignKey("ApplicationUser")]
        public string? ApproverUserId { get; set; }
        // العلاقة مع جدول المرفقات (One-to-Many)
        [ValidateNever]
        public virtual ICollection<AirPortRequestFiles> RequestFiles { get; set; }

       public string? PilotName { get; set; }
        public string? FlightNumber { get; set; }
        public string? EntryPoint { get; set; }
        public string? ExitPoint { get; set; }

        public DateTime? EstimatedEntryTime { get; set; }

        public DateTime? EstimatedExitTime { get; set; }
        public string? CargoDetails { get; set; }

        public int? CrewCount { get; set; }

        public string?    CrewNationalities { get; set; }



    }
}
