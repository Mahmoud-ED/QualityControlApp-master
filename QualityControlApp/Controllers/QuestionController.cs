using Microsoft.AspNetCore.Authorization;
using QualityControlApp.Classes;
using QualityControlApp.Models.Interfaces;
using QualityControlApp.Models;
using static System.Collections.Specialized.BitVector32;
using QualityControlApp.Models.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.CodeAnalysis.Options;
using System;

namespace QualityControlApp.Controllers
{

    [ViewLayout("_LayoutDashboard")]
    //[Authorize(Roles = "Admin,Prog,Employee")] 
    //[Authorize(Policy = "ProgOrAdminOrEmployeePolicy")]
    public class QuestionController : BaseController
    {

        private readonly ApplicationDbContext _context;
        private readonly IUnitOfWork<Question > _question;
        private readonly IUnitOfWork<QuestionType> _questionType;
        //private readonly IWebHostEnvironment _host;

        public QuestionController(
                              ApplicationDbContext context,
                              IUnitOfWork<Question> question,
                              IUnitOfWork<QuestionType> questionType,
                              IWebHostEnvironment host) : base(host)
        {
            _context = context;
            _question = question;
            _questionType = questionType;
            //_host = host; // نفعله فقط لو احتجناه في هذا الكونترولر
        }




        public async Task<IActionResult> Index()
        {
            //var question = _question.Entity.Include(n => n.QuestionType).ToList();

            var question = await _question.Entity.Include(n => n.QuestionType)
                            .OrderByDescending(n => n.Created)
                            .ToListAsync();
            ViewBag.Section = "جميع الاسئلة ";  //ViewComponent في Default يستطيع تمرير قيمة الى صفحة Controller 
            return View(question);

        }

        public async Task<IActionResult> DisplayQuestionType(Guid? questionId)
        {
            if (questionId == null)
            {
                return View("NotFound");
            }

            var question = await _question.Entity.Include(n => n.QuestionType )
                                         .Where(n => n.QuestionTypeId == questionId)
                                         .OrderByDescending(n => n.Created)
                                         .ToListAsync();

            var TypeName = "";
            if (question .Count != 0)
            {
                TypeName = question  .Select(n => n.QuestionType .TypeName).FirstOrDefault();
            }
            else
            {
                TypeName = _questionType .Entity
                            .GetWhere(n => n.Id == questionId)
                            .Select(n => n.TypeName)
                            .FirstOrDefault();
            }

            ViewBag.Section = TypeName;


            return View("Index", question );
        }
        [Authorize(Policy = "CreatePolicy")]
        public async Task<IActionResult> Create()
        {
            ViewData["QuestionType"] =
                new SelectList(await _questionType .Entity.GetAll().ToListAsync(), "Id", "TypeName");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Content,QuestionTypeId,MaxGrid")] Question question)
        {
            //هذه لا تستعمل ابدا مع المفتاح الأجنبي لكن لو احتجنا لها في حقول اخرى
            //ModelState.Remove("Sections");// طريقة أخرى لمنع التحقق داخل وظيفة معينة وليس في الكلاس سيمنع التحقق بالمطلق لكل الوظائف

            if (ModelState.IsValid)
            {
                try
                {


                    question.Created = DateTime.Now;
                    _question.Entity.Insert(question);
                    await _question.SaveAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception)
                {
                    throw;
                }
            }
            return View();
        }


        [Authorize(Policy = "EditPolicy")]

        public async Task<IActionResult> Edit(Guid? id)
        {

            if (id == null)
            {
                return View("NotFound");
            }

            //var news1= _news.Entity.Include(s => s.Sections).GetByIdAsync(id);

            var question  = await _question .Entity.Include(s => s.QuestionType )
                                         .Where(n => n.Id == id)
                                         .FirstOrDefaultAsync();
            if (question == null)
            {
                return View("NotFound");
            }

            ViewData["QuestionType"] = new SelectList(await _questionType .Entity.GetAll().ToListAsync(), "Id", "TypeName", question.QuestionTypeId );
           
            return View(question);

        }


            //<div class="form-group">
            //        <label asp-for="QuestionTypeId " class="control-label"></label>
            //        <select asp-for="QuestionTypeId" class="form-control" asp-items="ViewBag.QuestionType">
            //            <option value = "" disabled selected>---------Select Type---------</option>
            //        </select>
            //        <span asp-validation-for="QuestionTypeId" class="text-danger"></span>
            //    </div>

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Content,QuestionTypeId,MaxGrid,Id,Created")] Question question)
        {

            // (Cross-Site Request Forgery)

            if (id != question.Id)
            {
                return View("NotFound");
            }

            //if (ModelState.IsValid)
            //{

                try
                {
                    question .Modified = DateTime.Now;
                    _question.Entity.Update(question);
                    await _question.SaveAsync();
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    if (!questionExists(question.Id)) //في حال محذوف
                    {
                        return View("NotFound");
                    }
                    else
                    {
                        ViewBag.ErrorTitle = "The basic data not found in the database ";
                        //ViewBag.ErrorMessage = "Missing data row- " + ex;  // ارسالها للإيميل وعدم عرضها
                        return View("Error");
                    }
                }
                return RedirectToAction(nameof(Index));
          
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "DeletePolicy")]
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return View("NotFound");
            }

            try
            {
                var question = await _question.Entity.GetByIdAsync(id);
                if (question == null)
                {
                    return View("NotFound");
                }

                _question.Entity.Delete(question);
                await _question.SaveAsync();

            }
            catch (Exception ex)
            {
                ViewBag.ErrorTitle = "The basic data not found in the database ";
                // ViewBag.ErrorMessage = "Missing data row- " + ex;  // ارسالها للإيميل وعدم عرضها
                return View("Error");

            }
            return RedirectToAction(nameof(Index));
        }

        private bool questionExists(Guid id)
        {
            return (_question.Entity.GetAll()?.Any(e => e.Id == id)).GetValueOrDefault();
        }







    }
}
