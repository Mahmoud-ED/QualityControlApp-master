using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using QualityControlApp.Classes;
using QualityControlApp.Models;
using QualityControlApp.Models.Entities;
using QualityControlApp.Models.Interfaces;
using QualityControlApp.ViewModels;
using System.Configuration;
using System.Data;
using System.Diagnostics.Contracts;

namespace QualityControlApp.Controllers
{
    public class EmployeeController : BaseController
    {
        private readonly ApplicationDbContext _context;
        private readonly IUnitOfWork<Company> _company;
        private readonly IUnitOfWork<Contact> _contact;
        private readonly IUnitOfWork<SiteInfo> _siteInfo;
        private readonly IWebHostEnvironment _host;
        private readonly IUnitOfWork<ApplicationUser> _applicationuser;
        private readonly IUnitOfWork<QuestionCategoryType> _questioncategorytype;
        private readonly IUnitOfWork<CompanyQuestion> _companyquestion;
        private readonly IUnitOfWork<CompanyQuestionContent> _companyquestionContent;
        private readonly IUnitOfWork<Question> _question;
        private readonly IEmailSender _emailSender;
        private readonly IUnitOfWork<QuestionType> _questiontype;
        private readonly IServiceProvider _serviceProvider;

        public EmployeeController(
                         ApplicationDbContext context,
                         IEmailSender emailSender,
                         UserManager<ApplicationUser> userManager,
                         IWebHostEnvironment host,
                               IConfiguration configuration,
                            IServiceProvider serviceProvider,
                         IUnitOfWork<SiteInfo> siteInfo,
                         IUnitOfWork<Contact> contact,
                         IUnitOfWork<ApplicationUser> applicationuser,
                         IUnitOfWork<Company> company,
                         IUnitOfWork<Question> question,
                         IUnitOfWork<QuestionCategoryType> questioncategorytype,
                         IUnitOfWork<QuestionType> questiontype,
                         IUnitOfWork<CompanyQuestion> companyquestion,
                         IUnitOfWork<CompanyQuestionContent> companyquestionContent
                                ) : base(host, configuration)
        {

            _questiontype = questiontype;
            _question = question;
            _context = context;
            _emailSender = emailSender;
            _contact = contact;
            _siteInfo = siteInfo;
            _serviceProvider = serviceProvider;
            _company = company;
            _applicationuser = applicationuser;
            _companyquestion = companyquestion;
            _companyquestionContent = companyquestionContent;
            _questioncategorytype = questioncategorytype;
            _host = host;
        }
        [ViewLayout("_LayoutEmployee")]

        public async Task<IActionResult> Index(Guid Id)
        {



            var companyquestion = await _context.CompanyQuestionAssignedUsers
 .Where(a => a.AssignedUsersId == Id.ToString())
 .Include(a => a.CompanyQuestion)
     .ThenInclude(q => q.Company)
 .Include(a => a.CompanyQuestion)
     .ThenInclude(q => q.Creator)
 .Select(a => a.CompanyQuestion)
 .OrderBy(q => q.Active)
 .ThenByDescending(q => q.Created)
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


            var category = await _questioncategorytype
        .Entity
        .GetWhere(ct => allowedCategoryNames.Contains(ct.CategoryName)) // ????? ??????: ??????? ????? ??? ????? ????????? ??????? ???
        .ToListAsync();



            var Company = await _company.Entity.GetAll().ToListAsync(); ;

            var CompanyQuestionVM = new CompanyQuestionVM
            {
                Company = Company,
                CompanyQuestion = companyquestion,
                QuestionCategoryType = category,
            };



            return View(CompanyQuestionVM);

            //  var companyquestion = await _companyquestion.Entity
            //.Include(q => q.Company)  // ????? ??????? ??????
            //.Where(n => n.UserId == Id)
            //.Where(n => n.Active == false)
            //.OrderByDescending(q => q.Created)
            //.ToListAsync();
            //  return View(companyquestion);

        }

        public async Task<IActionResult> Details(Guid id, Guid CategoryId)
        {

            var companyquestion = await _companyquestion.Entity
                .GetByIdAsync(id);
            if (companyquestion == null)
            {
                return View("NotFound");
            }



            //var questioncategory = _questioncategorytype.Entity
            //  .Include(n => n.QuestionType)
            //   .Where(n => n.Id == CategoryId && n.Type == companyquestion.Type).FirstOrDefault();




            //-------------------------

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


            var questioncategory = await _questioncategorytype
       .Entity // ????? ?? ??? ?? DbSet<QuestionCategoryType> ?? IQueryable<QuestionCategoryType>
       .Include(n => n.QuestionType) // ?????? ????? ??????? ???????? ??? ????? ??? ????
       .Where(ct => ct.Type == companyquestion.Type && // 1. ????? ??? ????? (??? "Old")
                      allowedCategoryNames.Contains(ct.CategoryName)) // 2. ????? ??? ????? ????????? ??????? ???
       .ToListAsync();


            //-------------------------













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
            // ?????: ??? ????????? ???? ??????? ???????? ??????
             hasOPPerm = false;
             hasAirPerm = false;
             hasBELPerm = false;

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
             allowedCategoryNames = new List<string>();
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

            var user = await _applicationuser.Entity.GetAll().ToListAsync();

            // ????? ???????? ??? SelectList
            ViewBag.Companies = new SelectList(companies, "Id", "Name", companyquestion.CompanyId);
            ViewBag.Users = new SelectList(user, "Id", "UserName", companyquestion.CreatorId);



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
            return (_questioncategorytype.Entity.GetAll()?.Any(e => e.Id == id)).GetValueOrDefault();
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
            var chartData = new
            {
                Labels = labels,
                PercentageScores = percentageScores,
                MaxScores = maxScores
            };
            ViewBag.CompanyQuestionId = id;
            ViewBag.TypeId = TypeId;
            // ????? ???????? ??? ??? View
            ViewBag.ChartData = JsonConvert.SerializeObject(chartData); // ????? ???????? ??? JSON ???????? ??? View

            var TypeList = await _questiontype.Entity.GetWhere(n => n.QuestionCategoryTypeId == Categoryid)
                .ToListAsync();

            var companies = await _company.Entity.GetAll().ToListAsync();
            var user = await _applicationuser.Entity.GetAll().ToListAsync();


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
            ViewBag.Users = new SelectList(user, "Id", "UserName", companyquestion.CreatorId);

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
        public IActionResult SaveScores(Dictionary<string, string> scores)
        {
            // ???? ?? ?? ????? ???? ?? ??????? ???? ?????
            if (scores == null || !scores.Any())
            {
                return Json(new { success = false });
            }

            try
            {
                // ???? ?? ???? ????? ?? ????? ???????? ????? ??? ??? ID
                foreach (var score in scores)
                {
                    var questionId = Guid.Parse(score.Key);  // ????? ID ?????? ?? String ??? Guid
                    var newScore = int.Parse(score.Value);    // ??? ?????? ??????? ???????? ??? int

                    // ????? ?? ?????? ?? ????? ???????? ????? ??? ??? ID
                    var question = _context.CompanyQuestionContent.FirstOrDefault(q => q.Id == questionId);
                    if (question != null)
                    {
                        // ????? ??????
                        question.Score = newScore;
                    }
                }

                // ??? ????????? ?? ????? ????????
                _context.SaveChanges();

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                // ?? ???? ???? ?? ???
                return Json(null);
            }
        }



        // ???? ????? ????? ??????? ????? ??? CompanyQuestion.Id
        [HttpPost]
        public async Task<IActionResult> UpdateActiveStatus(Guid id, bool active)
        {
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


            // ????? ????? ???????? ??? ??? ?????? ?? ?? ???? ???? ??? ??????
            return RedirectToAction("Index", new { id = companyQuestion.CreatorId });
        }


        [HttpPost]
        public async Task<ActionResult> UpdateActive(Guid companyQuestionId, bool newActiveValue)
        {
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
            var Uesr = await _applicationuser.Entity.GetByIdAsync(companyQuestion.CreatorId);
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


                var message = new Message(new string[] { EmailInfo.Email }, "OverSiteUpdate", content, null);


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

                //-----------------------

                // ????? ??????? ??? ????? ?? ??? ????? ??????
            }
            else
            {
                // ?? ???? ??? ???????? ??? ????? ???
                TempData["ErrorMessage"] = "??? ????? ??????";
            }
        
            
            
            
            
            
            var UseriD = _companyquestion.Entity.GetWhere(n => n.Id == companyQuestionId).Select(n => n.UserId)
.FirstOrDefault();
            return RedirectToAction("Index", "Employee", new { id = UseriD });  // ????? ??????? ???? "Index" ?? Controller "Employee"

        }

        // ???? ???????? ??????? ?????? ?? SQL Server
        private  bool UpdateCompanyQuestionActiveOld(Guid companyQuestionId, bool newActiveValue)
        {
            if (newActiveValue == true)
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
            else
            {
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













    }
}



