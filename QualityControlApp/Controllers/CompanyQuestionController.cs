using Beiyuan.WebSystem.Collections;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Org.BouncyCastle.Ocsp;
using QualityControlApp.Classes;
using QualityControlApp.Models;
using QualityControlApp.Models.Entities;
using QualityControlApp.Models.Interfaces;
using Microsoft.Extensions.Configuration;
using QualityControlApp.Models.Repositories;
using QualityControlApp.ViewModels;
using ScottPlot;
using ScottPlot.Plottables;
using SelectPdf;
using System.ComponentModel.Design;
using System.Configuration;
using System.Data;
using System.Diagnostics.Contracts;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static System.Collections.Specialized.BitVector32;
using Microsoft.Extensions.Options;

namespace QualityControlApp.Controllers
{
    public class CompanyQuestionController : BaseController
    {

        private readonly IViewHelper _viewHelper;
        private readonly ApplicationDbContext _context;
        private readonly IUnitOfWork<Company> _company;
        private readonly IUnitOfWork<Contact> _contact;
        private readonly IUnitOfWork<SiteInfo> _siteInfo;
        private readonly IUnitOfWork<ApplicationUser> _applicationUser;
        private readonly IUnitOfWork<QuestionCategoryType> _questioncategorytype;
        private readonly IUnitOfWork<CompanyQuestion> _companyquestion;
        private readonly IUnitOfWork<CompanyQuestionContent> _companyquestionContent;
        private readonly IUnitOfWork<Question> _question;
        private readonly IEmailSender _emailSender;
        private readonly IUnitOfWork<QuestionType> _questiontype;
        private readonly IWebHostEnvironment _host;
        private readonly IServiceProvider _serviceProvider;

        public CompanyQuestionController(
            ApplicationDbContext context,
            IEmailSender emailSender,
            IUnitOfWork<Company> company,
            IUnitOfWork<SiteInfo> siteInfo,
            IUnitOfWork<Contact> contact,
            IUnitOfWork<Question> question,
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
            _context = context;
            _siteInfo = siteInfo;
            _company = company;
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
   .Entity
   .GetAll()
   .Where(ct => ct.Type == Type) // إضافة شرط Where هنا
   .ToListAsync();


            var Company = await _company.Entity.GetAll().ToListAsync(); ;
            ViewBag.Type = Type;

            var CompanyQuestionVM = new CompanyQuestionVM
            {
                Company = Company,
                CompanyQuestion = companyquestion,
                QuestionCategoryType = category,
            };



            return View(CompanyQuestionVM);
        }


        // الإجراء لفلترة الأسئلة حسب الشركة
        public IActionResult FilterByCompany(Guid? companyId, string? Type)
        {
            var questions = _companyquestion.Entity.Include(q => q.Company).Include(q => q.ApplicationUser)
                .Where(q => !companyId.HasValue || q.Company.Id == companyId && q.Type== Type)
                .ToList();

            return PartialView("QuestionsTable", questions);
        }
        public async Task<IActionResult> Create(string Type)
        {
            // استرداد بيانات الموظفين والشركات
            var companies = await _company.Entity.GetAll().ToListAsync();
            var applicationUser = await _applicationUser.Entity
                .GetAll().ToListAsync();

            // تحويل البيانات إلى SelectList
            ViewBag.Type = Type;
            ViewBag.Users = new SelectList(applicationUser, "Id", "UserName");
            ViewBag.Companies = new SelectList(companies, "Id", "Name");

            return View();
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



        [HttpPost, ActionName("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCompanyQuestion(Guid companyId, string UserId, string Type)
        {

            //------------------------------------------------


            var filePath = _host.WebRootPath + "\\templates" + "\\StartCompanyQuestion.html";

            var Uesr = await _applicationUser.Entity.GetByIdAsync(UserId);
            if (Uesr == null)
            {
                return NotFound();
            }

            var company = await _company.Entity.GetByIdAsync(companyId);
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

            StreamReader htmlFile = new StreamReader(filePath);
            string content = htmlFile.ReadToEnd();
            htmlFile.Close();
            //تم استعماله مرتين: مرة ضمن الرسالة ومرة اخرى في عنوان الايميل ولكنه نفس العنوان// Subject
            content = content.Replace("{Subject}", Uesr.UserName); // يظهر داخل الرسالة
            content = content.Replace("{Content}", company.Name);

            var message = new Message(new string[] { EmailInfo.Email }, Uesr.UserName, content, null);

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



            //--------------------------------------------------


            var newQuestion = new CompanyQuestion
            {
                CompanyId = companyId,
                UserId = UserId,
                Active = false,
                SaftyGrid = 0,
                SqurtyGrid = 0,
                Type = Type,
                Created = DateTime.Now
            };

            _context.CompanyQuestion.Add(newQuestion);
            await _context.SaveChangesAsync();

            //var questions = await _question.Entity.GetAll().ToListAsync();

          
var oldQuestions = await _question.Entity
    .Include(q => q.QuestionType) 
        .Include(qt => qt.QuestionType.QuestionCategoryType) 
    .Where(q => q.QuestionType != null && q.QuestionType.QuestionCategoryType != null && q.QuestionType.QuestionCategoryType.Type == Type) // Filter where the Type is "Old"
    .ToListAsync(); 

            foreach (var question in oldQuestions)
            {
                var questionResponse = new CompanyQuestionContent
                {
                    CompanyQuestionId = newQuestion.Id,
                    QuestionId = question.Id,
                    Score = 0,
                    Level = null,
                    Nots = null,
                    Inspect = null,
                    Created = DateTime.Now
                };
                _context.CompanyQuestionContent.Add(questionResponse);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Index", new { Type = Type });
        }

        public async Task<IActionResult> Details(Guid id, Guid CategoryId)
        {

            var companyquestion = await _companyquestion.Entity
                .GetByIdAsync(id);
            if (companyquestion == null)
            {
                return View("NotFound");
            }



            var questioncategory = _questioncategorytype.Entity
              .Include(n => n.QuestionType)
               .Where(n => n.Id == CategoryId && n.Type == companyquestion.Type).FirstOrDefault();


            //        var questioncategoryAsync = await _context.QuestionCategoryType // استخدام await
            //.Include(n => n.QuestionType)
            //.Where(n => n.Id == CategoryId && n.Type == companyquestion.Type)
            //.FirstOrDefaultAsync(); // استخدام النسخة غير المتزامنة
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
            var category = await _questioncategorytype
    .Entity
    .GetAll()
    .Where(ct => ct.Type == companyquestion.Type) // إضافة شرط Where هنا
    .ToListAsync();
            var user = await _applicationUser.Entity.GetAll().ToListAsync();

            // تحويل البيانات إلى SelectList
            ViewBag.Companies = new SelectList(companies, "Id", "Name", companyquestion.CompanyId);
            ViewBag.Users = new SelectList(user, "Id", "UserName", companyquestion.UserId);



            var CompanyQuestionContentVM = new CompanyQuestionContentVM
            {
                CompanyQuestion = companyquestion,
                CompanyQuestionContent = ContentList,
                QuestionType = TypeList,
                QuestionCategoryType = category,
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
            var chartData = new
            {
                Labels = labels,
                PercentageScores = percentageScores,
                MaxScores = maxScores
            };
            ViewBag.CompanyQuestionId = id;
            ViewBag.TypeId = TypeId;
            // إرسال البيانات إلى الـ View
            ViewBag.ChartData = JsonConvert.SerializeObject(chartData); // تحويل البيانات إلى JSON لتمريرها للـ View

            var TypeList = await _questiontype.Entity.GetWhere(n => n.QuestionCategoryTypeId == Categoryid)
                .ToListAsync();

            var companies = await _company.Entity.GetAll().ToListAsync();
            var user = await _applicationUser.Entity.GetAll().ToListAsync();


            var category = await _questioncategorytype
    .Entity
    .GetAll()
    .Where(ct => ct.Type == companyquestion.Type) // إضافة شرط Where هنا
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
                CompanyQuestion = companyquestion,
                CompanyQuestionContent = ContentList,
                QuestionType = TypeList,
                QuestionCategoryType = category,
            };
            return View("Details", CompanyQuestionContentVM);
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

        // استدعاء جملة الاتصال

        //   private readonly string connectionString = "Server=95.216.93.102;Database=QualityDB;Trusted_Connection=false;uid=AppUser;password=Mahmoud091474;MultipleActiveResultSets=true;TrustServerCertificate=True;";
        // دلوقتي تقدر توصل للكونكشن سترنق من خلال _configuration
        
        //private readonly string connectionString = "Server=DESKTOP-8KCBVKR\\SQLEXPRESS;Database=QualityDB;Trusted_Connection=false;uid=AppUser;password=111;MultipleActiveResultSets=true;TrustServerCertificate=True;";

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
            if (compayneQuestion.Type =="Old")
            {
                result= UpdateCompanyQuestionActiveOld(companyQuestionId, newActiveValue);
            }
            else
            {
             result = UpdateCompanyQuestionActiveNew(companyQuestionId, newActiveValue);

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
            //string filePath1 = Directory.GetCurrentDirectory() + "\\templates\\Confirm.html";
            //var filePath = _host.WebRootPath
            //                + Path.DirectorySeparatorChar + "templates"
            //                + Path.DirectorySeparatorChar + "EmailBootstrapTemplate.html";


            return View();

        }



        public async Task<ActionResult> PrintReport(Guid Id)
        {
            var companyquestion =await _companyquestion.Entity.GetWhere(s => s.Id == Id).FirstOrDefaultAsync();
            //var lstCategory = await _context.QuestionCategoryType.Getw.ToListAsync();
            var lstCategory = await _questioncategorytype .Entity .GetWhere(n => n.Type== companyquestion.Type).ToListAsync();
            var lstQty = await _companyquestionContent.Entity
                .Include(s => s.Question.QuestionType.QuestionCategoryType)
                .Include(s => s.CompanyQuestion.Company)
                .Include(s => s.CompanyQuestion.ApplicationUser)
                .Where(s => s.CompanyQuestionId == Id)
                .OrderBy(s => s.Question.QuestionType.TypeName)
                .ToListAsync();
            //
            var studentname = lstQty?.FirstOrDefault()?.CompanyQuestion?.ApplicationUser?.UserName ?? "اسم الموظف";
            var createdate = lstQty?.FirstOrDefault()?.CompanyQuestion?.Created ?? DateTime.Now;
            var CompanyName = lstQty?.FirstOrDefault()?.CompanyQuestion?.Company?.Name ?? "اسم الشركة";

            //
            var vm = new RepUserCompanyQuestionVM()
            {
               
                CompanyName = CompanyName,
                CreateDate = createdate.ToString("d"),
                UserName = studentname,
                lstCompanyQuestionContent = lstQty,
                lstQuestionCategoryType = lstCategory
            };

            //create pdf
            var converter = new HtmlToPdf();
            var fullView = new HtmlToPdf();
            // Render the view to a string
            var viewContent = _viewHelper.RenderViewToString("~/Views/Report/UserCompanyQuestion.cshtml", vm);
            //var viewChart1 = _viewHelper.RenderViewToString("~/Views/Report/UserCompanyChart1.cshtml", vm);
            //var viewChart2 = _viewHelper.RenderViewToString("~/Views/Report/UserCompanyChart2.cshtml", vm);
            ////
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
                return BadRequest("لا توجد بيانات محدثة.");

            foreach (var item in updates)
            {
                var questionContent = await _companyquestionContent.Entity.GetByIdAsync(item.Id);
                if (questionContent != null)
                {
                    if (item.Score != null)
                        questionContent.Score = item.Score;
                    if (!string.IsNullOrEmpty(item.Notes))
                        questionContent.Nots = item.Notes;

                    if (item.Level.HasValue)
                        questionContent.Level = item.Level.Value;

                    if (!string.IsNullOrEmpty(item.Inspect))
                    {
                        questionContent.Inspect = item.Inspect;

                        if (item.Inspect =="N")
                        {
                            questionContent.Level = null;
                            questionContent.Nots = null;
                        }
                      

                    }




                }
            }

            await _context.SaveChangesAsync();
            return Ok();
        }




    }





}
