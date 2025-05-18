using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace QualityControlApp.ViewModels
{
    public class QuestionCategoryTypeSelectItemVM
    {
        public Guid Id { get; set; }
        public string CategoryName { get; set; }
    }

    public class CreateCompanyQuestionVM
    {
        [Required(ErrorMessage = "Please select a user.")]
        [Display(Name = "User")]
        public string UserId { get; set; }

        [Required(ErrorMessage = "Please select a company.")]
        [Display(Name = "Company")]
        public Guid CompanyId { get; set; }

        [Required(ErrorMessage = "Please select a location.")]
        [Display(Name = "Location")]
        public Guid LocationId { get; set; }

        [Required]
        public string Type { get; set; }

        [Display(Name = "Question Categories")]
        [Required(ErrorMessage = "Please select at least one question category.")]
        [MinLength(1, ErrorMessage = "Please select at least one question category.")]
        public List<Guid> SelectedQuestionCategoryTypeIds { get; set; }

        public List<QuestionCategoryTypeSelectItemVM> AvailableQuestionCategoryTypes { get; set; }

        public CreateCompanyQuestionVM()
        {
            SelectedQuestionCategoryTypeIds = new List<Guid>();
            AvailableQuestionCategoryTypes = new List<QuestionCategoryTypeSelectItemVM>();
        }
    }
}