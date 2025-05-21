using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using QualityControlApp.Models;
// using QualityControlApp.ViewModels; // أزلته مؤقتاً لأنه غير مستخدم في هذا السياق
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic; // Add this for ICollection, HashSet
using System; // Add this for DateTime

namespace QualityControlApp.Models.Entities
{
    public class ApplicationUser : IdentityUser
    {
        [Range(18, 100)]
        public int Age { get; set; }
        public DateTime? LastAccessTime { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }

        public bool? Approval { get; set; }

        [ValidateNever]
        public UserProfile? UserProfile { get; set; } // Made nullable for consistency

        [ValidateNever]
        public Employee? Employee { get; set; } // Made nullable for consistency

        // --- إزالة هذه الخاصية ---
        // [ValidateNever]
        // public CompanyQuestion CompanyQuestion { get; set; } // هذه الخاصية مربكة في سياق العلاقات الجديدة

        // --- الأسئلة التي أنشأها هذا المستخدم (الطرف "المجموعة" من علاقة واحد-لمجموعة) ---
        [ValidateNever]
        public virtual ICollection<CompanyQuestion> CreatedCompanyQuestions { get; set; } = new List<CompanyQuestion>();

        // --- الأسئلة التي تم تعيينها لهذا المستخدم (جزء من علاقة مجموعة-لمجموعة) ---

        [ValidateNever]
        public virtual ICollection<AirPortRequest> AirportRequest { get; set; }
        public virtual ICollection<Landing> Landing { get; set; }

        public string LastAccessTimeLocalTime
        {
            get { return LastAccessTime?.ToLocalTime().ToString("dd-MM-yyyy hh:mm:ss tt"); }
        }
        public string CreatedDateLocalTime
        {
            get { return CreatedDate.ToLocalTime().ToString("dd-MM-yyyy hh:mm:ss tt"); }
        }
        public string ModifiedDateLocalTime
        {
            get { return ModifiedDate?.ToLocalTime().ToString("dd-MM-yyyy hh:mm:ss tt"); }
        }

        public ApplicationUser()
        {
            CreatedCompanyQuestions = new HashSet<CompanyQuestion>();
            AirportRequest = new HashSet<AirPortRequest>();
            Landing = new HashSet<Landing>();
        }
    }
}