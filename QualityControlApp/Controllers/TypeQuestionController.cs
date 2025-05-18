using Microsoft.AspNetCore.Mvc;
using QualityControlApp.Models.Interfaces;
using QualityControlApp.Models;
using static System.Collections.Specialized.BitVector32;
using QualityControlApp.Models.Entities;
using Microsoft.EntityFrameworkCore;
using QualityControlApp.Classes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace QualityControlApp.Controllers
{

    [Authorize(Policy = "ProgOrAdminOrEmployeePolicy")]

    [ViewLayout("_LayoutDashboard")]

    public class TypeQuestionController : Controller
    {


        private readonly ApplicationDbContext _context;
        private readonly IUnitOfWork<QuestionType> _questionType;
        private readonly IUnitOfWork<QuestionCategoryType> _questioncategoryType;

        public TypeQuestionController (ApplicationDbContext context,
                                  IUnitOfWork<QuestionType> questionType,
                                  IUnitOfWork<QuestionCategoryType> questioncategoryType)
        {
            _context = context;
            _questionType = questionType;
            _questioncategoryType = questioncategoryType;
        }
        // GET: QuestionType
        [Route("QuestionType")]
        public async Task<IActionResult> Index()
        {

           
            var QuestionType = await _questionType .Entity.Include(n => n.QuestionCategoryType).OrderBy(s => s.QuestionCategoryType.CategoryName ).ToListAsync();

            ViewData["QuestionTypeCount"] = QuestionType.Count();
          

            return View(QuestionType);

        }

        //[ViewLayout("_Layout")]
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null) // عند مسح خانة من المعرف
            {
                //return NotFound();  // 404
                return View("NotFound");
            }

            ViewData["QuestionCategoryType"] =
               new SelectList(await _questioncategoryType.Entity.GetAll().ToListAsync(), "Id", "CategoryName");

            var QuestionType = await _questionType.Entity.GetByIdAsync(id);
            if (QuestionType == null)
            {
                ViewBag.Message = "questionType details is not found";
                return View("NotFound");
            }

            return View(QuestionType);
        }


        public async Task<IActionResult> CreateAsync()
        {
            ViewData["QuestionCategoryType"] =
            new SelectList(await _questioncategoryType.Entity.GetAll().ToListAsync(), "Id", "CategoryName");

            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TypeName,QuestionCategoryTypeId")] QuestionType QuestionType)
        {
            if (ModelState.IsValid)
            {
                try

                {
                    
                    var exists = await _questionType.Entity
                                      .GetWhere(a => a.TypeName == QuestionType.TypeName) // لا يهم التحويل الى حروف كبيرة لأنه غير حساس لحالة الأحرف
                                      .FirstOrDefaultAsync();

                    if (exists != null)
                    {
                        ViewBag.Message = $"QuestionType '{exists.TypeName}' has already been added";
                      
                        return View();
                    }
                    QuestionType.TypeName = QuestionType.TypeName.Trim();
                    QuestionType.QuestionCategoryTypeId = QuestionType.QuestionCategoryTypeId;
                    QuestionType.Created = DateTime.Now ;
                    QuestionType.Modified  = DateTime.Now;
                    _questionType.Entity.Insert(QuestionType);
                    await _questionType.SaveAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch
                {
                    throw;
                }
            }
            return View(QuestionType);
        }

        [AcceptVerbs("Get", "Post")]
        public async Task<JsonResult> NameExists(string name)
        {
            var exists = await _questionType.Entity.GetAll()
                                  .FirstOrDefaultAsync(n => n.TypeName == name.Trim()); // لا يهم التحويل الى حروف كبيرة لأنه غير حساس لحالة الأحرف

            if (exists == null)
            {
                return Json(true);
            }
            else
            {
                return Json($"The questionType '{exists.TypeName}' is already in use");
            }
        }


        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return View("NotFound");
            }
            ViewData["QuestionCategoryType"] =
            new SelectList(await _questioncategoryType.Entity.GetAll().ToListAsync(), "Id", "CategoryName");

            var QuestionType = await _questionType.Entity.GetByIdAsync(id);

            if (QuestionType == null)
            {
                ViewBag.Message = "The questionType Is Not Found";
                return View("NotFound");
            }
            return View(QuestionType);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("TypeName,Id,Created,Modified,QuestionCategoryTypeId")] QuestionType QuestionType)
        {
            //لمنع حدوث هجمات CSRF (Cross-Site Request Forgery):
            if (id != QuestionType.Id)
            {
                return View("NotFound");
            }

            //if (ModelState.IsValid)
            //{
                try
                {
                    var QuestionTypeNameExists = await _questionType .Entity
                                            .GetWhere(a => a.TypeName == QuestionType.TypeName.Trim() & a.Id != QuestionType.Id)
                                            .FirstOrDefaultAsync();

                    if (QuestionTypeNameExists != null)
                    {
                        ViewBag.Message = $"QuestionType '{QuestionType.TypeName}' has already been added";
                        return View(QuestionType);
                    }

                    _questionType .Entity.Update(QuestionType);
                    await _questionType .SaveAsync();
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    if (!QuestionTypeNameExists(QuestionType.Id))
                    {
                        return View("NotFound"); // في حال غير موجود
                    }
                    else
                    {
                        //throw; //  صفحة الخطأ الإفتراضية للمتصفح
                        ViewBag.ErrorTitle = "The basic data not found in the database ";
                        ViewBag.ErrorMessage = "Missing data row- " + ex;
                        return View("Error");
                    }

                }
                return RedirectToAction(nameof(Index));
         
        }

        private bool QuestionTypeNameExists(Guid id)
        {
            return (_questionType .Entity.GetAll()?.Any(e => e.Id == id)).GetValueOrDefault();

        }


        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return View("NotFound");
            }
            ViewData["QuestionCategoryType"] =
            new SelectList(await _questioncategoryType.Entity.GetAll().ToListAsync(), "Id", "CategoryName");

            var QuestionType = await _questionType.Entity.GetByIdAsync(id);
            if (QuestionType == null)
            {
                return View("NotFound");
            }

            return View(QuestionType);

        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        //public async Task<IActionResult> DeleteConfirmed(Guid id,string name , string test)
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var QuestionType = await _questionType .Entity.GetByIdAsync(id);
            if (QuestionType == null)
            {
                return View("NotFound");
            }

            try
            {
                _questionType.Entity.Delete(QuestionType);
                await _questionType.SaveAsync();
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                throw;
            }
        }


    }
}