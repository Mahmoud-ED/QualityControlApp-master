using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace QualityControlApp.Models.Entities
{
    public class Company : BaseEntity 
    {

        public string Name { get; set; }
       
        [ValidateNever]
        public List <CompanyQuestion>? CompanyQuestions { get; set; }
    }
}
