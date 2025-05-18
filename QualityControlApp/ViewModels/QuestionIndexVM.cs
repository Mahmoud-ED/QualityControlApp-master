using QualityControlApp.Models.Entities;

namespace QualityControlApp.ViewModels
{
    public class QuestionIndexVM
    {
        public List<Question> Question { get; set; }
        public List<QuestionCategoryType> QuestionCategoryType { get; set; }
        public List<QuestionType> QuestionType { get; set; }


    }
}
