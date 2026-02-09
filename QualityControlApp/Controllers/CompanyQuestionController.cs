using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using QualityControlApp.Classes;
using QualityControlApp.Models;
using QualityControlApp.Models.Entities;
using QualityControlApp.Models.Interfaces;
using QualityControlApp.ViewModels;
using SelectPdf;
using System.Data;
using Microsoft.Extensions.Options;
using System.Diagnostics.Contracts;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace QualityControlApp.Controllers
{
    [Authorize(Policy = "ProgOrAdminOrEmployeePolicy")]

    [ViewLayout("_LayoutDashboard")]
    public class CompanyQuestionController : BaseController
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IViewHelper _viewHelper;
        private readonly ApplicationDbContext _context;
        private readonly IUnitOfWork<Company> _company;
        private readonly IUnitOfWork<Contact> _contact;
        private readonly IUnitOfWork<SiteInfo> _siteInfo;
        private readonly IUnitOfWork<ApplicationUser> _applicationUser;
        private readonly IUnitOfWork<QuestionCategoryType> _questioncategorytype;
        private readonly IUnitOfWork<CompanyQuestion> _companyquestion;
        private readonly IUnitOfWork<CompanyQuestionAssignedUsers> _companyQuestionAssignedUsers;
        private readonly IUnitOfWork<CompanyTypeCategoryAvailable> _companytypeCategoryAvailable;
        private readonly IUnitOfWork<CompanyQuestionContent> _companyquestionContent;
        private readonly IUnitOfWork<Question> _question;
        private readonly IUnitOfWork<Location> _location;
        private readonly IEmailSender _emailSender;
        private readonly IUnitOfWork<QuestionType> _questiontype;
        private readonly IWebHostEnvironment _host;
        private readonly IServiceProvider _serviceProvider;
        public CompanyQuestionController(
            ApplicationDbContext context,
            IEmailSender emailSender,
            IUnitOfWork<Company> company,
          UserManager<ApplicationUser> userManager,
            IUnitOfWork<SiteInfo> siteInfo,
            IUnitOfWork<Contact> contact,
            IUnitOfWork<CompanyTypeCategoryAvailable> companytypeCategoryAvailable,
            IUnitOfWork<CompanyQuestionAssignedUsers> companyQuestionAssignedUsers,
            IUnitOfWork<Question> question,
            IUnitOfWork<Location> location,
            IUnitOfWork<QuestionCategoryType> questioncategorytype,
            IUnitOfWork<Employee> employee,
            IUnitOfWork<ApplicationUser> applicationUser,
            IUnitOfWork<QuestionType> questiontype,
            IUnitOfWork<CompanyQuestion> companyquestion,
            IUnitOfWork<CompanyQuestionContent> companyquestionContent,
            IWebHostEnvironment host,
                               IConfiguration configuration,
            IServiceProvider serviceProvider,
            IViewHelper viewHelper
            ) : base(host, configuration)
        {
            _questiontype = questiontype;
            _question = question;
            _location = location;
            _context = context;
            _siteInfo = siteInfo;
            _userManager = userManager;
            _company = company;
            _companytypeCategoryAvailable = companytypeCategoryAvailable;
            _companyQuestionAssignedUsers = companyQuestionAssignedUsers;
            _contact = contact;
            _emailSender = emailSender;
            _applicationUser = applicationUser;
            _companyquestion = companyquestion;
            _companyquestionContent = companyquestionContent;
            _questioncategorytype = questioncategorytype;
            _host = host;
            _serviceProvider = serviceProvider;
            _viewHelper = viewHelper;

        }


        public async Task<IActionResult> Index(string Type)
        {
            ViewData["HeroViewName"] = "CompanyQuestion_Index";
            var companyquestion = await _companyquestion.Entity.GetWhere(q => q.Type == Type) // old or New  ??? ???? ??? ?????? old
                       .Include(q => q.Company)  // ????? ??????? ??????
                       .Include(q => q.Creator)
                      .OrderBy(ec => ec.Active)
                      .OrderByDescending(q => q.Created)
                       .ToListAsync();

            var category = await _questioncategorytype
        .Entity.GetWhere(ct => ct.Type == Type)
        .ToListAsync();

            var Company = await _company.Entity.GetAll().ToListAsync(); ;
            var Location = await _location.Entity.GetAll().ToListAsync(); ;
            ViewBag.Type = Type;

            var CompanyQuestionVM = new CompanyQuestionVM
            {
                Company = Company,
                CompanyQuestion = companyquestion,
                QuestionCategoryType = category,
            };

            return View(CompanyQuestionVM);
        }


        public async Task<IActionResult> Create(string Type)
        {
            if (string.IsNullOrEmpty(Type))
            {
                TempData["ErrorMessage"] = "Task type is required.";
                return RedirectToAction("Index", new { Type = "old" }); // ???? Type ??? ????? ??? Redirect ???? ????
            }

            var availableCategoriesData = await _questioncategorytype.Entity
                                                .GetWhere(qct => qct.Type == Type)
                                                .OrderBy(qct => qct.CategoryName)
                                                .Select(qct => new QuestionCategoryTypeSelectItemVM
                                                {
                                                    Id = qct.Id,
                                                    CategoryName = qct.CategoryName
                                                })
                                                .ToListAsync();

            // ??? ?????????? ?????? ????? ???????? ?????????? ????????
            var allUsers = await _applicationUser.Entity
                                        .Include(u => u.UserProfile)
                                        .OrderBy(u => u.UserProfile != null ? u.UserProfile.DisplayName : u.UserName)
                                        .Select(u => new SelectListItem
                                        {
                                            Value = u.Id.ToString(),
                                            Text = u.UserProfile != null && !string.IsNullOrEmpty(u.UserProfile.DisplayName) ? u.UserProfile.DisplayName : u.UserName
                                        })
                                        .ToListAsync();

            // ??? ?????????? ?????? ????? ?????? (Creator)
            var creatorUsersList = allUsers; // ???? ??????? ??? ??????? ?? ????? ?????? ??? ??? ???? ???? ?????

            var viewModel = new CreateCompanyQuestionVM
            {
                CreatorId="123",
                Type = Type,
                AvailableQuestionCategoryTypes = availableCategoriesData,
                SelectedQuestionCategoryTypeIds = new List<Guid>(),
                AvailableAssignedUsers = allUsers, // ????? ????? ?????????? ???????? ???????
                SelectedAssignedUserIds = new List<Guid>() // ??????? ?? ???? ???????? ?????? ??????
            };

            ViewBag.CreatorUsers = new SelectList(creatorUsersList, "Value", "Text"); // ???????? ???????
            ViewBag.Companies = new SelectList(await _company.Entity.GetAll().OrderBy(c => c.Name).ToListAsync(), "Id", "Name");
            ViewBag.Location = new SelectList(await _location.Entity.GetAll().OrderBy(l => l.Name).ToListAsync(), "Id", "Name");
            // ??? ????? ViewBag.Users ???? ?? ???? ??? ????? ????? ?? AvailableAssignedUsers

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id)
        {
            var companyQuestion = await _companyquestion.Entity.GetByIdAsync(id);

            if (companyQuestion == null)
            {
                return NotFound(); // ?? ?????? ???? NotFound ?? ?????? ?? ?????
            }

            _companyquestion.Entity.Delete(companyQuestion);
            await _companyquestion.SaveAsync();

            return RedirectToAction("Index", new { type = companyQuestion.Type });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateCompanyQuestionVM viewModel) // ??????? ??? ViewModel
        {
            async Task PopulateViewBagsForCreateView(string type)
            {
                ViewBag.AvailableQuestionCategoryTypes = await _context.QuestionCategoryType
                                                         .Where(qct => qct.Type == type)
                                                         .OrderBy(qct => qct.CategoryName)
                                                         .ToListAsync();
                ViewBag.Users = new SelectList(await _applicationUser.Entity.GetAll().OrderBy(u => u.UserName).ToListAsync(), "Id", "UserName", viewModel.UserId);
                ViewBag.Companies = new SelectList(await _company.Entity.GetAll().OrderBy(c => c.Name).ToListAsync(), "Id", "Name", viewModel.CompanyId);
                ViewBag.Location = new SelectList(await _location.Entity.GetAll().OrderBy(l => l.Name).ToListAsync(), "Id", "Name", viewModel.LocationId);
            }

            viewModel.UserId = Guid.Parse(_userManager.GetUserId(User));
            viewModel.CreatorId = _userManager.GetUserId(User);


            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Please correct the validation errors.";
                var allUsers = await _applicationUser.Entity
                                        .Include(u => u.UserProfile)
                                        .OrderBy(u => u.UserProfile != null ? u.UserProfile.DisplayName : u.UserName)
                                        .Select(u => new SelectListItem
                                        {
                                            Value = u.Id.ToString(),
                                            Text = u.UserProfile != null && !string.IsNullOrEmpty(u.UserProfile.DisplayName) ? u.UserProfile.DisplayName : u.UserName
                                        })
                                        .ToListAsync();

                // ??? ?????????? ?????? ????? ?????? (Creator)
                var creatorUsersList = allUsers; // ???? ??????? ??? ??????? ?? ????? ?????? ??? ??? ???? ???? ?????

                var viewModel2 = new CreateCompanyQuestionVM
                {
                    AvailableQuestionCategoryTypes = null,
                    SelectedQuestionCategoryTypeIds = new List<Guid>(),
                    AvailableAssignedUsers = allUsers, // ????? ????? ?????????? ???????? ???????
                    SelectedAssignedUserIds = new List<Guid>() // ??????? ?? ???? ???????? ?????? ??????
                };

                ViewBag.CreatorUsers = new SelectList(creatorUsersList, "Value", "Text"); // ???????? ???????
                ViewBag.Companies = new SelectList(await _company.Entity.GetAll().OrderBy(c => c.Name).ToListAsync(), "Id", "Name");
                ViewBag.Location = new SelectList(await _location.Entity.GetAll().OrderBy(l => l.Name).ToListAsync(), "Id", "Name");
                // ??? ????? ViewBag.Users ???? ?? ???? ??? ????? ????? ?? AvailableAssignedUsers

                return View(viewModel2);
            }





            string Id = viewModel.UserId.ToString();
            var userForEmail = await _applicationUser.Entity.GetByIdAsync(Id);
            if (userForEmail == null)
            {
                ModelState.AddModelError("", "Selected user not found for email.");
                await PopulateViewBagsForCreateView(viewModel.Type);
                return View(viewModel);
            }
            var companyForEmail = await _company.Entity.GetByIdAsync(viewModel.CompanyId);
            if (companyForEmail == null)
            {
                ModelState.AddModelError("", "Selected company not found for email.");
                await PopulateViewBagsForCreateView(viewModel.Type);
                return View(viewModel);
            }

            var filePath = Path.Combine(_host.WebRootPath, "templates", "StartCompanyQuestion.html");
            if (System.IO.File.Exists(filePath))
            {


                var allUsers2 = await _applicationUser.Entity
      .Include(u => u.UserProfile)
      .OrderBy(u => u.UserProfile != null ? u.UserProfile.DisplayName : u.UserName)
      .ToListAsync();

                StreamReader htmlFile = new StreamReader(filePath);
                string emailContent = await htmlFile.ReadToEndAsync();
                htmlFile.Close();
                emailContent = emailContent.Replace("{Subject}", userForEmail.UserName); // ?? ?? ????? ?????
                emailContent = emailContent.Replace("{Content}", $"A new OverSight has been initiated for company: {companyForEmail.Name}.");

                emailContent = emailContent + " Inspectors ( ";
                foreach (var user in allUsers2)
                {
                    var email = user.Email;
                    emailContent = emailContent + "-";

                }
                emailContent = emailContent + ")";

                  // ????? ?? Message ? _emailSender ?????? ???? ????
                  var message = new Message(new string[] { userForEmail.Email }, $"Quality Task Started: {companyForEmail.Name}", emailContent, null);

                try
                {
                    await _emailSender.SendEmailAsync(message);
                    TempData["SuccessMessage"] = "Email notification sent successfully.";
                }
                catch (Exception ex)
                {
                    // Log the exception (ex)
                    TempData["WarningMessage"] = "Quality task created, but failed to send email notification. Error: " + ex.Message;
                }



                foreach (var user in allUsers2)
                {
                    var email = user.Email; // Access the Email property directly from the user object
                    if (!string.IsNullOrEmpty(email))
                    {
                        var displayName = user.UserProfile != null && !string.IsNullOrEmpty(user.UserProfile.DisplayName)
                            ? user.UserProfile.DisplayName
                            : user.UserName;

                        var message2 = new Message(
                            new string[] { email },
                            $"Quality Task Started: {companyForEmail.Name}",
                            emailContent,
                            null
                        );

                        try
                        {
                            await _emailSender.SendEmailAsync(message2);
                            TempData["SuccessMessage"] = $"Email sent successfully to {displayName}.";
                        }
                        catch (Exception ex)
                        {
                            TempData["WarningMessage"] = $"Failed to send email to {displayName}. Error: {ex.Message}";
                        }
                    }
                }


            }
            else
            {
                TempData["WarningMessage"] = "Quality task created, but email template not found. Email not sent.";
            }
            // --- ????? ??? ??????? ---

            var lastNum = await _context.CompanyQuestion
                                .Where(q => q.CompanyId == viewModel.CompanyId && q.Type == viewModel.Type) // ?? ???? ?? ????? ????? ?? ????? ??????
                                .OrderByDescending(q => q.Num)
                                .Select(q => (int?)q.Num)
                                .FirstOrDefaultAsync();

            var newCompanyQuestionEntity = new CompanyQuestion // ??? ?? ???? ??????
            {
                Num = (lastNum ?? 0) + 1,
                LocationId = viewModel.LocationId,
                CompanyId = viewModel.CompanyId,
                UserId = viewModel.UserId,
                CreatorId = viewModel.CreatorId,
                Active = false,
                SaftyGrid = 0,
                SqurtyGrid = 0,
                Type = viewModel.Type,
                Created = DateTime.UtcNow // ?????? UtcNow ???????
            };

            _context.CompanyQuestion.Add(newCompanyQuestionEntity);

            try
            {
                await _context.SaveChangesAsync(); // ??? CompanyQuestion ?????? ??? newCompanyQuestionEntity.Id
            }
            catch (Exception ex)
            {
                // Log the exception
                ModelState.AddModelError("", "An error occurred while creating the quality task. " + ex.Message);
                await PopulateViewBagsForCreateView(viewModel.Type);
                return View(viewModel);
            }


            // ????? ?? ?????? ??????? ????? ??? viewModel.SelectedQuestionCategoryTypeIds
            var filteredQuestions = await _question.Entity
                .Include(q => q.QuestionType)
                    .Include(qt => qt.QuestionType.QuestionCategoryType)
                .Where(q => q.QuestionType != null &&
                             q.QuestionType.QuestionCategoryType != null &&
                             q.QuestionType.QuestionCategoryType.Type == viewModel.Type &&
                             viewModel.SelectedQuestionCategoryTypeIds.Contains(q.QuestionType.QuestionCategoryType.Id))
                .ToListAsync();

            if (filteredQuestions.Any())
            {
                foreach (var question in filteredQuestions)
                {
                    var questionResponse = new CompanyQuestionContent
                    {
                        CompanyQuestionId = newCompanyQuestionEntity.Id, // ?????? ??? Id ?? ?????? ???? ?? ??????
                        QuestionId = question.Id,
                        Score = (viewModel.Type == "Old" ? (int?)0 : null), // ??? ??? ????? "Old" ???? ?? 0? ???? null
                        Level = null,
                        Nots = null,
                        Inspect = (viewModel.Type == "New" ? "" : null), // ??? ??? ????? "New" ???? ?? ""? ???? null
                        Created = DateTime.UtcNow
                    };
                    _context.CompanyQuestionContent.Add(questionResponse);
                }

                try
                {
                    await _context.SaveChangesAsync(); // ??? CompanyQuestionContents
                }
                catch (Exception ex)
                {
                    // Log the exception
                    // ?? ???? ?? ??? CompanyQuestion ???? ?? ?????? ??? ??? ??? ????? (?????? transaction)
                    TempData["ErrorMessage"] = "Task created, but an error occurred while adding questions. " + ex.Message;
                    return RedirectToAction("Details", "CompanyQuestion", new { id = newCompanyQuestionEntity.Id }); // ???? ???????? ???? ?? ?? ??????
                }
            }
            else
            {
                TempData["WarningMessage"] = (TempData["WarningMessage"] ?? "") + " Quality task created, but no questions were found for the selected categories.";
            }


            foreach (var userS in viewModel.SelectedAssignedUserIds) // ???? 'user' ??? ?? ???? ApplicationUser
            {
                var companyQuestionAssignedUsers = new CompanyQuestionAssignedUsers // ??? ?? ???? ??????
                {
                    AssignedCompanyQuestionsId = newCompanyQuestionEntity.Id,
                    AssignedUsersId = userS.ToString(),
                };
                _companyQuestionAssignedUsers.Entity.Insert(companyQuestionAssignedUsers);
            }





            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "CompanyQuestion", new { Type = viewModel.Type });
        }

        public async Task<IActionResult> Details(Guid id, Guid CategoryId)
        {

            var companyquestion = await _companyquestion.Entity
                .GetByIdAsync(id);
            if (companyquestion == null)
            {
                return View("NotFound");
            }


            Guid? companyTypeId = await _company.Entity // ?? _company.Entity.AsQueryable()
                                  .GetWhere(c => c.Id == companyquestion.CompanyId) // companyquestion.CompanyId ??? ?? ???? ?? ??? ??? Id ??????
                                  .Select(c => c.CompanyTypeId) // ??? ????? ??????? ???
                 .FirstOrDefaultAsync();



            var availableCategoryInfo = _companytypeCategoryAvailable.Entity
                .GetWhere(cta => cta.CompanyTypeId == companyTypeId)
                .Select(cta => cta.QuestionCategoryTypeId)
                .ToList();



            List<Guid> availableCategoryIds;

            if (_companytypeCategoryAvailable.Entity.GetWhere(cta => cta.CompanyTypeId == companyTypeId) is IQueryable<CompanyTypeCategoryAvailable> queryableLinks)
            {
                availableCategoryIds = await queryableLinks
                                            .Select(cta => cta.QuestionCategoryTypeId) // ??? ??? ??? ID ???????
                                            .Distinct() // ???? ??????? ??? ??? ??????
                                            .ToListAsync();
            }
            else
            {
                var links = _companytypeCategoryAvailable.Entity.GetWhere(cta => cta.CompanyTypeId == companyTypeId);
                availableCategoryIds = links.Select(cta => cta.QuestionCategoryTypeId).Distinct().ToList();
            }


            List<QuestionCategoryType> filteredCategories;
            if (availableCategoryIds != null && availableCategoryIds.Any())
            {
                // ??? ??? _questioncategorytype.Entity.GetAll() ???? IQueryable<QuestionCategoryType>
                if (_questioncategorytype.Entity.GetAll() is IQueryable<QuestionCategoryType> allCategoriesQueryable)
                {
                    filteredCategories = await allCategoriesQueryable
                        .Where(qct => availableCategoryIds.Contains(qct.Id)) // <-- ??????? ???
                        .OrderBy(qct => qct.CategoryName) // ????? ???????
                        .ToListAsync();
                }
                else
                {
                    filteredCategories = await _questioncategorytype.Entity.GetAll().ToListAsync(); // ??? ???? ?????
                    filteredCategories = filteredCategories
                        .Where(qct => availableCategoryIds.Contains(qct.Id))
                        .OrderBy(qct => qct.CategoryName)
                        .ToList();
                }
            }
            else
            {
                filteredCategories = new List<QuestionCategoryType>();
            }



            List<CompanyQuestionContent>? ContentList;
            List<QuestionType>? TypeList;



            if (CategoryExists(CategoryId))
            {

                ContentList = null;
                TypeList = await _questiontype.Entity
.GetWhere(n => n.QuestionCategoryTypeId == CategoryId)
.ToListAsync();

            }
            else
            {
                TypeList = null;
                ContentList = null;
            }




            var companies = await _company.Entity.GetAll().ToListAsync();

            var user = await _applicationUser.Entity.GetAll().ToListAsync();

            // ????? ???????? ??? SelectList
            ViewBag.Companies = new SelectList(companies, "Id", "Name", companyquestion.CompanyId);
            ViewBag.Users = new SelectList(user, "Id", "UserName", companyquestion.UserId);



            var CompanyQuestionContentVM = new CompanyQuestionContentVM
            {
                CompanyQuestion = companyquestion,
                CompanyQuestionContent = ContentList,
                QuestionType = TypeList,
                QuestionCategoryType = filteredCategories,
            };
            return View(CompanyQuestionContentVM);
        }
        private bool CategoryExists(Guid id)
        {
            return _questioncategorytype.Entity.GetAll()?.Any(e => e.Id == id) ?? false;
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            var companyquestion = await _companyquestion.Entity.GetByIdAsync(id);
            if (companyquestion == null)
            {
                return NotFound();
            }

            var companies = await _company.Entity.GetAll().ToListAsync();
            var user = await _applicationUser.Entity.GetAll().ToListAsync();

            // ????? ???????? ??? SelectList
            ViewBag.Companies = new SelectList(companies, "Id", "Name", companyquestion.CompanyId);
            ViewBag.Users = new SelectList(user, "Id", "UserName", companyquestion.CreatorId);

            return View(companyquestion);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,Created,CompanyId,EmployeeId,Active,SaftyGrid,SqurtyGrid,Type")] CompanyQuestion companyquestion)
        {
            if (id != companyquestion.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    companyquestion.Modified = DateTime.Now;
                    _companyquestion.Entity.Update(companyquestion);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CompanyQuestionExists(companyquestion.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                return RedirectToAction(nameof(Index), new { Type = companyquestion.Type });
            }

            // ????? ????? ??????? ????????? ?? ???? ?????
            var companies = await _company.Entity.GetAll().ToListAsync();
            var user = await _applicationUser.Entity.GetAll().ToListAsync();

            ViewBag.Companies = new SelectList(companies, "Id", "Name", companyquestion.CompanyId);
            ViewBag.Users = new SelectList(user, "Id", "UserName", companyquestion.UserId);

            return View(companyquestion);
        }

        private bool CompanyQuestionExists(Guid id)
        {
            return _companyquestion.Entity.GetByIdAsync(id) != null;
        }


        [HttpGet]
        public IActionResult GetChartData(Guid companyQuestionId, Guid typeId)
        {
            // ??? ????? ????? ???????? ??? ??? companyQuestionId ? typeId ??? ???? ??????
            var filteredData = _companyquestionContent.Entity
    .GetWhere(a => a.CompanyQuestionId == companyQuestionId)
    .ToList();
            // ?????? ???? ?? ????????

            return Json(filteredData);
        }
        public async Task<IActionResult> GetQuestionType(Guid id, Guid TypeId)
        {

            ViewData["ActiveQuestionTypeId"] = TypeId; // ??????? ViewData


            var companyquestion = await _companyquestion.Entity
                .GetByIdAsync(id);
            if (companyquestion == null)
            {
                return View("NotFound");
            }

            Guid Categoryid;
            Categoryid = _questiontype.Entity.GetWhere(n => n.Id == TypeId).Select(n => n.QuestionCategoryTypeId)
                .FirstOrDefault();

            var ContentList = await _companyquestionContent.Entity
                .Include(n => n.Question)
                .Where(n => n.CompanyQuestionId == companyquestion.Id)
                .Where(n => n.Question.QuestionTypeId == TypeId)
                .OrderByDescending(n => n.Created)
                .ToListAsync();

            var maxScores = ContentList.Select(c => c.Question.MaxGrid).ToList();

            // ???? ?????? ??????? ??? ???? ??????? ???? ??????
            var percentageScores = ContentList.Select(c =>
                (c.Score * 100.0) / (c.Question.MaxGrid > 0 ? c.Question.MaxGrid : 1) // ?????? ?? ?? MaxGrid ???? ?? 0 ????? ?????? ??? ???
            ).ToList();

            // ????? ??????? ?? "Labels"
            var labels = ContentList.Select(c => c.Question.Content).ToList();

            // ????? ???????? ?????? ??????? ?? JSON ?????? ?? ?????? ?????

            var TypeList = await _questiontype.Entity.GetWhere(n => n.QuestionCategoryTypeId == Categoryid)
                .ToListAsync();

            var companies = await _company.Entity.GetAll().ToListAsync();
            var user = await _applicationUser.Entity.GetAll().ToListAsync();

            // ?????: ??? ????????? ???? ??????? ???????? ??????
            bool hasOPPerm = false;
            bool hasAirPerm = false;
            bool hasBELPerm = false;

            if (User.IsInRole("OPPerm"))
            {
                hasOPPerm = true;
            }

            if (User.IsInRole("AirPerm"))
            {
                hasAirPerm = true;
            }


            if (User.IsInRole("BELPerm"))
            {
                hasBELPerm = true;
            }
            // ??????: ?? ????? ????? ?????? ????????? ??????? ??? ????? ??? ??????? ????????
            var allowedCategoryNames = new List<string>();
            if (hasOPPerm)
            {
                allowedCategoryNames.Add("OP");
            }
            if (hasAirPerm)
            {
                allowedCategoryNames.Add("Air");
                allowedCategoryNames.Add("AMO145");
            }
            if (hasBELPerm)
            {
                allowedCategoryNames.Add("BEL");
            }


            var category = await _questioncategorytype
        .Entity
        .GetWhere(ct => ct.Type == companyquestion.Type && // ????? ?????: ??????? ????? ??? ????? "Old" ?? "New"
                      allowedCategoryNames.Contains(ct.CategoryName)) // ????? ??????: ??????? ????? ??? ????? ????????? ??????? ???
        .ToListAsync();
            // ????? ???????? ??? SelectList
            ViewBag.Companies = new SelectList(companies, "Id", "Name", companyquestion.CompanyId);
            ViewBag.Users = new SelectList(user, "Id", "UserName", companyquestion.UserId);

            var labels2 = ContentList.Select(c => c.Question.Content).ToList();
            var percentageScores2 = ContentList.Select(c =>
                (c.Score * 100.0) / (c.Question.MaxGrid > 0 ? c.Question.MaxGrid : 1)
            ).ToList();

            ViewBag.Labels2 = labels2;
            ViewBag.PercentageScores2 = percentageScores2;

            var CompanyQuestionContentVM = new CompanyQuestionContentVM
            {
                ActiveQuestionTypeId = TypeId, // ?? ?????? ????? ?????
                CompanyQuestion = companyquestion,
                CompanyQuestionContent = ContentList,
                QuestionType = TypeList,
                QuestionCategoryType = category,
            };
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("_QuestionDetailsPartial", CompanyQuestionContentVM);
            }
            return PartialView("_QuestionDetailsPartial", CompanyQuestionContentVM);

        }


        [HttpPost]
        public IActionResult UpdateCompanyQuestionContent([FromBody] List<CompanyQuestionContentUpdateViewModel> updatedData, Guid Id)
        {
            if (updatedData != null && updatedData.Any())
            {
                try
                {
                    foreach (var dataItem in updatedData)
                    {
                        var existingContent = _companyquestionContent.Entity
                         .GetWhere(c => c.CompanyQuestionId == Id && c.QuestionId == dataItem.QuestionId).FirstOrDefault();


                        if (existingContent != null)
                        {
                            if (dataItem.Score.HasValue)
                            {
                                existingContent.Score = dataItem.Score.Value;
                            }
                            else
                            {
                                existingContent.Inspect = dataItem.Inspect;
                                existingContent.Nots = dataItem.Notes;
                                existingContent.Level = dataItem.Level;
                            }
                            _context.Update(existingContent);
                        }
                        // ????? ??? ????? ???? ?????? ??? ???? ??? ?? ??? ???????
                    }
                    TempData["SuccessMessage"] = "?? ????? ?????";
                    _context.SaveChanges();

                    return Json(new { success = true });
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, message = ex.Message });
                }
            }
            return Json(new { success = false, message = "?? ???? ?????? ?????." });
        }

        // ????? ViewModel ???????? ???????? (??????? ???? ????)
        public class CompanyQuestionContentUpdateViewModel
        {
            public Guid QuestionId { get; set; }
            public int? Score { get; set; }
            public string Inspect { get; set; }
            public string Notes { get; set; }
            public int Level { get; set; }
        }

        // ???? ????? ????? ??????? ????? ??? CompanyQuestion.Id
        [HttpPost]
        public async Task<IActionResult> UpdateActiveStatus(Guid id, bool active)
        {

            var programmerSettings = _serviceProvider.GetRequiredService<IOptions<ProgrammerSettings>>().Value;
            string connectionString = programmerSettings.DbCon;

            // ????? ?? ????? ???????? ??? ID
            var companyQuestion = await _companyquestion.Entity.GetByIdAsync(id);

            if (companyQuestion == null)
            {
                return NotFound();
            }

            // ????? ???? Active
            companyQuestion.Active = active;

            // ??? ????????? ?? ????? ????????
            _context.SaveChanges();

            //---------------------------


            // ????? ????? ???????? ??? ??? ?????? ?? ?? ???? ???? ??? ??????
            return RedirectToAction("Index", "CompanyQuestion", new { id = companyQuestion.UserId });
        }


        [HttpPost]
        public async Task<ActionResult> UpdateActive(Guid companyQuestionId, bool newActiveValue)
        {
            // ??????? ??????? ?????? ?????? "Active"
            var compayneQuestion = await _companyquestion.Entity.GetByIdAsync(companyQuestionId);
            bool result;
            if (compayneQuestion.Type == "Old")
            {
                result = UpdateCompanyQuestionActiveOld(companyQuestionId, newActiveValue);
            }
            else
            {
                result = UpdateCompanyQuestionActiveNew(companyQuestionId, newActiveValue);

            }

            var compayneQuestion2 = await _companyquestion.Entity.GetByIdAsync(companyQuestionId);


            if (!compayneQuestion2.Active == true)
            {
                var companyQuestion = await _companyquestion.Entity.GetByIdAsync(companyQuestionId);

                if (companyQuestion == null)
                {
                    return NotFound();
                }


                string filePath;
                if (companyQuestion.Type == "Old")
                {

                    filePath = _host.WebRootPath + "\\templates" + "\\EndCompanyQuestion.html";
                }
                else
                {
                    filePath = _host.WebRootPath + "\\templates" + "\\EndCompanyQuestionNew.html";

                }
                var Uesr = await _applicationUser.Entity.GetByIdAsync(companyQuestion.CreatorId);
                if (Uesr == null)
                {
                    return NotFound();
                }

                var company = await _company.Entity.GetByIdAsync(companyQuestion.CompanyId);
                if (company == null)
                {
                    return NotFound();
                }
                var EmailInfo = await _contact.Entity.GetAll().FirstOrDefaultAsync();
                if (EmailInfo == null)
                {
                    return NotFound();
                }
                var SiteInfo = await _siteInfo.Entity.GetAll().FirstOrDefaultAsync();
                if (SiteInfo == null)
                {
                    return NotFound();
                }
                // ???? ?? ??????? ?? ????? ?????
                if (result)
                {


                    //------------------------------------------------

                    StreamReader htmlFile = new StreamReader(filePath);
                    string content = htmlFile.ReadToEnd();
                    htmlFile.Close();
                    //?? ???????? ?????: ??? ??? ??????? ???? ???? ?? ????? ??????? ????? ??? ???????// Subject
                    content = content.Replace("{Subject}", Uesr.UserName); // ???? ???? ???????
                    content = content.Replace("{Content}", company.Name);
                    content = content.Replace("{SPL}", companyQuestion.SaftyGrid.ToString());
                    content = content.Replace("{COL}", companyQuestion.SqurtyGrid.ToString());
                    content = content.Replace("{CurrentYear}", DateTime.Now.Year.ToString());
                    content = content.Replace("{SiteName}", SiteInfo.Name);
                    content = content.Replace("{Mail}", EmailInfo.Email);
                    content = content.Replace("{Phone}", EmailInfo.Phone);


                    var message = new Message(new string[] { companyQuestion.Creator.UserName }, "OverSiteUpdate", content, null);

                    try
                    {
                        await _emailSender.SendEmailAsync(message);
                        TempData["SuccessMessage"] = "The email has been sent successfully";
                    }
                    catch
                    {
                        ViewBag.errorMessage = "Failed to send email";
                        TempData["ErrorMessage"] = "Failed to send email";
                    }
                }
            }

            // ???? ?? ??????? ?? ????? ?????
            if (result)
            {


                // ????? ??????? ??? ????? ?? ??? ????? ??????
                return RedirectToAction("Index", "CompanyQuestion", new { type = compayneQuestion.Type });  // ????? ??????? ???? "Index" ?? Controller "CompanyQuestion"
            }
            else
            {
                // ?? ???? ??? ???????? ??? ????? ???
                TempData["ErrorMessage"] = "??? ????? ??????";
                return RedirectToAction("Index", new { type = compayneQuestion.Type });  // ?????? ??? ????? ??????
            }
        }

        // ???? ???????? ??????? ?????? ?? SQL Server
        private bool UpdateCompanyQuestionActiveOld(Guid companyQuestionId, bool newActiveValue)
        {
            var programmerSettings = _serviceProvider.GetRequiredService<IOptions<ProgrammerSettings>>().Value;
            string connectionString = programmerSettings.DbCon;


            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand("UpdateSPL_COL_Proc", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        // ????? ???????? ??? ??????? ??????
                        cmd.Parameters.AddWithValue("@id", companyQuestionId);
                        cmd.Parameters.AddWithValue("@newActiveValue", newActiveValue);

                        // ????? ??????? ??????
                        var rowsAffected = cmd.ExecuteNonQuery(); // ???? ??????? ????? ??? ?????? ????????

                        return rowsAffected > 0;  // ??? ??? ???? ???? ?? ??????? ?????
                    }
                }
            }
            catch (Exception ex)
            {
                // ?? ???? ???? ?? ???
                Console.WriteLine("Error: " + ex.Message);
                return false;
            }



        }

        private bool UpdateCompanyQuestionActiveNew(Guid companyQuestionId, bool newActiveValue)
        {
            var programmerSettings = _serviceProvider.GetRequiredService<IOptions<ProgrammerSettings>>().Value;
            string connectionString = programmerSettings.DbCon;


            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand("UpdateStateOnly_Proc", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        // ????? ???????? ??? ??????? ??????
                        cmd.Parameters.AddWithValue("@id", companyQuestionId);
                        cmd.Parameters.AddWithValue("@newActiveValue", newActiveValue);

                        // ????? ??????? ??????
                        var rowsAffected = cmd.ExecuteNonQuery(); // ???? ??????? ????? ??? ?????? ????????

                        return rowsAffected > 0;  // ??? ??? ???? ???? ?? ??????? ?????
                    }
                }
            }
            catch (Exception ex)
            {
                // ?? ???? ???? ?? ???
                Console.WriteLine("Error: " + ex.Message);
                return false;
            }

        }

        public async Task<ActionResult> SendStartEmail(Guid companyId, string UserId)
        {


            return View();

        }

        public async Task<ActionResult> PrintReport(Guid Id, Guid? CategoryId)
        {
            var companyquestion = await _companyquestion.Entity
                .GetWhere(s => s.Id == Id)
                .FirstOrDefaultAsync();

            List<CompanyQuestionContent> lstQty;
            List<QuestionCategoryType> lstCategory;

            if (CategoryId.HasValue)
            {
                // ??? ??????? ???????? ???????? ?????? ???
                lstQty = await _companyquestionContent.Entity
                    .Include(s => s.Question.QuestionType.QuestionCategoryType)
                    .Include(s => s.CompanyQuestion.Company)
                    .Include(s => s.CompanyQuestion.Creator)
                    .Where(s =>
                        s.CompanyQuestionId == Id &&
                        s.Question.QuestionType.QuestionCategoryTypeId == CategoryId &&
                        (s.CompanyQuestion.Type != "New" || s.Inspect == "Ns"))
                    .OrderBy(s => s.Question.QuestionType.TypeName)
                    .ToListAsync();

                // ??? ??????? ?????? ???
                lstCategory = await _questioncategorytype.Entity
                    .GetWhere(n => n.Id == CategoryId.Value)
                    .ToListAsync();



            }
            else
            {
                // ??? ???? ???????
                lstQty = await _companyquestionContent.Entity
                    .Include(s => s.Question.QuestionType.QuestionCategoryType)
                    .Include(s => s.CompanyQuestion.Company)
                    .Include(s => s.CompanyQuestion.Creator)
                    .Where(s =>
                        s.CompanyQuestionId == Id &&
                        (s.CompanyQuestion.Type != "New" || s.Inspect == "Ns"))
                    .OrderBy(s => s.Question.QuestionType.TypeName)
                    .ToListAsync();

                // ?????: ??? ????????? ???? ??????? ???????? ??????
                bool hasOPPerm = false;
                bool hasAirPerm = false;
                bool hasBELPerm = false;

                if (User.IsInRole("OPPerm"))
                {
                    hasOPPerm = true;
                }

                if (User.IsInRole("AirPerm"))
                {
                    hasAirPerm = true;
                }


                if (User.IsInRole("BELPerm"))
                {
                    hasBELPerm = true;
                }
                // ??????: ?? ????? ????? ?????? ????????? ??????? ??? ????? ??? ??????? ????????
                var allowedCategoryNames = new List<string>();
                if (hasOPPerm)
                {
                    allowedCategoryNames.Add("OP");
                }
                if (hasAirPerm)
                {
                    allowedCategoryNames.Add("Air");
                    allowedCategoryNames.Add("AMO145");
                }
                if (hasBELPerm)
                {
                    allowedCategoryNames.Add("BEL");
                }


                lstCategory = _questioncategorytype
           .Entity
           .GetWhere(ct => ct.Type == companyquestion.Type && // ????? ?????: ??????? ????? ??? ????? "Old" ?? "New"
                         allowedCategoryNames.Contains(ct.CategoryName)) // ????? ??????: ??????? ????? ??? ????? ????????? ??????? ???
           .ToList();


            }

            var studentname = lstQty?.FirstOrDefault()?.CompanyQuestion?.Creator?.UserName ?? "??? ??????";
            var createdate = lstQty?.FirstOrDefault()?.CompanyQuestion?.Created ?? DateTime.Now;
            var companyName = lstQty?.FirstOrDefault()?.CompanyQuestion?.Company?.Name ?? "??? ??????";

            var vm = new RepUserCompanyQuestionVM()
            {
                CompanyName = companyName,
                CreateQuestion = companyquestion,
                CreateDate = createdate.ToString("d"),
                UserName = studentname,
                lstCompanyQuestionContent = lstQty,
                lstQuestionCategoryType = lstCategory
            };


            var converter = new HtmlToPdf();
            var fullView = new HtmlToPdf();
            // Render the view to a string
            var viewContent = _viewHelper.RenderViewToString("~/Views/Report/UserCompanyQuestion.cshtml", vm);
            using MemoryStream ms = new MemoryStream();


            // convert the url to pdf
            PdfDocument doc = converter.ConvertHtmlString(viewContent);
            //doc.Append(converter.ConvertHtmlString(viewChart2));
            //doc.Append(converter.ConvertHtmlString(viewChart1));
            //// save pdf document
            doc.Save(ms);
            // close pdf document
            doc.Close();
            //
            //return File(ms.ToArray(), "application/pdf", $"{studentname}.pdf");
            return File(ms.ToArray(), "application/pdf");

        }
        [HttpPost]
        public async Task<IActionResult> UpdateQuestionContent([FromBody] List<QuestionContentUpdateVM> updates)
        {
            if (updates == null || !updates.Any())
            {
                return BadRequest("?? ???? ?????? ?????.");
            }

            bool hasChanges = false;

            foreach (var item in updates)
            {
                // ??????? ?????? ?? ????? ????????
                // ??????? ????? ??? ????? ????? ??? repository ?? DbContext
                // ?????? ?????? ????? ?? _companyquestionContent.Entity.GetByIdAsync ?????
                var questionContent = await _companyquestionContent.Entity.GetByIdAsync(item.Id);
                // ?? ??? ??? ?????? DbContext ??????:
                // var questionContent = await _context.CompanyQuestionContents.FindAsync(item.Id);


                if (questionContent != null)
                {
                    bool currentItemChanged = false;

                    if (questionContent.Score != item.Score)
                    {
                        questionContent.Score = item.Score;
                        currentItemChanged = true;
                    }

                    if (questionContent.Nots != item.Nots) // ??????? item.Nots
                    {
                        questionContent.Nots = item.Nots;
                        currentItemChanged = true;
                    }

                    if (questionContent.Level != item.Level)
                    {
                        questionContent.Level = item.Level;
                        currentItemChanged = true;
                    }


                    if (questionContent.Inspect != item.Inspect)
                    {
                        questionContent.Inspect = item.Inspect;
                        currentItemChanged = true;
                    }


                    if (!string.IsNullOrEmpty(item.Inspect))
                    {
                        // ??? ?? ????? S ?? Na? ?? ???? Nots ? Level
                        if (item.Inspect == "S" || item.Inspect == "Na") // ????? ?? "Na"
                        {
                            if (questionContent.Nots != null)
                            {
                                questionContent.Nots = null;
                                currentItemChanged = true;
                            }
                            if (questionContent.Level != null)
                            {
                                questionContent.Level = null;
                                currentItemChanged = true;
                            }
                        }
                    }

                    if (currentItemChanged)
                    {
                        // ??? ??? ?? ?????? Unit of Work pattern ????? ????????? ????????
                        // ?? ????? ?????? ?????? ???? ???? ???
                        // _companyquestionContent.Entity.Update(questionContent); ?? _context.Update(questionContent);
                        // ????? ??? ????????
                        hasChanges = true;
                    }
                }
                else
                {
                    // ????? ????? ??? ??? ??? ??? ?? ??????? ?? ???? ?????? ??????? ??????
                    Console.WriteLine($"QuestionContent with Id {item.Id} not found for update.");
                }
            }

            if (hasChanges)
            {
                try
                {
                    await _context.SaveChangesAsync(); // ?? _unitOfWork.CompleteAsync();
                    return Ok(new { message = "?? ??? ????????? ?????." });
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    Console.WriteLine($"Concurrency error during save: {ex.Message}");
                    return StatusCode(500, "??? ??? ????? ?????? ??? ???????? ???? ????? ??????. ???? ??? ????.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Generic error during save: {ex.Message}");
                    return StatusCode(500, "??? ??? ??? ????? ????? ?????? ??? ????????.");
                }
            }

            return Ok(new { message = "?? ??? ?????? ??? ??????? ?????." });
        }

        public async Task<IActionResult> FilterQuestions(string Type, Guid? companyId, DateTime? dateFrom, DateTime? dateTo, Guid? locationId)
        {
            Console.WriteLine($"Filter Action Called - Type: {Type}, CompanyId: {companyId}"); // Debugging

            // --- 1. ???? ????????? ??????? ?????? ---
            var query = _companyquestion.Entity
                        .GetWhere(q => q.Type == Type);

            // --- 2. ????? ????? ?????? ??? ?? ????? ID ???? ---
            if (companyId.HasValue && companyId.Value != Guid.Empty)
            {
                query = query.Where(q => q.CompanyId == companyId.Value);
            }

            if (locationId.HasValue && locationId.Value != Guid.Empty)
            {
                query = query.Where(q => q.LocationId == locationId.Value);
            }
            if (dateFrom.HasValue)
            {
                query = query.Where(q => q.Created >= dateFrom.Value);
            }

            if (dateTo.HasValue)
            {
                // ?? ???? ????? ???? ????? ??????? ??? ????? ?????? ?? ?????? `<`
                query = query.Where(q => q.Created <= dateTo.Value.Date.AddDays(1).AddTicks(-1));
            }

            // --- 3. ????? ???????? ???????? ?????? ??????? ---
            var filteredQuestions = await query
                                        .Include(q => q.Company)
                                        .Include(q => q.Creator)
                                        .OrderByDescending(q => q.Created) // ??? ??????? ???????? ?? Index
                                        .ToListAsync();

            // --- 4. ??? ?????? ???????? ??? Partial View ---




            var categories = _questioncategorytype
       .Entity
       .GetWhere(ct => ct.Type == Type) // ????? ??????: ??????? ????? ??? ????? ????????? ??????? ???
       .ToList();

            // --- 5. ????? ??? ViewModel ??? Partial View ---
            // ?????? ??? ??? ViewModel ???? ???? ??? ???????? ????????
            var partialViewModel = new CompanyQuestionVM
            {
                CompanyQuestion = filteredQuestions,
                QuestionCategoryType = categories,
                Company = null // ?? ????? ??????? ???
            };

            Console.WriteLine($"Found {filteredQuestions.Count} questions after filter."); // Debugging


            // --- 6. ????? ??? Partial View ?? ???????? ???????? ---
            return PartialView("_CompanyQuestionsCards", partialViewModel);
        }




        [HttpGet]
        public async Task<IActionResult> GetCategoriesForCompanyType(Guid? companyId)
        {
            if (companyId == null)
            {
                return Json(new List<object>()); // ?? BadRequest()
            }

            // 1. ???? ?? ?????? ????? ??? CompanyTypeId ????? ???
            var company = await _company.Entity.GetByIdAsync(companyId);

            if (company == null || company.CompanyTypeId == null) // ???? ?? ?????? ?????? ??? ????? CompanyTypeId
            {
                return Json(new List<object>()); // ?? ???? ???? ?? ?? ???? ??? ???? ?????
            }

            Guid companyTypeId = company.CompanyTypeId.Value; // .Value ??? CompanyTypeId ?? ???? int?

            var categories = await _companytypeCategoryAvailable.Entity
                .GetWhere(cta => cta.CompanyTypeId == companyTypeId)
                .Include(cta => cta.QuestionCategoryType) // ?? ?????? ?????? QuestionCategoryType
                .Select(cta => new // ?? ?????? ???????? ??? DTO ???? ?? JSON
                {
                    id = cta.QuestionCategoryType.Id, // ?? cta.QuestionCategoryTypeId ??? ??? ??? ID ?? ????
                    categoryName = cta.QuestionCategoryType.CategoryName
                })
                .OrderBy(qct => qct.categoryName) // ????? ???????
                .ToListAsync();

            return Json(categories);
        }




    }


}






