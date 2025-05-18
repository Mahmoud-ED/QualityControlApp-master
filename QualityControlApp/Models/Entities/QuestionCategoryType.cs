using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;


namespace QualityControlApp.Models.Entities
{
    public class QuestionCategoryType : BaseEntity
    {
        public string CategoryName { get; set; }

        public List<QuestionType>? QuestionType { get; set; }

        [NotMapped]
        public int TypeCount { get; set; }
        public string Type { get; set; } // هل بنظام القديم SPL and COL او بنظام الجديد 



    }
}
