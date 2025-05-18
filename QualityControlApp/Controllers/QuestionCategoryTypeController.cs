using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QualityControlApp.Classes;
using QualityControlApp.Models.Entities;
using QualityControlApp.Models.Interfaces;
using QualityControlApp.Models;

namespace QualityControlApp.Controllers
{
   

        [Authorize(Policy = "ProgOrAdminOrEmployeePolicy")]

        [ViewLayout("_LayoutDashboard")]

        public class QuestionCategoryTypeController: BaseController 
        {


            private readonly ApplicationDbContext _context;
            private readonly IUnitOfWork<QuestionCategoryType> _questioncategoryType;

            public QuestionCategoryTypeController(ApplicationDbContext context,
                                      IUnitOfWork<QuestionCategoryType> questioncategoryType,
                              IWebHostEnvironment host) : base(host)
            {
                _context = context;
                _questioncategoryType = questioncategoryType;
            }
            [Route("QuestionCategoryType")]
            public async Task<IActionResult> Index()
            {


                var QuestionCategoryType = await _questioncategoryType.Entity.GetAll() .ToListAsync();

                ViewData["QuestionTypeCount"] = QuestionCategoryType.Count();


                return View(QuestionCategoryType);

            }

            //[ViewLayout("_Layout")]
            public async Task<IActionResult> Details(Guid? id)
            {
                if (id == null) // عند مسح خانة من المعرف
                {
                    //return NotFound();  // 404
                    return View("NotFound");
                }

                var QuestionCategoryType = await _questioncategoryType.Entity.GetByIdAsync(id);
                if (QuestionCategoryType == null)
                {
                    ViewBag.Message = "QuestionCategoryType details is not found";
                    return View("NotFound");
                }

                return View(QuestionCategoryType);
            }


            public IActionResult Create()
            {
                return View();
            }


            [HttpPost]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> Create([Bind("CategoryName,Type")] QuestionCategoryType QuestionCategoryType)
            {
                if (ModelState.IsValid)
                {
                    try
                    {

                        var exists = await _questioncategoryType .Entity
                                          .GetWhere(a => a.CategoryName  == QuestionCategoryType.CategoryName ) // لا يهم التحويل الى حروف كبيرة لأنه غير حساس لحالة الأحرف
                                          .FirstOrDefaultAsync();

                        if (exists != null)
                        {
                            ViewBag.Message = $"CategoryName '{exists.CategoryName }' has already been added";

                            return View();
                        }
                        QuestionCategoryType.CategoryName  = QuestionCategoryType.CategoryName.Trim();
                        QuestionCategoryType.Type  = QuestionCategoryType.Type.Trim();
                        QuestionCategoryType.Created = DateTime.Now;
                        QuestionCategoryType.Modified = DateTime.Now;
                        _questioncategoryType .Entity.Insert(QuestionCategoryType);
                        await _questioncategoryType.SaveAsync();
                        return RedirectToAction(nameof(Index));
                    }
                    catch
                    {
                        throw;
                    }
                }
                return View(QuestionCategoryType);
            }

            [AcceptVerbs("Get", "Post")]
            public async Task<JsonResult> NameExists(string name)
            {
                var exists = await _questioncategoryType.Entity.GetAll()
                                      .FirstOrDefaultAsync(n => n.CategoryName == name.Trim()); // لا يهم التحويل الى حروف كبيرة لأنه غير حساس لحالة الأحرف

                if (exists == null)
                {
                    return Json(true);
                }
                else
                {
                    return Json($"The questionType '{exists.CategoryName}' is already in use");
                }
            }


            public async Task<IActionResult> Edit(Guid? id)
            {
                if (id == null)
                {
                    return View("NotFound");
                }

                var QuestionType = await _questioncategoryType .Entity.GetByIdAsync(id);

                if (QuestionType == null)
                {
                    ViewBag.Message = "The questionType Is Not Found";
                    return View("NotFound");
                }
                return View(QuestionType);
            }

            [HttpPost]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> Edit(Guid id, [Bind("TypeName,Id,Created,Modified,Type")] QuestionCategoryType QuestionCategoryType)
            {
                //لمنع حدوث هجمات CSRF (Cross-Site Request Forgery):
                if (id != QuestionCategoryType.Id)
                {
                    return View("NotFound");
                }

                //if (ModelState.IsValid)
                //{
                try
                {
                    var QuestionCategoryTypeNameExists = await _questioncategoryType .Entity
                                            .GetWhere(a => a.CategoryName  == QuestionCategoryType.CategoryName.Trim() & a.Id != QuestionCategoryType.Id)
                                            .FirstOrDefaultAsync();

                    if (QuestionTypeNameExists != null)
                    {
                        ViewBag.Message = $"QuestionType '{QuestionCategoryType.CategoryName}' has already been added";
                        return View(QuestionCategoryType);
                    }

                    _questioncategoryType .Entity.Update(QuestionCategoryType);
                    await _questioncategoryType.SaveAsync();
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    if (!QuestionTypeNameExists(QuestionCategoryType.Id))
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
                return (_questioncategoryType .Entity.GetAll()?.Any(e => e.Id == id)).GetValueOrDefault();

            }


            public async Task<IActionResult> Delete(Guid? id)
            {
                if (id == null)
                {
                    return View("NotFound");
                }

                var QuestionCategoryType = await _questioncategoryType.Entity.GetByIdAsync(id);
                if (QuestionCategoryType == null)
                {
                    return View("NotFound");
                }

                return View(QuestionCategoryType);

            }

            [HttpPost, ActionName("Delete")]
            [ValidateAntiForgeryToken]
            //public async Task<IActionResult> DeleteConfirmed(Guid id,string name , string test)
            public async Task<IActionResult> DeleteConfirmed(Guid id)
            {
                var QuestionCategoryType = await _questioncategoryType .Entity.GetByIdAsync(id);
                if (QuestionCategoryType == null)
                {
                    return View("NotFound");
                }

                try
                {
                    _questioncategoryType .Entity.Delete(QuestionCategoryType);
                    await _questioncategoryType .SaveAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch
                {
                    throw;
                }
            }


        }
    }

