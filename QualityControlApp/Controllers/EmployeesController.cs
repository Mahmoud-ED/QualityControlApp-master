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
                // It's generally better to return NotFoundResult() or a specific view
                // that informs the user the resource was not found, rather than a generic "NotFound" view
                // unless "NotFound" is specifically designed for this.
                return NotFound(); // Or return View("NotFound");
            }

            var employee = await _employee.Entity.GetWhere(e => e.Id == id)
                                                 .Include(u => u.ApplicationUser)
                                                 .FirstOrDefaultAsync();
            if (employee == null)
            {
                // Log this error for your reference
                // _logger.LogWarning($"Employee with Id={id} not found.");
                ViewBag.ErrorMessage = $"Cannot be found employee with Id={id}";
                return View("NotFound"); // Or return NotFound();
            }

            return View(employee);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        //[Authorize(Policy = "EditUserPolicy")]
        public async Task<IActionResult> EditEmployee(
            [Bind("Id,Name,PhoneNumber,Address,YearsOfExperience,Specialization,Bio,UserId,Created,MotherName,DateOfBirth,MaritalStatus,Gender")] Employee employee)
        {
            if (string.IsNullOrEmpty(employee.UserId))
            {
            }
            else // Proceed only if UserId is present
            {
                var applicationUser = await _userManager.FindByIdAsync(employee.UserId);
                if (applicationUser == null)
                {
                    ViewBag.ErrorMessage = $"Cannot be found User with Id={employee.UserId}";
                    return View("NotFound"); // Or a specific error view
                }
                employee.ApplicationUser = applicationUser;
            }



            if (!ModelState.IsValid)
            {
                return View(employee);
            }

            var existingEmployeeWithSameName = await _employee.Entity
                .GetWhere(e => e.Id != employee.Id && e.Name == employee.Name)
                .FirstOrDefaultAsync(); // Fetch the entity or null

            if (existingEmployeeWithSameName != null)
            {
                TempData["ErrorMessage"] = "The Employee name is already in use by another employee.";
                return View(employee);
            }

            try
            {

                var employeeToUpdate = await _employee.Entity.GetWhere(e => e.Id == employee.Id).FirstOrDefaultAsync();
                if (employeeToUpdate == null)
                {
                    ViewBag.ErrorMessage = $"Cannot be found employee with Id={employee.Id} to update.";
                    return View("NotFound");
                }

                // Map the bound properties to the entity loaded from the database
                employeeToUpdate.Name = employee.Name;
                employeeToUpdate.PhoneNumber = employee.PhoneNumber;
                employeeToUpdate.Address = employee.Address;
                employeeToUpdate.YearsOfExperience = employee.YearsOfExperience;
                employeeToUpdate.Specialization = employee.Specialization;
                employeeToUpdate.Bio = employee.Bio;

                // New Fields
                employeeToUpdate.MotherName = employee.MotherName;
                employeeToUpdate.DateOfBirth = employee.DateOfBirth;
                employeeToUpdate.MaritalStatus = employee.MaritalStatus;
                employeeToUpdate.Gender = employee.Gender;

                employeeToUpdate.Modified = DateTime.UtcNow;

                _employee.Entity.Update(employeeToUpdate); // Update the tracked entity
                await _employee.SaveAsync();
            }
            catch (DbUpdateConcurrencyException ex) // Be more specific with exceptions
            {
                // Log the exception (ex)
                // Handle concurrency issues, e.g., inform the user data was changed by someone else.
                ModelState.AddModelError(string.Empty, "The record you attempted to edit "
                    + "was modified by another user after you got the original value. The "
                    + "edit operation was canceled. If you still want to edit this record, please "
                    + "reload the page.");
                // You'll need to repopulate employee.ApplicationUser if you return View(employee)
                // and it was lost or not part of employeeToUpdate correctly.
                var applicationUserForErrorView = await _userManager.FindByIdAsync(employee.UserId);
                employee.ApplicationUser = applicationUserForErrorView; // Ensure it's populated for the view
                return View(employee);
            }
            catch (Exception ex) // Catch more general exceptions
            {
                // Log the exception (ex)
                // ModelState.AddModelError(string.Empty, "An error occurred while saving the changes. Please try again.");
                // return View(employee); // Consider what to return. A generic error page might be better.
                ViewBag.ErrorMessage = "An error occurred while saving changes."; // Simple error message
                return View("Error"); // Generic error view
            }

            TempData["SuccessMessage"] = "Saved successfully";
            // It's important to repopulate ApplicationUser for the model passed back to the view after redirect,
            // if the EditEmployee GET action relies on it being pre-loaded.
            // However, RedirectToAction will trigger the GET action which re-fetches the employee with ApplicationUser.
            return RedirectToAction("EditEmployee", new { id = employee.Id }); // Use 'id' to match parameter name in GET
        }

        //[Authorize(Policy = "EditUserPolicy")]
        public async Task<IActionResult> ApprovalEmployee(string? userId)
        {
            if (userId == null)
            {
                return View("NotFound");
            }

            var user = await _userManager.Users.Where(u => u.Id == userId)
                                               .Include(e => e.Employee)
                                               .FirstOrDefaultAsync();
            if (user == null)
            {
                ViewBag.ErrorMessage = $"Cannot be found User with Id={userId}";
                return View("NotFound");
            }


            user.Approval = true;

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
                    //ModelState.AddModelError(string.Empty, error.Description); // RedirectToAction ?? ???? ???? ?? ???? ?? ??? ?????? ???? ???? ?????? ?????? ?? ???? ?? ????
                    TempData["ErrorMessage"] = error.Description;
                }
            }


            var refererUrl = HttpContext.Request.Headers["Referer"].ToString(); // ??? ????? ?????? ?????? ??????? ???? ?? ????????? ????
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateHealthRecord([Bind("Id,EmployeeId,ChronicDiseaseId,DiagnosisDate,Notes,Created")] HealthRecord model)
        {
            if (model == null || model.Id == Guid.Empty)
            {
                return NotFound();
            }

            var record = await _healthRecord.Entity.GetWhere(hr => hr.Id == model.Id).FirstOrDefaultAsync();
            if (record == null)
            {
                return NotFound();
            }

            record.ChronicDiseaseId = model.ChronicDiseaseId;
            record.DiagnosisDate = model.DiagnosisDate;
            record.Notes = model.Notes;
            record.Modified = DateTime.UtcNow;

            _healthRecord.Entity.Update(record);
            await _healthRecord.SaveAsync();

            TempData["success"] = "تم تحديث السجل الصحي بنجاح";
            return RedirectToAction("CreateHealthRecord", new { employeeId = record.EmployeeId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteHealthRecord(Guid id)
        {
            var record = await _healthRecord.Entity
                .GetWhere(hr => hr.Id == id)
                .Include(hr => hr.HealthRecordMedications)
                .FirstOrDefaultAsync();
            if (record == null)
            {
                return NotFound();
            }

            if (record.HealthRecordMedications != null && record.HealthRecordMedications.Any())
            {
                TempData["error"] = "لا يمكن حذف السجل الصحي لوجود أدوية مرتبطة به";
                return RedirectToAction("CreateHealthRecord", new { employeeId = record.EmployeeId });
            }

            _healthRecord.Entity.Delete(record);
            await _healthRecord.SaveAsync();

            TempData["success"] = "تم حذف السجل الصحي بنجاح";
            return RedirectToAction("CreateHealthRecord", new { employeeId = record.EmployeeId });
        }

        public async Task<IActionResult> PrintEmployeeCard(Guid id)
        {
            var employee = await _employee.Entity.Include(e => e.ApplicationUser).Where(i => i.Id == id).FirstOrDefaultAsync();
            if (employee == null)
            {
                return NotFound();
            }
            // ????? ????? ViewModel ??? ??? ???? ????? ?????? ??????
            return View("PrintEmployeeCard", employee);
        }

        // Action to display multiple employee cards for printing (if needed)
        public async Task<IActionResult> PrintAllEmployeeCards()
        {
            var employees = await _employee.Entity.Include(e => e.ApplicationUser)
                                          .ToListAsync();
            return View("PrintMultipleEmployeeCards", employees); // ?????? View ????? ??? ??? ??????? ???????
        }
        // EmployeesController.cs (???? ?? GET Action)


        public async Task<IActionResult> CreateHealthRecord(Guid employeeId) // ??? ?? ??? GET action
        {
            var employee = await _employee.Entity.GetByIdAsync(employeeId);
            if (employee == null)
            {
                // Log this maybe
                // _logger.LogWarning($"Employee with ID {employeeId} not found when trying to create health record.");
                return NotFound($"Employee with ID {employeeId} not found."); // ????? ????? ????? ???
            }

            // ??? ????? ??? ???? ???? ???? ?? ?????? ?????
            var chronicDiseases = _chronicDisease.Entity.GetAll(); // ????? ?? ??? ???? IEnumerable<ChronicDisease>

            // Load existing health records for display with medications
            var existingHealthRecords = await _healthRecord.Entity
                .Include(hr => hr.ChronicDisease)
                .Include(hr => hr.HealthRecordMedications)
                    .ThenInclude(hrm => hrm.Medicine)
                .Where(hr => hr.EmployeeId == employeeId)
                .OrderByDescending(hr => hr.DiagnosisDate)
                .ToListAsync();

            var viewModel = new CreateHealthRecordViewModel
            {
                Id = employee.Id,
                EmployeeName = employee.Name,
                ChronicDiseaseOptions = chronicDiseases.Select(cd => new SelectListItem
                {
                    Value = cd.Id.ToString(),
                    Text = cd.Name
                }).ToList(),
                // ????? HealthRecord ?????? EmployeeId
                // ??? ??? ??? constructor ????? ?? CreateHealthRecordViewModel ???? ?? new HealthRecord()?
                // ??? ??? ????? ????? ????? ??????. ??? ?? ??? ????? ???? ????? ?????.
                // ????? ??? ????????? ??? ???.
                HealthRecord = new HealthRecord { EmployeeId = employeeId },
                HealthRecords = existingHealthRecords
            };

            // ?????? ???????? ??? ?? ???? "return"
            return View("CreateHealthRecord", viewModel); // <--- ??? "return" ???

            // ??? ??? ??? ??? View ?? ???? ??? ??? Action (CreateHealthRecord.cshtml)
            // ????? ????? ???????:
            // return View(viewModel);
        }    // POST: Employees/CreateHealthRecordForEmployee (or HealthRecords/Create)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateHealthRecordForEmployee(CreateHealthRecordViewModel viewModel)
        {
            ModelState.Remove("HealthRecord.Employee");
            ModelState.Remove("HealthRecord.ChronicDisease");


            //if (ModelState.IsValid) // Validates viewModel.HealthRecord based on its DataAnnotations
            //{
                try
                {
                    // Ensure EmployeeId is set if somehow it wasn't bound (though hidden field should handle it)
                    if (viewModel.Id == Guid.Empty)
                    {
                        // This case should ideally not happen if the form is set up correctly
                        TempData["error"] = "????? ???? ?????? ??? ???????.";
                        // Repopulate and return
                        var employeeForName = await _employee.Entity.GetByIdAsync(viewModel.HealthRecord.EmployeeId); // This line might be problematic if EmployeeId is empty
                        viewModel.EmployeeName = employeeForName?.Name ?? "??? ?????";
                        var chronicDiseasesList = _chronicDisease.Entity.GetAll();
                        viewModel.ChronicDiseaseOptions = chronicDiseasesList.Select(cd => new SelectListItem
                        {
                            Value = cd.Id.ToString(),
                            Text = cd.Name
                        }).ToList();
                        return View("CreateHealthRecord", viewModel);
                    }
                viewModel.HealthRecord.EmployeeId = viewModel.Id;
                viewModel.HealthRecord.Created = DateTime.Now;

                    _healthRecord.Entity.Insert(viewModel.HealthRecord);
                    await _healthRecord.SaveAsync();
                    TempData["success"] = $"?? ????? ??? ??? ?????? ?????.";
                    // Redirect to the employee's details page
                    return RedirectToAction("Details", "Employees", new { id = viewModel.HealthRecord.EmployeeId });
                }
                catch (Exception ex)
                {
                    // Log the exception (ex)
                    TempData["error"] = "??? ??? ????? ????? ????? ?????.";
                // Fall through to re-render the form with error
                return RedirectToAction("Details", "Employees", new { id = viewModel.HealthRecord.EmployeeId });
            }
            //}
            //else
            //{
            //    TempData["error"] = "??? ????? ????? ?????. ???? ?????? ???????? ???????.";
            //}

            //// If ModelState is invalid or an error occurred, repopulate necessary data for the view model
            //var employee = await _employee.Entity.GetByIdAsync(viewModel.HealthRecord.EmployeeId); //_context.Employees.FindAsync(viewModel.HealthRecord.EmployeeId);
            //viewModel.EmployeeName = employee?.Name ?? "??? ?????"; // Handle if employee somehow not found

            //var chronicDiseases = _chronicDisease.Entity.GetAll(); //_context.ChronicDiseases.OrderBy(cd => cd.Name).ToListAsync();
            //viewModel.ChronicDiseaseOptions = chronicDiseases.Select(cd => new SelectListItem
            //{
            //    Value = cd.Id.ToString(),
            //    Text = cd.Name
            //}).ToList();
            //// The viewModel.HealthRecord already contains the user's input and validation errors

            //return View("CreateHealthRecord", viewModel);
        }


        [AllowAnonymous]
        [ViewLayout("_Layout")]
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound("Employee ID is missing.");
            }

            var employee = await _employee.Entity.GetByIdAsync(id.Value);

            if (employee == null)
            {
                TempData["error"] = "?????? ??? ?????.";
                return RedirectToAction(nameof(Index)); // Or your employee list action
            }

            var healthRecords = await _healthRecord.Entity
                                        .Include(hr => hr.ChronicDisease) // To display ChronicDisease.Name
                                        .Include(hr => hr.HealthRecordMedications) // Include medications
                                            .ThenInclude(hrm => hrm.Medicine) // Include medicine details
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

