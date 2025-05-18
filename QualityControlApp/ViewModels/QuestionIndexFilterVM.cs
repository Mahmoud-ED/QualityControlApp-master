// في مجلد ViewModels أو Models
using QualityControlApp.Models.Entities;

public class QuestionIndexFilterVM
{
    public IEnumerable<Question> Questions { get; set; }
    public IEnumerable<QuestionType> AllQuestionTypes { get; set; } // لعرض أزرار الفلترة
    public Guid? CurrentQuestionTypeIdFilter { get; set; } // لتحديد الفلتر النشط
    public string PageTitle { get; set; }

    public QuestionIndexFilterVM()
    {
        Questions = new List<Question>();
        AllQuestionTypes = new List<QuestionType>();
    }
}