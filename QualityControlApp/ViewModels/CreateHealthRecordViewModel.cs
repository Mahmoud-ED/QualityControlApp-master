// QualityControlApp.ViewModels.CreateHealthRecordViewModel.cs
using QualityControlApp.Models.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations; // If you need VM-specific annotations

namespace QualityControlApp.ViewModels
{
    public class CreateHealthRecordViewModel
    {
        public Guid Id { get; set; } // To display whose record is being added
        public HealthRecord HealthRecord { get; set; }
        public string EmployeeName { get; set; } // To display whose record is being added
        public IEnumerable<SelectListItem> ChronicDiseaseOptions { get; set; }
        public IEnumerable<HealthRecord> HealthRecords { get; set; }

        public CreateHealthRecordViewModel()
        {
            HealthRecord = new HealthRecord();
            ChronicDiseaseOptions = new List<SelectListItem>();
            HealthRecords = new List<HealthRecord>();
        }
    }
}