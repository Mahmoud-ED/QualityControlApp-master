using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace QualityControlApp.Models.Entities
{
    public class BookingAppointment : BaseEntity
    {
        [Display(Name = "Company")]
        public Guid CompanyId { get; set; }
        [ValidateNever]

        public virtual Company Company { get; set; }

        [Required]
        [Display(Name = "BookingDate")]
        public DateTime BookingDate { get; set; }

        [MaxLength(1000)]
        public string? Notes { get; set; }

    }
}
