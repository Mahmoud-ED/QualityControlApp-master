using QualityControlApp.Models.Entities;

namespace QualityControlApp.ViewModels
{
    public class CompanyQuestionContentVM
    {
        public CompanyQuestion CompanyQuestion { get; set; }
        public List<CompanyQuestionContent> CompanyQuestionContent { get; set; }

        public List<QuestionType> QuestionType { get; set; }
        public List<QuestionCategoryType> QuestionCategoryType { get; set; }

    }
}
