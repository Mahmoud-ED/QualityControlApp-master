using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QualityControlApp.Models.Entities;
using QualityControlApp.Models.Interfaces;
using QualityControlApp.Models;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using static System.Collections.Specialized.BitVector32;
using QualityControlApp.Classes;

namespace QualityControlApp.Controllers
{

    [Authorize(Policy = "ProgOrAdminOrEmployeePolicy")]
  
    [ViewLayout("_LayoutDashboard")]
    public class CompanyController : BaseController
    {

        private readonly ApplicationDbContext _context;
        private readonly IUnitOfWork<Company > _company;
        //private readonly IWebHostEnvironment _host;

        public CompanyController(
    ApplicationDbContext context,
                         IUnitOfWork<Company> company,
                              IWebHostEnvironment host) : base(host)
        {
            _context = context;
            _company = company;
            //_host = host; // نفعله فقط لو احتجناه في هذا الكونترولر
        }




        public async Task<IActionResult> Index()
        {
            var company = await _company.Entity.GetAll().ToListAsync();

            return View(company);

        }

        public async Task<IActionResult> Details(Guid id)
        {



        


            var company = await _company.Entity.GetByIdAsync(id);

           
            return View(company);

        }
        //[Authorize(Policy = "CreatePolicy")]
        public async Task<IActionResult> Create()
        {
           
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name")] Company  company )
        {
            //هذه لا تستعمل ابدا مع المفتاح الأجنبي لكن لو احتجنا لها في حقول اخرى
            //ModelState.Remove("Sections");// طريقة أخرى لمنع التحقق داخل وظيفة معينة وليس في الكلاس سيمنع التحقق بالمطلق لكل الوظائف

            if (ModelState.IsValid)
            {
                try
                {
                    if (companyExistsadd(company.Name)) //في حال اسم موجود
                    {
                        ViewBag.Message = " الاسم موجود مسبقا ";
                        return View();
                    }

                    company.Created = DateTime.Now;
                    company.Name = company.Name;
                    _company .Entity.Insert(company);
                    await _company.SaveAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception)
                {
                    throw;
                }
            }
            return View();
        }


        //[Authorize(Policy = "EditPolicy")]

        public async Task<IActionResult> Edit(Guid? id)
        {

            if (id == null)
            {
                return View("NotFound");
            }

            //var news1= _news.Entity.Include(s => s.Sections).GetByIdAsync(id);

            var company = await _company.Entity
                                         .GetByIdAsync(id);
            if (company == null)
            {
                return View("NotFound");
            }

           
            return View(company);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Name,Id,Created")] Company company)
        {

            // (Cross-Site Request Forgery)

            if (id != company.Id)
            {
                return View("NotFound");
            }

            if (companyExistsEdit(company.Name,company.Id)) //في حال اسم موجود
            {
                ViewBag.Message = " الاسم موجود مسبقا ";
                return View();
            }

            if (ModelState.IsValid)
            {

                try
                {
                    company.Modified = DateTime.Now;
                    _company .Entity.Update(company);
                    await _company.SaveAsync();
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    if (!companyExists(company.Id )) //في حال محذوف
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
            return View(company);
        }



        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return View("NotFound");
            }

            var company = await _company.Entity.GetByIdAsync(id);
            if (company == null)
            {
                return View("NotFound");
            }

            return View(company);

        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid? id)
        {
            if (id == null)
            {
                return View("NotFound");
            }

            try
            {
                var company = await _company.Entity.GetByIdAsync(id);
                if (company == null)
                {
                    return View("NotFound");
                }

                _company.Entity.Delete(company);
                await _company .SaveAsync();

            }
            catch (Exception ex)
            {
                ViewBag.ErrorTitle = "The basic data not found in the database ";
                // ViewBag.ErrorMessage = "Missing data row- " + ex;  // ارسالها للإيميل وعدم عرضها
                return View("Error");

            }
            return RedirectToAction(nameof(Index));
        }

        private bool companyExists(Guid id)
        {
            return (_company.Entity.GetAll()?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        private bool companyExistsadd(String  Name)
        {
            return (_company.Entity.GetAll()?.Any(e => e.Name  == Name)).GetValueOrDefault();
        }

        private bool companyExistsEdit(String Name,Guid id)
        {
            return (_company.Entity.GetAll()?.Any(e => e.Name == Name &&  e.Id != id)).GetValueOrDefault();
        }




    }
}
