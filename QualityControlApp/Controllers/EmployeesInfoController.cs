using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QualityControlApp.Models.Entities;
using QualityControlApp.Models.Interfaces;
using QualityControlApp.ViewModels;
using SkiaSharp;

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
                               IConfiguration configuration,
                      
                                   IEmailSender emailSender,
                                   UserManager<ApplicationUser> userManager,
                                   SignInManager<ApplicationUser> signInManager,
                                   RoleManager<IdentityRole> roleManager) : base(host, configuration)
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
        public async Task<IActionResult> Details(Guid id) // ?? ?? ??? ??????? ??? ID
        {
            if (id == Guid.Empty) // ?? ?? ???? ????? ??? ID
            {
                return NotFound();
            }

            var employee = await _employee.Entity
                                        .Include(e => e.ApplicationUser) // ??? ??? ???????
                                        .FirstOrDefaultAsync(m => m.Id == id);

            if (employee == null)
            {
                TempData["error"] = "?? ??? ?????? ??? ??????.";
                return RedirectToAction(nameof(Index)); // ?? ???? ??? ??????
            }

            // ??? ??????? ?????? ?????? ??????? ?????? ?? ????? ?????? ????? ??????
            var healthRecordsForEmployee = await _healthRecord.Entity
                                                .Include(hr => hr.ChronicDisease)
                                                .Include(hr => hr.HealthRecordMedications)
                                                    .ThenInclude(hrm => hrm.Medicine)
                                                .Where(hr => hr.EmployeeId == id)
                                                .ToListAsync();

            // >>> ????? ?????: ??? ???? ??????? ??????? ?? ????? ????????
            var allChronicDiseasesFromDb = await _chronicDisease.Entity.GetAll().OrderBy(cd => cd.Name).ToListAsync();

            var viewModel = new EmployeeDetailsViewModel
            {
                Employee = employee,
                HealthRecords = healthRecordsForEmployee,
                AllChronicDiseases = allChronicDiseasesFromDb, // << ????? ??????? ??? ViewModel
                                                               // NewHealthRecord ? ChronicDiseaseOptions ???? ????? ??? ??? ??? ????? ????? ??????? ?? ??? ??????
            };

            // ??? ??? ??????? ????? ?????? ?? ???? ChronicDiseaseOptions
            // viewModel.ChronicDiseaseOptions = allChronicDiseasesFromDb.Select(cd => new SelectListItem
            // {
            //     Value = cd.Id.ToString(),
            //     Text = cd.Name
            // }).ToList();

            return View(viewModel);
        }




    }
}

