using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.Collections.Generic; // Required for List

namespace QualityControlApp.Models.Entities
{
    public class CompanyType : BaseEntity
    {
        public string Name { get; set; }

        [ValidateNever]
        public List<Company>? Company { get; set; } // This is a one-to-many with Company

        // New navigation property for the many-to-many relationship
        // This represents all the QuestionCategoryTypes available for this CompanyType
        [ValidateNever]
        public List<CompanyTypeCategoryAvailable>? AvailableCategories { get; set; }

        // OR, if you prefer to directly access QuestionCategoryType and let EF Core handle the join table implicitly (more common in EF Core 6+ for pure many-to-many)
        // [ValidateNever]
        // public List<QuestionCategoryType>? QuestionCategories { get; set; }
    }
}