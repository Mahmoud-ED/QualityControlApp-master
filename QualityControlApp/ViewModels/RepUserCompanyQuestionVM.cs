using QualityControlApp.Models.Entities;

namespace QualityControlApp.ViewModels
{
    public class RepUserCompanyQuestionVM
    {
        public string ChartImg1 { get; set; }
        public string ChartImg2 { get; set; }
        public string CompanyName { get; set; }
        public string UserName { get; set; }
        public string CreateDate { get; set; }

        public CompanyQuestion CreateQuestion { get; set; }
        public List<QuestionCategoryType> lstQuestionCategoryType { get; set; }
        public List<CompanyQuestionContent> lstCompanyQuestionContent { get; set; }
         
    }
}
