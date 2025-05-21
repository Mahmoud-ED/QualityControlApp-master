using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic; // For ICollection
using System; // For Guid

namespace QualityControlApp.Models.Entities
{
    public class CompanyQuestion : BaseEntity
    {
        public Guid CompanyId { get; set; }
        [ValidateNever]
        public virtual Company Company { get; set; }

      
        public string? CreatorId { get; set; } // Renamed FK for clarity

        [ForeignKey("CreatorId")]
        [ValidateNever]
        public virtual ApplicationUser Creator { get; set; } // Renamed navigation property for clarity, made virtual and nullable

        public Guid? UserId { get; set; } // Renamed FK for clarity




        [ValidateNever]
        public List<CompanyQuestionContent>? CompanyQuestionContents { get; set; }

        public Boolean Active { get; set; }
        public int SaftyGrid { get; set; }
        public int SqurtyGrid { get; set; }
        public String? Type { get; set; }

        public Guid? LocationId { get; set; }
        [ValidateNever]
        public Location? Location { get; set; }

        public int? Num { get; set; }

        public CompanyQuestion()
        {
            CompanyQuestionContents = new List<CompanyQuestionContent>();
        }
    }
}