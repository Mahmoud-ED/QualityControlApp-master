using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QualityControlApp.Classes;
using QualityControlApp.Models.Entities;
using QualityControlApp.Models.Interfaces;
using QualityControlApp.ViewModels;
using System.Text.RegularExpressions;

namespace QualityControlApp.Controllers
{
    [ViewLayout("_LayoutDashboard")]

    [Authorize(Roles = "Admin,Prog,Employee")]
    public class AdminController : Controller
    {
        private readonly IUnitOfWork<SiteState> _siteState;
        private readonly IUnitOfWork<SiteInfo> _siteInfo;
        private readonly IUnitOfWork<Contact> _contact;
        private readonly IUnitOfWork<Company> _company;
        private readonly IUnitOfWork<Landing> _landing;
        private readonly IUnitOfWork<AirPortRequest> _airPortRequest;
        private readonly IUnitOfWork<CompanyQuestion> _companyQuestion;
        private readonly IUnitOfWork<ApplicationUser> _applicationUser;
        private readonly IUnitOfWork<Question> _question;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly UserSessionTracker _userSessionTracker;
        private readonly IWebHostEnvironment _host;
        private readonly IMapper _mapper;


        public AdminController(IUnitOfWork<SiteState> siteState,
                               IUnitOfWork<SiteInfo> siteInfo,
                               IUnitOfWork<Contact> contact,
                               IUnitOfWork<Landing> landing,
                               IUnitOfWork<AirPortRequest> airPortReques,
                               IUnitOfWork<CompanyQuestion> companyQuestion,
                               IUnitOfWork<ApplicationUser> applicationUser,
                               IUnitOfWork<Company> company,
                               IUnitOfWork<Question> question,
                               UserManager<ApplicationUser> userManager,
                                UserSessionTracker userSessionTracker,
                               IWebHostEnvironment host,
                               IMapper mapper)
        {
            _siteState = siteState;
            _siteInfo = siteInfo;
            _contact = contact;
            _companyQuestion = companyQuestion;
            _applicationUser = applicationUser;
            _airPortRequest = airPortReques;
            _landing = landing;
            _company = company;
            _question = question;
            _userManager = userManager;
            _userSessionTracker = userSessionTracker;
            _host = host;
            _mapper = mapper;
        }


        [HttpGet]
        [Route("Dashboard")]
        public async Task<IActionResult> Index(Guid? companyId, DateTime? fromDate, DateTime? toDate)
        {
            ViewBag.usersCount = await _userManager.Users.CountAsync();
            ViewBag.ActiveUsers = _userSessionTracker.ActiveUsers;
            ViewBag.companies = await _company.Entity.GetAll().CountAsync();
            ViewBag.question = await _question.Entity.GetAll().CountAsync();
            ViewBag.AirRequest = await _airPortRequest.Entity.GetWhere(s => s.RequestStatus == "Pending").CountAsync();
            ViewBag.landing = await _landing.Entity.GetWhere(s => s.RequestStatus == "Pending").CountAsync();


            var usersInRoleOps = await _userManager.GetUsersInRoleAsync("OpsPerm");
            ViewBag.Ops = usersInRoleOps.Count;

            var usersInRoleAir = await _userManager.GetUsersInRoleAsync("AirPerm");
            ViewBag.Air = usersInRoleAir.Count;

            var usersInRoleAmo145 = await _userManager.GetUsersInRoleAsync("Amo145");
            ViewBag.Amo145 = usersInRoleAmo145.Count;

            var Bel = await _userManager.GetUsersInRoleAsync("BELPerm");
            ViewBag.Bel = Bel.Count;

            ViewBag.CompaniesList = await _company.Entity.GetAll().Select(c => new { c.Id, c.Name }).ToListAsync(); // اختر فقط Id و Name

            var questionsQuery = _companyQuestion.Entity.GetAll()
                .Where(q => q.Type == "New");

            if (companyId.HasValue)
                questionsQuery = questionsQuery.Where(q => q.CompanyId == companyId.Value);

            if (fromDate.HasValue)
                questionsQuery = questionsQuery.Where(q => q.Created >= fromDate.Value);

            if (toDate.HasValue)
                questionsQuery = questionsQuery.Where(q => q.Created <= toDate.Value.AddDays(1).AddTicks(-1)); // ليشمل نهاية اليوم المحدد

            ViewBag.ActiveQuestions = await questionsQuery.CountAsync(q => q.Active == true);
            ViewBag.InactiveQuestions = await questionsQuery.CountAsync(q => q.Active == false);

            ViewBag.SelectedCompanyId = companyId?.ToString();
            // إذا كنت تستخدم companyId في الفلتر، تأكد من تمرير القيمة الحالية من الفلتر
            // ViewBag.SelectedFromDate = fromDate?.ToString("yyyy-MM-dd");
            // ViewBag.SelectedToDate = toDate?.ToString("yyyy-MM-dd");


            return View();
        }


        [HttpGet]
        public async Task<IActionResult> GetRequestStatusChartData()
        {
           
            var allPossibleStatuses = new List<string> { "Pending", "Approved", "Rejected" };

            // تجميع الطلبات حسب الحالة وحساب العدد لكل حالة
            var statusCounts = await _airPortRequest.Entity 
                .GetWhere(r => !string.IsNullOrEmpty(r.RequestStatus)) // تجاهل القيم الفارغة إذا وجدت
                .GroupBy(r => r.RequestStatus)
                .Select(g => new { StatusName = g.Key, Count = g.Count() })
                .ToListAsync();

            // إعداد البيانات النهائية للشارت، مع التأكد من وجود كل الحالات
            var chartData = new
            {
                Pending = statusCounts.FirstOrDefault(s => s.StatusName == "Pending")?.Count ?? 0,
                Approved = statusCounts.FirstOrDefault(s => s.StatusName == "Approved")?.Count ?? 0,
                Rejected = statusCounts.FirstOrDefault(s => s.StatusName == "Rejected")?.Count ?? 0
                // يمكنك إضافة أي حالات أخرى هنا بنفس الطريقة إذا توسع نطاق الحالات
            };

            return Json(chartData);
        }



        [HttpGet]
        public async Task<IActionResult> GetLandingRequestStatusChartData()
        {

            var allPossibleStatuses = new List<string> { "Pending", "Approved", "Rejected" };

            // تجميع الطلبات حسب الحالة وحساب العدد لكل حالة
            var statusCounts = await _landing.Entity
                .GetWhere(r => !string.IsNullOrEmpty(r.RequestStatus)) // تجاهل القيم الفارغة إذا وجدت
                .GroupBy(r => r.RequestStatus)
                .Select(g => new { StatusName = g.Key, Count = g.Count() })
                .ToListAsync();

            var chartData = new
            {
                Pending = statusCounts.FirstOrDefault(s => s.StatusName == "Pending")?.Count ?? 0,
                Approved = statusCounts.FirstOrDefault(s => s.StatusName == "Approved")?.Count ?? 0,
                Rejected = statusCounts.FirstOrDefault(s => s.StatusName == "Rejected")?.Count ?? 0
            };

            return Json(chartData);
        }


        // Action جديد للتعامل مع طلبات AJAX الخاصة بال Chart
        [HttpGet]
        public async Task<IActionResult> GetChartData(Guid? companyId, DateTime? fromDate, DateTime? toDate)
        {
            var questionsQuery = _companyQuestion.Entity.GetAll()
                .Where(q => q.Type == "New");

            if (companyId.HasValue && companyId.Value != Guid.Empty) // تحقق من أن companyId ليس فارغاً
                questionsQuery = questionsQuery.Where(q => q.CompanyId == companyId.Value);

            if (fromDate.HasValue)
                questionsQuery = questionsQuery.Where(q => q.Created >= fromDate.Value.Date); // ابدأ من بداية اليوم

            if (toDate.HasValue)
                questionsQuery = questionsQuery.Where(q => q.Created < toDate.Value.Date.AddDays(1)); // حتى نهاية اليوم المحدد (قبل بداية اليوم التالي)


            int ActiveQuestions = await questionsQuery.CountAsync(q => q.Active == true);
            var InactiveQuestions = await questionsQuery.CountAsync(q => q.Active == false);

            // ملاحظة: إذا كنت تستخدم System.Text.Json قد تحتاج إلى JsonSerializerOptions للتعامل مع الأسماء
            // var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            // return Json(new { activeQuestions, inactiveQuestions }, options);
            return Json(new {  ActiveQuestions, InactiveQuestions });
        }



        [Authorize(Roles = "Admin,Prog")]
        public async Task<IActionResult> SiteDetails()
        {
            return View(await GetSiteVM());
        }

        [Authorize(Roles = "Admin,Prog")]
        public async Task<IActionResult> SiteEdit()
        {
            return View(await GetSiteVM());
        }
        public async Task<SiteVM> GetSiteVM()
        {
            var siteInfo = await _siteInfo.Entity.GetAll().FirstOrDefaultAsync();
            var contact = await _contact.Entity.GetAll().FirstOrDefaultAsync();

            if (siteInfo == null || contact == null)
            {
                return new SiteVM();
            }

            var siteVM = new SiteVM
            {
                SiteInfo = siteInfo,
                Contact = contact
            };
            return siteVM;
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SiteEdit(SiteVM siteVM, string? isImg1, string? isImg2)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    string? logoFileName = UploadFile(siteVM.SiteInfo.Logo, siteVM.SiteInfo.LogoUrl, isImg1);
                    string? coverFileName = UploadFile(siteVM.SiteInfo.CoverImage, siteVM.SiteInfo.CoverImageUrl, isImg2);

                    var siteInfo = await _siteInfo.Entity.GetAll().FirstOrDefaultAsync();
                    if (siteInfo != null)
                    {
                        siteInfo.Name = siteVM.SiteInfo.Name;
                        siteInfo.Activity = siteVM.SiteInfo.Activity;
                        siteInfo.About = siteVM.SiteInfo.About;
                        siteInfo.LogoUrl = logoFileName;
                        siteInfo.CoverImageUrl = coverFileName;
                        siteInfo.Created = siteVM.SiteInfo.Created;
                        siteInfo.Modified = DateTime.Now;

                        _siteInfo.Entity.Update(siteInfo);
                    }
                    var contact = await _contact.Entity.GetAll().FirstOrDefaultAsync();
                    if (contact != null)
                    {
                        contact.Email = siteVM.Contact.Email;
                        contact.Phone = siteVM.Contact.Phone;
                        contact.Facebook = siteVM.Contact.Facebook;
                        contact.Twitter = siteVM.Contact.Twitter;
                        contact.Instagram = siteVM.Contact.Instagram;
                        contact.Created = siteVM.Contact.Created;
                        contact.Modified = DateTime.Now;

                        _contact.Entity.Update(contact);
                    }

                    await _siteInfo.SaveAsync();



                    TempData["SuccessMessage"] = "The website data has been successfully saved";
                }
                catch
                {
                    TempData["ErrorMessage"] = "Error Save";
                    throw;
                }

                return RedirectToAction("SiteDetails");

            }

            return RedirectToAction("SiteEdit");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SiteEdit1(SiteVM siteVM, string? isImg1, string? isImg2)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    string? logoFileName = UploadFile(siteVM.SiteInfo.Logo, siteVM.SiteInfo.LogoUrl, isImg1);
                    string? coverFileName = UploadFile(siteVM.SiteInfo.CoverImage, siteVM.SiteInfo.CoverImageUrl, isImg2);


                    var contact = await _contact.Entity.GetAll().FirstOrDefaultAsync();
                    if (contact != null)
                    {
                        contact = _mapper.Map<Contact>(siteVM); // صيغة مختصرة

                        _contact.Entity.Update(contact);
                    }

                    var siteInfo = await _siteInfo.Entity.GetAll().FirstOrDefaultAsync();
                    if (siteInfo != null)
                    {
                        siteInfo = _mapper.Map<SiteVM, SiteInfo>(source: siteVM); // صيغة مطولة

                        siteInfo.LogoUrl = logoFileName;
                        siteInfo.CoverImageUrl = coverFileName;

                        _siteInfo.Entity.Update(siteInfo);
                    }

                    await _siteInfo.SaveAsync();
                    TempData["SuccessMessage"] = "The website data has been successfully saved";

                }
                catch
                {
                    TempData["ErrorMessage"] = "Error Save";
                    throw;
                }
                return RedirectToAction("SiteDetails");
            }

            return RedirectToAction("SiteEdit");
        }
        public string? UploadFile(IFormFile? img, string? imageUrl, string? isImg)
        {
            if (isImg == null) // في حال تم حذف الصورة فقط
            {
                DeleteOldFile(imageUrl);
                return null;
            }

            if (img != null)// في حال تم تحميل صورة جديدة
            {
                DeleteOldFile(imageUrl);

                string pictures = Path.Combine(_host.WebRootPath, "pictures");
                string NewPath = Path.Combine(pictures, img.FileName);
                if (!System.IO.File.Exists(NewPath))
                    img.CopyTo(new FileStream(NewPath, FileMode.CreateNew));

                return img.FileName;
            }
            return imageUrl; // في حال لم يتم تحميل صورة جديدة تبقى الصورة القديمة كما هي
        }
        public void DeleteOldFile(string? imageUrl)
        {
            if (imageUrl != null)
            {
                string picturesPath = Path.Combine(_host.WebRootPath, "pictures");
                string oldPath = Path.Combine(picturesPath, imageUrl);
                if (System.IO.File.Exists(oldPath))
                {
                    GC.Collect(); GC.WaitForPendingFinalizers();
                    System.IO.File.Delete(oldPath);
                }
            }
        }

        [AcceptVerbs("Get", "Post")]
        public async Task<JsonResult> EmailIsValid(Contact contact)
        {
            var regex = new Regex(@"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4})$");

            return Json(regex.IsMatch(contact.Email));

        }
        
        [Authorize(Roles = "Prog")]
        public async Task<IActionResult> SiteState(string? saveMessage)
        {
            ViewBag.Message = saveMessage;

            var siteState = await _siteState.Entity.GetAll().FirstOrDefaultAsync();
            if (siteState == null)
            {
                return View("NotFound");
            }
            return View(siteState);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SiteState(string save, [Bind("ClosingMessage")] SiteState siteStat)
        {
            try
            {
                string? message = null;

                var siteStateEdit = await _siteState.Entity.GetAll().FirstOrDefaultAsync();

                if (siteStateEdit == null)
                {
                    return View("NotFound");
                }

                if (save == "Activating Site")
                {
                    siteStateEdit.State = true;
                }
                else if (save == "Deactivate Site")
                {
                    siteStateEdit.State = false;
                }
                else if (save == "Update colsing message")
                {
                    siteStateEdit.ClosingMessage = siteStat.ClosingMessage;
                    message = "Save Success...";
                }

                siteStateEdit.Modified = DateTime.Now;
                _siteState.Entity.Update(siteStateEdit);
                await _siteState.SaveAsync();

                return RedirectToAction("SiteState", new { saveMessage = message });
            }
            catch
            {
                throw;
            }
        }


    }
}
