using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations.Schema;

namespace QualityControlApp.Models.Entities
{
    public class CompanyQuestion : BaseEntity 
    {


        public Guid  CompanyId { get; set; }
        [ValidateNever]

        public virtual  Company Company { get; set; }

        [ForeignKey("ApplicationUser")]
        public string? UserId { get; set; }
        [ValidateNever]
        public ApplicationUser ApplicationUser { get; set; }

        [ValidateNever]

        public List <CompanyQuestionContent>? CompanyQuestionContents { get; set; }

        public Boolean  Active { get; set; }
        public  int SaftyGrid { get; set; }
        public  int SqurtyGrid { get; set; }
        public  String? Type { get; set; } // هل هوا بنظام الاسئلة القديم او الجديد الي يبيه جمعة



    }
}
