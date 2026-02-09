using QualityControlApp.Models.Entities;
using Microsoft.AspNetCore.Mvc.Rendering; // For SelectListItem
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace QualityControlApp.ViewModels
{
    public class EmployeeDetailsViewModel
    {
        public Employee Employee { get; set; }
        public IEnumerable<HealthRecord> HealthRecords { get; set; } // سجلات الموظف الحالية

        // >>> إضافة جديدة: قائمة بجميع الأمراض المزمنة المتاحة في النظام
        public IEnumerable<ChronicDisease> AllChronicDiseases { get; set; }

        // For the "Add New Health Record" form
        public HealthRecord NewHealthRecord { get; set; }
        public IEnumerable<SelectListItem> ChronicDiseaseOptions { get; set; }

        public EmployeeDetailsViewModel()
        {
            HealthRecords = new List<HealthRecord>();
            AllChronicDiseases = new List<ChronicDisease>(); // << تهيئة القائمة الجديدة
            NewHealthRecord = new HealthRecord();
            ChronicDiseaseOptions = new List<SelectListItem>();
        }
    }
}