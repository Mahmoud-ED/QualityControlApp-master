using Microsoft.AspNetCore.Mvc.Rendering; // For SelectListItem
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
        [Required(ErrorMessage = "Please select the creator user.")] // تم تغيير الرسالة للتوضيح
        [Display(Name = "Creator User")] // تم تغيير الاسم المعروض للتوضيح
        public Guid UserId { get; set; } // هذا للمستخدم المُنشئ
        public string CreatorId { get; set; } // هذا للمستخدم المُنشئ

        [Required(ErrorMessage = "Please select a company.")]
        [Display(Name = "Company")]
        public Guid CompanyId { get; set; }

        [Display(Name = "Location")] // جعلته اختياريًا بناءً على عدم وجود [Required]
        public Guid? LocationId { get; set; } // غيرته إلى nullable إذا كان يمكن أن يكون اختياريًا

        [Required]
        public string Type { get; set; }

        [Display(Name = "Question Categories")]
        [Required(ErrorMessage = "Please select at least one question category.")]
        [MinLength(1, ErrorMessage = "Please select at least one question category.")]
        public List<Guid> SelectedQuestionCategoryTypeIds { get; set; }

        public List<QuestionCategoryTypeSelectItemVM> AvailableQuestionCategoryTypes { get; set; }

        // --- إضافة الخصائص الجديدة للمستخدمين المعينين ---
        [Display(Name = "Assign Users")]
        public List<Guid> SelectedAssignedUserIds { get; set; } // لاستقبال IDs المستخدمين المعينين

        public List<SelectListItem> AvailableAssignedUsers { get; set; } // لتوفير قائمة المستخدمين للاختيار

        public CreateCompanyQuestionVM()
        {
            SelectedQuestionCategoryTypeIds = new List<Guid>();
            AvailableQuestionCategoryTypes = new List<QuestionCategoryTypeSelectItemVM>();
            SelectedAssignedUserIds = new List<Guid>(); // تهيئة القائمة
            AvailableAssignedUsers = new List<SelectListItem>(); // تهيئة القائمة
        }
    }
}