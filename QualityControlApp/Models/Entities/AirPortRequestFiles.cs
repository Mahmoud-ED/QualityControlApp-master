using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace QualityControlApp.Models.Entities
{
    public class AirPortRequestFiles : BaseEntity
    {


        [Required(ErrorMessage = "حقل اسم المرفق مطلوب")]
        [Display(Name = "اسم المرفق")]
        public string FileName { get; set; }

        [Required(ErrorMessage = "حقل مسار المرفق مطلوب")]
        [Display(Name = "مسار المرفق")]
        public string FilePath { get; set; }

        // العلاقة مع جدول طلبات المطار (Many-to-One)
        [Required]
        public Guid AirPortRequestId { get; set; }

        [ForeignKey("AirPortRequestId")]
        public virtual AirPortRequest AirPortRequest { get; set; }

        // العلاقة مع نوع الملف (Many-to-One)
        [Required]
        public Guid FileTypeId { get; set; }

        [ForeignKey("FileTypeId")]
        public virtual FileType FileType { get; set; }


    }
}


