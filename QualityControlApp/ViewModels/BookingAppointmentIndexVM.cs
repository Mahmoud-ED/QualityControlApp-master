using Microsoft.AspNetCore.Mvc.Rendering;
using QualityControlApp.Models.Entities;

namespace QualityControlApp.ViewModels
{
    public class BookingAppointmentIndexVM
    {
        public List<BookingAppointment> Appointments { get; set; } = new List<BookingAppointment>();
        public SelectList? CompanySelectList { get; set; }

        // Optional: To retain filter values in the view after post/refresh
        public Guid? SelectedCompanyId { get; set; }
        public DateTime? SelectedBookingDate { get; set; }
    }
}
