using System.ComponentModel.DataAnnotations;

namespace QualityControlApp.Models.Entities
{
    public class FileType : BaseEntity 
    {

      

            [Required(ErrorMessage = "حقل نوع الملف مطلوب")]
            [Display(Name = "نوع الملف")]
            public string TypeName { get; set; }
    }
}

