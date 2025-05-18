namespace QualityControlApp.Models.Entities
{
    public class CompanyQuestionContent : BaseEntity 
    {

        public Guid CompanyQuestionId { get; set; }
        public CompanyQuestion CompanyQuestion { get; set; }

        public Guid  QuestionId { get; set; }
        public Question Question { get; set; }

        public int? Score { get; set; } // الدرجة المتحصل عليها
        public string? Inspect { get; set; } // Ns Or s or NA
        public string? Nots { get; set; } // ملاحظات في حالة Ns
        public int? Level { get; set; } // درجة الخطورة
    }
}
