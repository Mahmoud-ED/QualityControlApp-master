using QualityControlApp.Models.Entities;

namespace QualityControlApp.ViewModels
{
    public class CompanyQuestionVM
    {

        public List<CompanyQuestion> CompanyQuestion { get; set; }
        public List<Company> Company { get; set; }

        public List<QuestionCategoryType> QuestionCategoryType { get; set; }
    }
}
