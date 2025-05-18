using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace QualityControlApp.Models.Entities
{
    public class Company : BaseEntity 
    {
        public string Name { get; set; }
        public string? AocNum { get; set; }
       
        [ValidateNever]
        public List <CompanyQuestion>? CompanyQuestions { get; set; }
        public List<BookingAppointment>? BookingAppointment { get; set; }

        public Guid? CompanyTypeId { get; set; }
        [ValidateNever]

        public virtual CompanyType CompanyType { get; set; }
    }
}
