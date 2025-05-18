using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace QualityControlApp.Models.Entities
{
    public class AirPortRequestFiles : BaseEntity
    {
        [Required(ErrorMessage = "The File Name field is required.")]
        [Display(Name = "File Name")]
        public string FileName { get; set; }

        [Required(ErrorMessage = "The File Path field is required.")]
        [Display(Name = "File Path")]
        public string FilePath { get; set; }

        // Relationship with AirPortRequest table (Many-to-One)
        [Required(ErrorMessage = "The Airport Request ID is required.")] // Added error message for clarity
        [Display(Name = "Airport Request ID")] // Added display name for clarity
        public Guid? AirPortRequestId { get; set; }

        [ForeignKey("AirPortRequestId")]
        public virtual AirPortRequest? AirPortRequest { get; set; } // Made navigation property nullable



        public Guid? LandingId { get; set; }

        [ForeignKey("LandingId")]
        public virtual Landing? Landing { get; set; } // 

        // Relationship with FileType table (Many-to-One)
        [Required(ErrorMessage = "The File Type ID is required.")] // Added error message for clarity
        [Display(Name = "File Type ID")] // Added display name for clarity
        public Guid FileTypeId { get; set; }

        [ForeignKey("FileTypeId")]
        public virtual FileType? FileType { get; set; } // Made navigation property nullable

        public  string?  Inspect { get; set; }
        public string? Nots { get; set; }


    }
}