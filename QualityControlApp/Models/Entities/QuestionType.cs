using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations.Schema;

namespace QualityControlApp.Models.Entities
{
    public class QuestionType : BaseEntity 
    {
        public string TypeName { get; set; }
        public List <Question>? Questions { get; set; }

        [NotMapped]
        public int QuestionsCount { get; set; }


          public Guid QuestionCategoryTypeId { get; set; }

        [ValidateNever]
        public QuestionCategoryType QuestionCategoryType { get; set; }

    }
}
