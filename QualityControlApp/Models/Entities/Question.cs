using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace QualityControlApp.Models.Entities
{
    public class Question : BaseEntity
    {
        public string Content { get; set; }
        public Guid QuestionTypeId { get; set; }

        [ValidateNever]
        public QuestionType QuestionType { get; set; }
        public List <CompanyQuestionContent>? CompanyQuestionContents { get; set; }

        [Range(0, 4)]
        public int  MaxGrid { get; set; }

        public string? SectionContent { get; set; }



    }
}
