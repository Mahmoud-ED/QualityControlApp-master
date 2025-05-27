using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PasswordGenerator;
using QualityControlApp.Classes;
using QualityControlApp.Models.Entities;
using QualityControlApp.Models.Interfaces;
using QualityControlApp.ViewModels;
using QualityControlApp.ViewModels.Identity;
using SkiaSharp;

namespace QualityControlApp.Controllers
{
    [Authorize(Roles = "Prog,Admin")]
    [ViewLayout("_LayoutDashboard")]
    public class employeesController : BaseController
    {
        private readonly IUnitOfWork<Employee> _employee;
        private readonly IUnitOfWork<HealthRecord> _healthRecord;
        private readonly IUnitOfWork<ChronicDisease> _chronicDisease;
        private readonly IEmailSender _emailSender;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public employeesController(IUnitOfWork<Employee> employee,
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

        public async Task<IActionResult> Employees()
        {
            var employees = await _employee.Entity.GetAll().Include(e => e.ApplicationUser).ToListAsync();

            return View(employees);
        }

        public IActionResult CreateEmployee()
        {

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateEmployee(CreateEmployeeVM createEmployeeVM)
        {
            if (!ModelState.IsValid)
            {
                return View(createEmployeeVM);
            }

            var user = new ApplicationUser
            {
                UserName = createEmployeeVM.Email,
                Email = createEmployeeVM.Email,
                Age = createEmployeeVM.Age,
                Approval = false,
                CreatedDate = DateTime.UtcNow
            };

            var password = new Password(true, true, true, true, 5);
            string generatedPassword = password.Next();
            var hasher = new PasswordHasher<ApplicationUser>();
            user.PasswordHash = hasher.HashPassword(user, generatedPassword);

            var result = await _userManager.CreateAsync(user);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View(createEmployeeVM);
            }


            //-------------------------Add Employee row------------------------------------
            var employee = new Employee
            {
                Name = createEmployeeVM.Employee.Name,
                PhoneNumber = createEmployeeVM.Employee.PhoneNumber,
                Address = createEmployeeVM.Employee.Address,
                YearsOfExperience = createEmployeeVM.Employee.YearsOfExperience,
                Specialization = createEmployeeVM.Employee.Specialization,
                Bio = createEmployeeVM.Employee.Bio,
                UserId = user.Id,
                Created = DateTime.UtcNow
            };

            try
            {
                _employee.Entity.Insert(employee);
                await _employee.SaveAsync();
            }
            catch
            {
                return View("Error");
            }

            //-------------------------Add Role EmployeeRequest------------------------------------
            if (!await _roleManager.RoleExistsAsync("EmployeeRequest "))
            {
                var employeeRequestRole = new IdentityRole
                {
                    Name = "EmployeeRequest ",
                    ConcurrencyStamp = Guid.NewGuid().ToString()
                };

                if (!(await _roleManager.CreateAsync(employeeRequestRole)).Succeeded)
                {
                    TempData["ErrorMessage"] = "The Role: '" + employeeRequestRole.Name + "' failed to be added";
                    return View("NotFound");
                }
            }

            //-----------------------Add User To Role Employee-------------------------------
            if (!(await _userManager.AddToRoleAsync(user, "EmployeeRequest")).Succeeded)
            {
                TempData["ErrorMessage"] = "Failed to add user to Role: 'EmployeeRequest'";
                return View("NotFound");
            }
            //------------------------Send email---------------------------------------------
            {
                var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var confirmationLink = Url.Action("EmailConfirm", "Account", new { userId = user.Id, token = token }, Request.Scheme);

                string content = ReadHtmlTemplate("ConfirmEmailWithPassword.html");
                string subject = "Email Confirmation";
                content = content.Replace("{Subject}", subject);
                content = content.Replace("{UserName}", createEmployeeVM.Email);
                content = content.Replace("{Password}", generatedPassword);
                content = content.Replace("{confirmationLink}", confirmationLink);

                var message = new Message(new string[] { createEmployeeVM.Email }, subject, content, null);

                try
                {
                    await _emailSender.SendEmailAsync(message);
                    TempData["SuccessMessage"] = "The email has been sent successfully";
                    return RedirectToAction("Employees");
                }
                catch
                {
                    TempData["ErrorMessage"] = "Failed to send email";
                    return View(createEmployeeVM);
                }
            }

        }


        public async Task<IActionResult> EditEmployee(Guid? id)
        {
            if (id == null)
            {
                return View("NotFound");
            }

            var employee = await _employee.Entity.GetWhere(e => e.Id == id)
                                                 .Include(u => u.ApplicationUser)
                                                 .FirstOrDefaultAsync();
            if (employee == null)
            {
                ViewBag.ErrorMessage = $"Cannot be found employee with Id={id}";
                return View("NotFound");
            }

            return View(employee);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        //[Authorize(Policy = "EditUserPolicy")]
        public async Task<IActionResult> EditEmployee([Bind("Id,Name,PhoneNumber,Address,YearsOfExperience,Specialization,Bio,UserId,Created")] Employee employee)
        {
            //المستخدم ب البوتون Id لا نحتاج له في التعديل ولكن نحتاج له عند ارجاعه الى نفس الصفحة لعرض ايميل المستخدم وربط ApplicationUser=null لأنه سيرجع
            //-----------------------------------------
            var applicationUser = await _userManager.FindByIdAsync(employee.UserId);
            if (applicationUser == null)
            {
                ViewBag.ErrorMessage = $"Cannot be found User with Id={employee.UserId}";
                return View("NotFound");
            }

            //var applicationUser = await _userManager.Users.Include(p => p.UserProfile)
            //                 .Where(u => u.Id == employee.UserId)
            //                 .FirstOrDefaultAsync();

            employee.ApplicationUser = applicationUser;
            //--------------------------------------------------------------------

            if (!ModelState.IsValid)
            {
                return View(employee);
            }

            var name = await _employee.Entity.GetWhere(e => e.Id != employee.Id & e.Name == employee.Name).Select(e => e.Name).FirstOrDefaultAsync();
            if (name != null)
            {
                TempData["ErrorMessage"] = "The Employee name is reserved";
                return View(employee);
            }

            try
            {
                employee.Modified = DateTime.UtcNow;
                _employee.Entity.Update(employee);
                await _employee.SaveAsync();
            }
            catch (Exception)
            {
                return View("Error");
            }

            TempData["SuccessMessage"] = "Saved successfully";
            return RedirectToAction("EditEmployee", new { employee.Id });
        }




        //[Authorize(Policy = "EditUserPolicy")]
        public async Task<IActionResult> ApprovalEmployee(string? userId)
        {
            if (userId == null)
            {
                return View("NotFound");
            }

            var user = await _userManager.Users.Where(u => u.Id == userId)
                                               .Include(e=>e.Employee)
                                               .FirstOrDefaultAsync();
            if (user == null)
            {
                ViewBag.ErrorMessage = $"Cannot be found User with Id={userId}";
                return View("NotFound");
            }

           
            user.Approval =true;

            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                //-------------------------Add Role Employee------------------------------------
                if (!await _roleManager.RoleExistsAsync("Employee"))
                {
                    var employeeRole = new IdentityRole
                    {
                        Name = "Employee",
                        ConcurrencyStamp = Guid.NewGuid().ToString()
                    };

                    if (!(await _roleManager.CreateAsync(employeeRole)).Succeeded)
                    {
                        ViewBag.ErrorMessage = "The Role: '" + employeeRole.Name + "' failed to be added";
                        return View("NotFound");
                    }
                }

                //-----------------------Add User To Role Employee-------------------------------
                if (!await _userManager.IsInRoleAsync(user, "Employee"))
                {
                    if (!(await _userManager.AddToRoleAsync(user, "Employee")).Succeeded)
                    {
                        ViewBag.ErrorMessage = "Failed to add user to Role: 'Employee'";
                        return View("Error");
                    }
                }
                //------------------------------------------------------------------------------
                if (await _userManager.IsInRoleAsync(user, "EmployeeRequest"))
                {
                    if (!(await _userManager.RemoveFromRoleAsync(user, "EmployeeRequest")).Succeeded)
                    {
                        ViewBag.ErrorMessage = "Failed to add user to Role: 'Employee'";
                        return View("Error");
                    }
                }
                    
                //------------------------------------------------------------------------------


                TempData["SuccessMessage"] = "Approval has been given to the employee";
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    //ModelState.AddModelError(string.Empty, error.Description); // RedirectToAction لا تظهر لأنه لا يبقى في نفس الصفحة لأنه يقوم بتحميل الصفحة من جديد عن طريق
                    TempData["ErrorMessage"] = error.Description;
                }
            }


            var refererUrl = HttpContext.Request.Headers["Referer"].ToString(); // جلب اللنك الكامل للصفحة السابقة التي تم الاستدعاء منها
            if (refererUrl != null && refererUrl.Contains("EditEmployee"))
            {
                return RedirectToAction("EditEmployee", new { id = user.Employee.Id });
            }
            else if (refererUrl != null && refererUrl.Contains("Employees"))
            {
                return RedirectToAction("Employees");
            }
            else
            {
                return View("NotFound");
            }


        }
        public async Task<IActionResult> PrintEmployeeCard(Guid id)
        {
            var employee = await _employee.Entity.Include(e => e.ApplicationUser).Where(i => i.Id == id).FirstOrDefaultAsync();
            if (employee == null)
            {
                return NotFound();
            }
            // يمكنك إنشاء ViewModel إذا كنت تريد تمرير بيانات إضافية
            return View("PrintEmployeeCard", employee);
        }

        // Action to display multiple employee cards for printing (if needed)
        public async Task<IActionResult> PrintAllEmployeeCards()
        {
            var employees = await _employee.Entity.Include(e => e.ApplicationUser)
                                          .ToListAsync();
            return View("PrintMultipleEmployeeCards", employees); // استخدم View منفصل إذا كان التخطيط مختلفًا
        }


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
            var chronicDiseases =  _chronicDisease.Entity.GetAll(); // Assuming GetAllAsync exists

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

        [HttpGet]
        public async Task<IActionResult> CreateHealthRecordForEmployee(Guid employeeId)
        {
            if (employeeId == Guid.Empty)
            {
                TempData["error"] = "معرف الموظف غير صالح.";
                return RedirectToAction("Index", "Employees"); // Or appropriate error page
            }

            var employee = await _employee.Entity.GetByIdAsync(employeeId); //_context.Employees.FindAsync(employeeId);
            if (employee == null)
            {
                TempData["error"] = "الموظف غير موجود.";
                return RedirectToAction("Index", "Employees");
            }

            var chronicDiseases = _chronicDisease.Entity.GetAll(); //_context.ChronicDiseases.OrderBy(cd => cd.Name).ToListAsync();

            var viewModel = new CreateHealthRecordViewModel
            {
                HealthRecord = new HealthRecord
                {
                    EmployeeId = employeeId,
                    DiagnosisDate = DateTime.Today // Default diagnosis date
                },
                EmployeeName = employee.Name,
                ChronicDiseaseOptions = chronicDiseases.Select(cd => new SelectListItem
                {
                    Value = cd.Id.ToString(),
                    Text = cd.Name
                }).ToList()
            };

            return View("CreateHealthRecord", viewModel); // Specify view name if action name differs
        }

        // POST: Employees/CreateHealthRecordForEmployee (or HealthRecords/Create)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateHealthRecordForEmployee(CreateHealthRecordViewModel viewModel)
        {
            // Remove navigation properties from ModelState if they cause issues
            // and you are not intending to validate them fully here.
            ModelState.Remove("HealthRecord.Employee");
            ModelState.Remove("HealthRecord.ChronicDisease");


            if (ModelState.IsValid) // Validates viewModel.HealthRecord based on its DataAnnotations
            {
                try
                {
                    // Ensure EmployeeId is set if somehow it wasn't bound (though hidden field should handle it)
                    if (viewModel.HealthRecord.EmployeeId == Guid.Empty)
                    {
                        // This case should ideally not happen if the form is set up correctly
                        TempData["error"] = "فقدان معرف الموظف عند الإرسال.";
                        // Repopulate and return
                        var employeeForName = await _employee.Entity.GetByIdAsync(viewModel.HealthRecord.EmployeeId); // This line might be problematic if EmployeeId is empty
                        viewModel.EmployeeName = employeeForName?.Name ?? "غير معروف";
                        var chronicDiseasesList = _chronicDisease.Entity.GetAll();
                        viewModel.ChronicDiseaseOptions = chronicDiseasesList.Select(cd => new SelectListItem
                        {
                            Value = cd.Id.ToString(),
                            Text = cd.Name
                        }).ToList();
                        return View("CreateHealthRecord", viewModel);
                    }


                    _healthRecord.Entity.Insert(viewModel.HealthRecord);
                    await _healthRecord.SaveAsync();
                    TempData["success"] = $"تم إضافة سجل صحي للموظف بنجاح.";
                    // Redirect to the employee's details page
                    return RedirectToAction("Details", "Employees", new { id = viewModel.HealthRecord.EmployeeId });
                }
                catch (Exception ex)
                {
                    // Log the exception (ex)
                    TempData["error"] = "حدث خطأ أثناء إضافة السجل الصحي.";
                    // Fall through to re-render the form with error
                }
            }
            else
            {
                TempData["error"] = "فشل إضافة السجل الصحي. يرجى مراجعة البيانات المدخلة.";
            }

            // If ModelState is invalid or an error occurred, repopulate necessary data for the view model
            var employee = await _employee.Entity.GetByIdAsync(viewModel.HealthRecord.EmployeeId); //_context.Employees.FindAsync(viewModel.HealthRecord.EmployeeId);
            viewModel.EmployeeName = employee?.Name ?? "غير معروف"; // Handle if employee somehow not found

            var chronicDiseases =  _chronicDisease.Entity.GetAll(); //_context.ChronicDiseases.OrderBy(cd => cd.Name).ToListAsync();
            viewModel.ChronicDiseaseOptions = chronicDiseases.Select(cd => new SelectListItem
            {
                Value = cd.Id.ToString(),
                Text = cd.Name
            }).ToList();
            // The viewModel.HealthRecord already contains the user's input and validation errors

            return View("CreateHealthRecord", viewModel);
        }
    }



}
