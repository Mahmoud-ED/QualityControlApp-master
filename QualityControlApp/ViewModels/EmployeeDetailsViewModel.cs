using QualityControlApp.Models.Entities;
using Microsoft.AspNetCore.Mvc.Rendering; // For SelectListItem
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace QualityControlApp.ViewModels
{
    public class EmployeeDetailsViewModel
    {
        public Employee Employee { get; set; }
        public IEnumerable<HealthRecord> HealthRecords { get; set; }

        // For the "Add New Health Record" form
        public HealthRecord NewHealthRecord { get; set; }
        public IEnumerable<SelectListItem> ChronicDiseaseOptions { get; set; }

        public EmployeeDetailsViewModel()
        {
            HealthRecords = new List<HealthRecord>();
            NewHealthRecord = new HealthRecord();
            ChronicDiseaseOptions = new List<SelectListItem>();
        }
    }
}