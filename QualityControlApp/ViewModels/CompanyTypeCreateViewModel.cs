using QualityControlApp.Models.Entities;
using Microsoft.AspNetCore.Mvc.Rendering; // For SelectListItem if using dropdowns
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace QualityControlApp.ViewModels // Or your preferred ViewModel namespace
{
    public class CompanyTypeCreateViewModel
    {
        [Required]
        [Display(Name = "Company Type Name")]
        public string Name { get; set; }

        // To hold all available QuestionCategoryTypes for selection
        public List<QuestionCategoryTypeViewModel> AvailableCategories { get; set; }

        // To hold the IDs of the selected QuestionCategoryTypes
        public List<Guid > SelectedCategoryIds { get; set; }

        public CompanyTypeCreateViewModel()
        {
            AvailableCategories = new List<QuestionCategoryTypeViewModel>();
            SelectedCategoryIds = new List<Guid>();
        }
    }

    // A simple DTO for displaying category information in the view
    public class QuestionCategoryTypeViewModel
    {
        public Guid Id { get; set; }
        public string CategoryName { get; set; }
        public bool IsSelected { get; set; } // Useful for checkboxes, especially on Edit
    }
}