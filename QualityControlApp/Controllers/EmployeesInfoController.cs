using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QualityControlApp.Models.Entities;
using QualityControlApp.Models.Interfaces;
using QualityControlApp.ViewModels;

namespace QualityControlApp.Controllers
{
    public class EmployeesInfoController :  BaseController
    {

        private readonly IUnitOfWork<Employee> _employee;
        private readonly IUnitOfWork<HealthRecord> _healthRecord;
        private readonly IUnitOfWork<ChronicDisease> _chronicDisease;
        private readonly IEmailSender _emailSender;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public EmployeesInfoController(IUnitOfWork<Employee> employee,
            IUnitOfWork<HealthRecord> healthRecord,
            IUnitOfWork<ChronicDisease> ChronicDisease,
                                   IWebHostEnvironment host,
                      
                                   IEmailSender emailSender,
                                   UserManager<ApplicationUser> userManager,
                                   SignInManager<ApplicationUser> signInManager,
                                   RoleManager<IdentityRole> roleManager) : base(host)
        {
            _healthRecord = healthRecord;
            _chronicDisease = ChronicDisease;
            _employee = employee;
            _emailSender = emailSender;
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }

        [AllowAnonymous]
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound("Employee ID is missing.");
            }

            var employee = await _employee.Entity.GetByIdAsync(id.Value);

            if (employee == null)
            {
                TempData["error"] = "الموظف غير موجود.";
                return RedirectToAction(nameof(Index)); // Or your employee list action
            }

            var healthRecords = await _healthRecord.Entity
                                        .Include(hr => hr.ChronicDisease) // To display ChronicDisease.Name
                                        .Where(hr => hr.EmployeeId == id)
                                        .OrderByDescending(hr => hr.DiagnosisDate)
                                        .ToListAsync();

            // Fetch Chronic Diseases for the dropdown in the "Add Health Record" form
            var chronicDiseases = _chronicDisease.Entity.GetAll(); // Assuming GetAllAsync exists

            var viewModel = new EmployeeDetailsViewModel
            {
                Employee = employee,
                HealthRecords = healthRecords,
                NewHealthRecord = new HealthRecord { EmployeeId = id.Value, DiagnosisDate = DateTime.Today }, // Pre-fill EmployeeId and a default date
                ChronicDiseaseOptions = chronicDiseases.Select(cd => new SelectListItem
                {
                    Value = cd.Id.ToString(),
                    Text = cd.Name
                }).ToList()
            };

            return View("Details", viewModel);
        }
    }
}
