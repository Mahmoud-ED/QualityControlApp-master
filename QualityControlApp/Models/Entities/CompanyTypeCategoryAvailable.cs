using System.ComponentModel.DataAnnotations.Schema; // For ForeignKey attribute
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation; // For ValidateNever if needed

namespace QualityControlApp.Models.Entities
{
    public class CompanyTypeCategoryAvailable : BaseEntity
    {
        // Foreign Key for CompanyType
        public Guid  CompanyTypeId { get; set; }

        // Navigation property to CompanyType
        [ForeignKey("CompanyTypeId")]
        [ValidateNever] // Usually, you don't validate navigation properties directly in the join entity
        public CompanyType CompanyType { get; set; }


        // Foreign Key for QuestionCategoryType
        public Guid QuestionCategoryTypeId { get; set; }

        // Navigation property to QuestionCategoryType
        [ForeignKey("QuestionCategoryTypeId")]
        [ValidateNever]
        public QuestionCategoryType QuestionCategoryType { get; set; }

        // You can add other properties specific to this relationship if needed
        // For example, an "IsActive" flag for this specific link,
        // or a "DisplayOrder" if the order of categories for a company type matters.
        // public bool IsActive { get; set; } = true;
        // public int DisplayOrder { get; set; }
    }
}