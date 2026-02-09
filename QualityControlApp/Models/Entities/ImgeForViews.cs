using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QualityControlApp.Models.Entities
{
    public class ImgeForViews : BaseEntity
    {
        public string? ViewName { get; set; }

        [Display(Name = "Cover Image")]
        public string? CoverImageUrl { get; set; }

       
        [NotMapped]
        public IFormFile? CoverImage { get; set; }
    }
}
