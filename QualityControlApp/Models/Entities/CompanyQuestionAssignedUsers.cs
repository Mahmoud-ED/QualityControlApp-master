using QualityControlApp.Models.Entities;

public class CompanyQuestionAssignedUsers
{
    public Guid AssignedCompanyQuestionsId { get; set; }
    public string AssignedUsersId { get; set; }

    public virtual CompanyQuestion CompanyQuestion { get; set; }
    public virtual ApplicationUser User { get; set; }
}
