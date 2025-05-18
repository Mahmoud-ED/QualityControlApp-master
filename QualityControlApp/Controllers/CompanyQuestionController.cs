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
            IUnitOfWork<Question> question,
            IUnitOfWork<Location> location,
            IUnitOfWork<QuestionCategoryType> questioncategorytype,
            IUnitOfWork<Employee> employee,
            IUnitOfWork<ApplicationUser> applicationUser,
            IUnitOfWork<QuestionType> questiontype,
            IUnitOfWork<CompanyQuestion> companyquestion,
            IUnitOfWork<CompanyQuestionContent> companyquestionContent,
            IWebHostEnvironment host,
            IServiceProvider serviceProvider,
            IViewHelper viewHelper
            ) : base(host)
        {
            _questiontype = questiontype;
            _question = question;
            _location = location;
            _context = context;
            _siteInfo = siteInfo;
            _userManager = userManager;
            _company = company;
            _companytypeCategoryAvailable = companytypeCategoryAvailable;
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

            var companyquestion = await _companyquestion.Entity.GetWhere(q => q.Type == Type) // old or New  الي يبيه عمي الشارف old
                       .Include(q => q.Company)  // تضمين معلومات الشركة
                       .Include(q => q.ApplicationUser)
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
                return RedirectToAction("Index");
            }

            var availableCategoriesData = await _context.QuestionCategoryType
                                                  .Where(qct => qct.Type == Type)
                                                  .OrderBy(qct => qct.CategoryName)
                                                  .Select(qct => new QuestionCategoryTypeSelectItemVM
                                                  {
                                                      Id = qct.Id,
                                                      CategoryName = qct.CategoryName
                                                  })
                                                  .ToListAsync();



            List<Guid> preselectedCategoryIds = new List<Guid>();



            var viewModel = new CreateCompanyQuestionVM
            {
                Type = Type,
                AvailableQuestionCategoryTypes = availableCategoriesData,
                SelectedQuestionCategoryTypeIds = preselectedCategoryIds // تعيين القائمة المبدئية هنا
            };


            var usersWithProfile = await _applicationUser.Entity  // أو _userRepository.GetQueryable() أو ما شابه
                                .Include(u => u.UserProfile) // <-- تضمين Profile
                                .OrderBy(u => u.UserName) // يمكنك الترتيب حسب UserName أو Profile.Name
                                .ToListAsync();

            // إنشاء قائمة SelectListItem مخصصة
            var userSelectList = usersWithProfile.Select(u => new SelectListItem
            {
                Value = u.Id.ToString(), // القيمة التي سيتم إرسالها عند الاختيار (عادةً الـ Id)
                Text = !string.IsNullOrEmpty(u.UserProfile?.DisplayName) ? u.UserProfile.DisplayName : u.UserName // <-- المنطق المطلوب
            }).ToList();
            ViewBag.Users = userSelectList;
            //ViewBag.Users = new SelectList(await _applicationUser.Entity.GetAll().OrderBy(u => u.UserName).ToListAsync(), "Id", "UserName");
            ViewBag.Companies = new SelectList(await _company.Entity.GetAll().OrderBy(c => c.Name).ToListAsync(), "Id", "Name");
            ViewBag.Location = new SelectList(await _location.Entity.GetAll().OrderBy(l => l.Name).ToListAsync(), "Id", "Name");

            return View(viewModel);
        }
    
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id)
        {
            var companyQuestion = await _companyquestion.Entity.GetByIdAsync(id);

            if (companyQuestion == null)
            {
                return NotFound(); // من الأفضل ترجع NotFound لو العنصر مش موجود
            }

            _companyquestion.Entity.Delete(companyQuestion);
            await _companyquestion.SaveAsync();

            return RedirectToAction("Index", new { type = companyQuestion.Type });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateCompanyQuestionVM viewModel) // استقبال الـ ViewModel
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

            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Please correct the validation errors.";
                await PopulateViewBagsForCreateView(viewModel.Type);
                return View(viewModel);
            }

            var userForEmail = await _applicationUser.Entity.GetByIdAsync(viewModel.UserId);
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
                StreamReader htmlFile = new StreamReader(filePath);
                string emailContent = await htmlFile.ReadToEndAsync();
                htmlFile.Close();
                emailContent = emailContent.Replace("{Subject}", userForEmail.UserName); // أو أي عنوان مناسب
                emailContent = emailContent.Replace("{Content}", $"A new quality task of type '{viewModel.Type}' has been initiated for company: {companyForEmail.Name}.");

                // افترض أن Message و _emailSender مهيئان بشكل صحيح
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
            }
            else
            {
                TempData["WarningMessage"] = "Quality task created, but email template not found. Email not sent.";
            }
            // --- نهاية كود الإيميل ---

            var lastNum = await _context.CompanyQuestion
                                .Where(q => q.CompanyId == viewModel.CompanyId && q.Type == viewModel.Type) // قد ترغب في تضمين النوع في ترقيم المهام
                                .OrderByDescending(q => q.Num)
                                .Select(q => (int?)q.Num)
                                .FirstOrDefaultAsync();

            var newCompanyQuestionEntity = new CompanyQuestion // هذا هو كائن الكيان
            {
                Num = (lastNum ?? 0) + 1,
                LocationId = viewModel.LocationId,
                CompanyId = viewModel.CompanyId,
                UserId = viewModel.UserId,
                Active = false, 
                SaftyGrid = 0,
                SqurtyGrid = 0,
                Type = viewModel.Type,
                Created = DateTime.UtcNow // استخدم UtcNow للخوادم
            };

            _context.CompanyQuestion.Add(newCompanyQuestionEntity);

            try
            {
                await _context.SaveChangesAsync(); // حفظ CompanyQuestion للحصول على newCompanyQuestionEntity.Id
            }
            catch (Exception ex)
            {
                // Log the exception
                ModelState.AddModelError("", "An error occurred while creating the quality task. " + ex.Message);
                await PopulateViewBagsForCreateView(viewModel.Type);
                return View(viewModel);
            }


            // الآن، قم بفلترة الأسئلة بناءً على viewModel.SelectedQuestionCategoryTypeIds
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
                        CompanyQuestionId = newCompanyQuestionEntity.Id, // استخدم الـ Id من الكيان الذي تم إنشاؤه
                        QuestionId = question.Id,
                        Score = (viewModel.Type == "Old" ? (int?)0 : null), // إذا كان النوع "Old" ابدأ بـ 0، وإلا null
                        Level = null,
                        Nots = null,
                        Inspect = (viewModel.Type == "New" ? "" : null), // إذا كان النوع "New" ابدأ بـ ""، وإلا null
                        Created = DateTime.UtcNow
                    };
                    _context.CompanyQuestionContent.Add(questionResponse);
                }

                try
                {
                    await _context.SaveChangesAsync(); // حفظ CompanyQuestionContents
                }
                catch (Exception ex)
                {
                    // Log the exception
                    // قد تفكر في حذف CompanyQuestion الذي تم إنشاؤه إذا فشل هذا الجزء (معاملة transaction)
                    TempData["ErrorMessage"] = "Task created, but an error occurred while adding questions. " + ex.Message;
                    return RedirectToAction("Details", "CompanyQuestion", new { id = newCompanyQuestionEntity.Id }); // اذهب للتفاصيل ليرى ما تم إنشاؤه
                }
            }
            else
            {
                TempData["WarningMessage"] = (TempData["WarningMessage"] ?? "") + " Quality task created, but no questions were found for the selected categories.";
            }

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


            Guid? companyTypeId = await _company.Entity // أو _company.Entity.AsQueryable()
                                  .GetWhere(c => c.Id == companyquestion.CompanyId) // companyquestion.CompanyId يجب أن يكون من نفس نوع Id الشركة
                                  .Select(c => c.CompanyTypeId) // حدد الحقل المطلوب فقط
                 .FirstOrDefaultAsync();


          
            var availableCategoryInfo = _companytypeCategoryAvailable.Entity
                .GetWhere(cta => cta.CompanyTypeId == companyTypeId)
                .Select(cta => cta.QuestionCategoryTypeId)
                .ToList();



            List<Guid> availableCategoryIds;

            if (_companytypeCategoryAvailable.Entity.GetWhere(cta => cta.CompanyTypeId == companyTypeId) is IQueryable<CompanyTypeCategoryAvailable> queryableLinks)
            {
                availableCategoryIds = await queryableLinks
                                            .Select(cta => cta.QuestionCategoryTypeId) // حدد فقط الـ ID المطلوب
                                            .Distinct() // لمنع التكرار إذا كان ممكنًا
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
                // إذا كان _questioncategorytype.Entity.GetAll() يعيد IQueryable<QuestionCategoryType>
                if (_questioncategorytype.Entity.GetAll() is IQueryable<QuestionCategoryType> allCategoriesQueryable)
                {
                    filteredCategories = await allCategoriesQueryable
                        .Where(qct => availableCategoryIds.Contains(qct.Id)) // <-- الفلترة هنا
                        .OrderBy(qct => qct.CategoryName) // ترتيب اختياري
                        .ToListAsync();
                }
                else
                {
                    filteredCategories = await _questioncategorytype.Entity.GetAll().ToListAsync(); // جلب الكل أولاً
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

            // تحويل البيانات إلى SelectList
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

            // تحويل البيانات إلى SelectList
            ViewBag.Companies = new SelectList(companies, "Id", "Name", companyquestion.CompanyId);
            ViewBag.Users = new SelectList(user, "Id", "UserName", companyquestion.UserId);

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

            // إعادة تحميل الشركات والموظفين في حالة الفشل
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
            // هنا يمكنك تصفية البيانات حسب الـ companyQuestionId و typeId إذا كانت مطلوبة
            var filteredData = _companyquestionContent.Entity
    .GetWhere(a => a.CompanyQuestionId == companyQuestionId)
    .ToList();
            // مؤقتًا نعيد كل البيانات

            return Json(filteredData);
        }
        public async Task<IActionResult> GetQuestionType(Guid id, Guid TypeId)
        {

            ViewData["ActiveQuestionTypeId"] = TypeId; // استخدام ViewData


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

            // حساب النسبة المئوية لكل درجة بالنسبة للحد الأقصى
            var percentageScores = ContentList.Select(c =>
                (c.Score * 100.0) / (c.Question.MaxGrid > 0 ? c.Question.MaxGrid : 1) // التأكد من أن MaxGrid أكبر من 0 لتجنب القسمة على صفر
            ).ToList();

            // محتوى الأسئلة كـ "Labels"
            var labels = ContentList.Select(c => c.Question.Content).ToList();

            // إرسال البيانات الخاصة بالشارت كـ JSON لعرضها في الجافا سكربت

            var TypeList = await _questiontype.Entity.GetWhere(n => n.QuestionCategoryTypeId == Categoryid)
                .ToListAsync();

            var companies = await _company.Entity.GetAll().ToListAsync();
            var user = await _applicationUser.Entity.GetAll().ToListAsync();

            // أولاً: حدد الصلاحيات التي يمتلكها المستخدم الحالي
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
            // ثانياً: قم ببناء قائمة بأسماء التصنيفات المسموح بها بناءً على صلاحيات المستخدم
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
        .GetWhere(ct => ct.Type == companyquestion.Type && // الشرط الأول: الفلترة بناءً على النوع "Old" أو "New"
                      allowedCategoryNames.Contains(ct.CategoryName)) // الشرط الثاني: الفلترة بناءً على أسماء التصنيفات المسموح بها
        .ToListAsync();
            // تحويل البيانات إلى SelectList
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
                ActiveQuestionTypeId = TypeId, // قم بتعيين النوع النشط
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
                        // يمكنك هنا إضافة منطق لإنشاء سجل جديد إذا لم يكن موجودًا
                    }
                    TempData["SuccessMessage"] = "تم الحفظ بنجاح";
                    _context.SaveChanges();

                    return Json(new { success = true });
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, message = ex.Message });
                }
            }
            return Json(new { success = false, message = "لا توجد بيانات تحديث." });
        }

        // إنشاء ViewModel لاستقبال البيانات (اختياري ولكن يفضل)
        public class CompanyQuestionContentUpdateViewModel
        {
            public Guid QuestionId { get; set; }
            public int? Score { get; set; }
            public string Inspect { get; set; }
            public string Notes { get; set; }
            public int Level { get; set; }
        }

        // أكشن لحساب النسب المئوية بناءً على CompanyQuestion.Id
        [HttpPost]
        public async Task<IActionResult> UpdateActiveStatus(Guid id, bool active)
        {

            var programmerSettings = _serviceProvider.GetRequiredService<IOptions<ProgrammerSettings>>().Value;
            string connectionString = programmerSettings.DbCon;

            // البحث عن السجل باستخدام الـ ID
            var companyQuestion = await _companyquestion.Entity.GetByIdAsync(id);

            if (companyQuestion == null)
            {
                return NotFound();
            }

            // تحديث قيمة Active
            companyQuestion.Active = active;

            // حفظ التغييرات في قاعدة البيانات
            _context.SaveChanges();

            //---------------------------


            // إعادة توجيه المستخدم إلى نفس الصفحة أو أي صفحة أخرى حسب الحاجة
            return RedirectToAction("Index", "CompanyQuestion", new { id = companyQuestion.UserId });
        }


        [HttpPost]
        public async Task<ActionResult> UpdateActive(Guid companyQuestionId, bool newActiveValue)
        {
            // استدعاء الإجراء المخزن لتحديث "Active"
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
                var Uesr = await _applicationUser.Entity.GetByIdAsync(companyQuestion.UserId);
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
                // تحقق من النتيجة أو إرجاع رسالة
                if (result)
                {


                    //------------------------------------------------

                    StreamReader htmlFile = new StreamReader(filePath);
                    string content = htmlFile.ReadToEnd();
                    htmlFile.Close();
                    //تم استعماله مرتين: مرة ضمن الرسالة ومرة اخرى في عنوان الايميل ولكنه نفس العنوان// Subject
                    content = content.Replace("{Subject}", Uesr.UserName); // يظهر داخل الرسالة
                    content = content.Replace("{Content}", company.Name);
                    content = content.Replace("{SPL}", companyQuestion.SaftyGrid.ToString());
                    content = content.Replace("{COL}", companyQuestion.SqurtyGrid.ToString());
                    content = content.Replace("{CurrentYear}", DateTime.Now.Year.ToString());
                    content = content.Replace("{SiteName}", SiteInfo.Name);
                    content = content.Replace("{Mail}", EmailInfo.Email);
                    content = content.Replace("{Phone}", EmailInfo.Phone);


                    var message = new Message(new string[] { companyQuestion.ApplicationUser.UserName }, "OverSiteUpdate", content, null);

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

            // تحقق من النتيجة أو إرجاع رسالة
            if (result)
            {


                // إعادة التوجيه إلى الفيو أو عرض رسالة النجاح
                return RedirectToAction("Index", "CompanyQuestion", new { type = compayneQuestion.Type });  // إعادة التوجيه لفيو "Index" في Controller "CompanyQuestion"
            }
            else
            {
                // في حالة فشل التحديث، عرض رسالة خطأ
                TempData["ErrorMessage"] = "فشل تحديث الحالة";
                return RedirectToAction("Index", new { type = compayneQuestion.Type });  // العودة إلى الفيو الحالي
            }
        }

        // دالة لاستدعاء الإجراء المخزن في SQL Server
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

                        // إضافة المعطيات إلى الإجراء المخزن
                        cmd.Parameters.AddWithValue("@id", companyQuestionId);
                        cmd.Parameters.AddWithValue("@newActiveValue", newActiveValue);

                        // تنفيذ الإجراء المخزن
                        var rowsAffected = cmd.ExecuteNonQuery(); // ينفذ التحديث ويعيد عدد الأسطر المتأثرة

                        return rowsAffected > 0;  // إذا كان هناك أسطر تم تحديثها بنجاح
                    }
                }
            }
            catch (Exception ex)
            {
                // في حالة حدوث أي خطأ
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

                        // إضافة المعطيات إلى الإجراء المخزن
                        cmd.Parameters.AddWithValue("@id", companyQuestionId);
                        cmd.Parameters.AddWithValue("@newActiveValue", newActiveValue);

                        // تنفيذ الإجراء المخزن
                        var rowsAffected = cmd.ExecuteNonQuery(); // ينفذ التحديث ويعيد عدد الأسطر المتأثرة

                        return rowsAffected > 0;  // إذا كان هناك أسطر تم تحديثها بنجاح
                    }
                }
            }
            catch (Exception ex)
            {
                // في حالة حدوث أي خطأ
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
                // جلب الأسئلة المرتبطة بالتصنيف المحدد فقط
                lstQty = await _companyquestionContent.Entity
                    .Include(s => s.Question.QuestionType.QuestionCategoryType)
                    .Include(s => s.CompanyQuestion.Company)
                    .Include(s => s.CompanyQuestion.ApplicationUser)
                    .Where(s =>
                        s.CompanyQuestionId == Id &&
                        s.Question.QuestionType.QuestionCategoryTypeId == CategoryId &&
                        (s.CompanyQuestion.Type != "New" || s.Inspect == "Ns"))
                    .OrderBy(s => s.Question.QuestionType.TypeName)
                    .ToListAsync();

                // جلب التصنيف المحدد فقط
                lstCategory = await _questioncategorytype.Entity
                    .GetWhere(n => n.Id == CategoryId.Value)
                    .ToListAsync();



            }
            else
            {
                // جلب جميع الأسئلة
                lstQty = await _companyquestionContent.Entity
                    .Include(s => s.Question.QuestionType.QuestionCategoryType)
                    .Include(s => s.CompanyQuestion.Company)
                    .Include(s => s.CompanyQuestion.ApplicationUser)
                    .Where(s =>
                        s.CompanyQuestionId == Id &&
                        (s.CompanyQuestion.Type != "New" || s.Inspect == "Ns"))
                    .OrderBy(s => s.Question.QuestionType.TypeName)
                    .ToListAsync();

                // أولاً: حدد الصلاحيات التي يمتلكها المستخدم الحالي
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
                // ثانياً: قم ببناء قائمة بأسماء التصنيفات المسموح بها بناءً على صلاحيات المستخدم
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
           .GetWhere(ct => ct.Type == companyquestion.Type && // الشرط الأول: الفلترة بناءً على النوع "Old" أو "New"
                         allowedCategoryNames.Contains(ct.CategoryName)) // الشرط الثاني: الفلترة بناءً على أسماء التصنيفات المسموح بها
           .ToList();


            }

            var studentname = lstQty?.FirstOrDefault()?.CompanyQuestion?.ApplicationUser?.UserName ?? "اسم الموظف";
            var createdate = lstQty?.FirstOrDefault()?.CompanyQuestion?.Created ?? DateTime.Now;
            var companyName = lstQty?.FirstOrDefault()?.CompanyQuestion?.Company?.Name ?? "اسم الشركة";

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
                return BadRequest("لا توجد بيانات محدثة.");
            }

            bool hasChanges = false;

            foreach (var item in updates)
            {
                // استرجاع الكيان من قاعدة البيانات
                // الطريقة تعتمد على كيفية إعداد الـ repository أو DbContext
                // المثال التالي يفترض أن _companyquestionContent.Entity.GetByIdAsync موجود
                var questionContent = await _companyquestionContent.Entity.GetByIdAsync(item.Id);
                // أو إذا كنت تستخدم DbContext مباشرة:
                // var questionContent = await _context.CompanyQuestionContents.FindAsync(item.Id);


                if (questionContent != null)
                {
                    bool currentItemChanged = false;

                    if (questionContent.Score != item.Score)
                    {
                        questionContent.Score = item.Score;
                        currentItemChanged = true;
                    }

                    if (questionContent.Nots != item.Nots) // استخدام item.Nots
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
                        // إذا تم تحديد S أو Na، قم بمسح Nots و Level
                        if (item.Inspect == "S" || item.Inspect == "Na") // تصحيح لـ "Na"
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
                        // إذا كنت لا تستخدم Unit of Work pattern يتتبع التغييرات تلقائياً
                        // قد تحتاج لتحديث الكيان بشكل صريح هنا
                        // _companyquestionContent.Entity.Update(questionContent); أو _context.Update(questionContent);
                        // يعتمد على إعداداتك
                        hasChanges = true;
                    }
                }
                else
                {
                    // يمكنك تسجيل خطأ هنا إذا كان من المفترض أن يكون الكيان موجودًا دائمًا
                    Console.WriteLine($"QuestionContent with Id {item.Id} not found for update.");
                }
            }

            if (hasChanges)
            {
                try
                {
                    await _context.SaveChangesAsync(); // أو _unitOfWork.CompleteAsync();
                    return Ok(new { message = "تم حفظ التغييرات بنجاح." });
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    Console.WriteLine($"Concurrency error during save: {ex.Message}");
                    return StatusCode(500, "حدث خطأ أثناء محاولة حفظ البيانات بسبب تحديث متزامن. حاول مرة أخرى.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Generic error during save: {ex.Message}");
                    return StatusCode(500, "حدث خطأ غير متوقع أثناء محاولة حفظ البيانات.");
                }
            }

            return Ok(new { message = "لم يتم العثور على تغييرات للحفظ." });
        }

        public async Task<IActionResult> FilterQuestions(string Type, Guid? companyId, DateTime? dateFrom, DateTime? dateTo, Guid? locationId)
        {
            Console.WriteLine($"Filter Action Called - Type: {Type}, CompanyId: {companyId}"); // Debugging

            // --- 1. بناء الاستعلام الأساسي بالنوع ---
            var query = _companyquestion.Entity
                        .GetWhere(q => q.Type == Type);

            // --- 2. إضافة فلترة الشركة إذا تم تحديد ID صالح ---
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
                // لو أردت تضمين كامل اليوم الأخير، أضف يومًا واحدًا ثم استخدم `<`
                query = query.Where(q => q.Created <= dateTo.Value.Date.AddDays(1).AddTicks(-1));
            }

            // --- 3. تضمين البيانات المرتبطة وتطبيق الترتيب ---
            var filteredQuestions = await query
                                        .Include(q => q.Company)
                                        .Include(q => q.ApplicationUser)
                                        .OrderByDescending(q => q.Created) // نفس الترتيب المستخدم في Index
                                        .ToListAsync();

            // --- 4. جلب الفئات المطلوبة للـ Partial View ---




            var categories = _questioncategorytype
       .Entity
       .GetWhere(ct => ct.Type == Type) // الشرط الثاني: الفلترة بناءً على أسماء التصنيفات المسموح بها
       .ToList();

            // --- 5. تجهيز الـ ViewModel للـ Partial View ---
            // نستخدم نفس الـ ViewModel ولكن نمرر فقط البيانات المطلوبة
            var partialViewModel = new CompanyQuestionVM
            {
                CompanyQuestion = filteredQuestions,
                QuestionCategoryType = categories,
                Company = null // لا نحتاج الشركات هنا
            };

            Console.WriteLine($"Found {filteredQuestions.Count} questions after filter."); // Debugging


            // --- 6. إرجاع الـ Partial View مع البيانات المفلترة ---
            return PartialView("_CompanyQuestionsCards", partialViewModel);
        }




        [HttpGet]
        public async Task<IActionResult> GetCategoriesForCompanyType(Guid? companyId)
        {
            if (companyId == null)
            {
                return Json(new List<object>()); // أو BadRequest()
            }

            // 1. ابحث عن الشركة واحصل على CompanyTypeId الخاص بها
            var company = await _company.Entity.GetByIdAsync(companyId);

            if (company == null || company.CompanyTypeId == null) // تأكد أن الشركة موجودة وأن لديها CompanyTypeId
            {
                return Json(new List<object>()); // لا توجد شركة أو لا يوجد نوع شركة مرتبط
            }

            Guid companyTypeId = company.CompanyTypeId.Value; // .Value لأن CompanyTypeId قد يكون int?

            var categories = await _companytypeCategoryAvailable.Entity
                .GetWhere(cta => cta.CompanyTypeId == companyTypeId)
                .Include(cta => cta.QuestionCategoryType) // قم بتضمين بيانات QuestionCategoryType
                .Select(cta => new // قم بإسقاط البيانات إلى DTO بسيط لـ JSON
                {
                    id = cta.QuestionCategoryType.Id, // أو cta.QuestionCategoryTypeId إذا كان الـ ID هو نفسه
                    categoryName = cta.QuestionCategoryType.CategoryName
                })
                .OrderBy(qct => qct.categoryName) // ترتيب اختياري
                .ToListAsync();

            return Json(categories);
        }




    }


}





