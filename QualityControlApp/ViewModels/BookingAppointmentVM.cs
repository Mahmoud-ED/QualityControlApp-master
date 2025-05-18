using Microsoft.AspNetCore.Mvc.Rendering;
using QualityControlApp.Models.Entities;
using System.ComponentModel.DataAnnotations;

namespace QualityControlApp.ViewModels
{
    public class BookingAppointmentVM
    {
        public Guid Id { get; set; }
        [Display(Name = "Company")]
        [Required]
        public Guid CompanyId { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime BookingDate { get; set; }

        [MaxLength(1000)]
        public string? Notes { get; set; }

        public SelectList? CompanySelectList { get; set; }

    }

}
