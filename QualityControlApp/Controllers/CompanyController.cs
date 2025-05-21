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
using QualityControlApp.ViewModels;

namespace QualityControlApp.Controllers
{

    

    [ViewLayout("_LayoutDashboard")]
    public class CompanyController : BaseController
    {

        private readonly ApplicationDbContext _context;
        private readonly IUnitOfWork<Company > _company;
        private readonly IUnitOfWork<CompanyType> _companytype;

        public CompanyController(
    ApplicationDbContext context,
                         IUnitOfWork<Company> company,
                         IUnitOfWork<CompanyType> compnayType,
                              IWebHostEnvironment host) : base(host)
        {
            _context = context;
            _companytype = compnayType;
            _company = company;
        }




        public async Task<IActionResult> Index(Guid? companyTypeId)
        {
            var allCompanyTypes = await _companytype.Entity.GetAll().ToListAsync();

            IQueryable<Company> companiesQuery = _company.Entity.Include(c => c.CompanyType);


            if (companyTypeId.HasValue )
            {
                companiesQuery = companiesQuery.Where(c => c.CompanyTypeId == companyTypeId.Value);
            }
            var companies = await companiesQuery.OrderBy(c => c.Name).ToListAsync(); // أو أي ترتيب تفضله

            var viewModel = new CompanyIndexVM
            {
                Companies = companies,
                CompanyTypes = allCompanyTypes
            };

            return View(viewModel);

        }

        public async Task<IActionResult> Details(Guid id)
        {



        


            var company = await _company.Entity.GetByIdAsync(id);

           
            return View(company);

        }
        //[Authorize(Policy = "CreatePolicy")]
        public async Task<IActionResult> Create()
        {
          
            var companyType = await _companytype.Entity.GetAll().ToListAsync();

            var viewModel = new CompanyCreateVM
            {
                Company = null,
                CompanyTypes = companyType
            };



            return View(viewModel);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name","AocNum", "CompanyTypeId")] Company  company )
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
                    company.CompanyTypeId = company.CompanyTypeId;
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
            var companyType = await _companytype.Entity.GetAll().ToListAsync();

            var viewModel = new CompanyCreateVM
            {
                Company = company,
                CompanyTypes = companyType
            };



            return View(viewModel);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Name,Id,Created,AocNum,CompanyTypeId")] Company company)
        {
            if (id != company.Id)
            {
                return View("NotFound");
            }

            if (companyExistsEdit(company.Name, company.Id))
            {
                ViewBag.Message = "الاسم موجود مسبقاً";

                // إعادة تحميل الـ ViewModel
                var vm = new CompanyCreateVM
                {
                    Company = company,
                    CompanyTypes = await _companytype.Entity.GetAll().ToListAsync()
                };
                return View(vm);
            }

            if (ModelState.IsValid)
            {
                try
                {
                    company.Modified = DateTime.Now;
                    company.CompanyTypeId = company.CompanyTypeId;
                    _company.Entity.Update(company);
                    await _company.SaveAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!companyExists(company.Id))
                    {
                        return View("NotFound");
                    }
                    else
                    {
                        ViewBag.ErrorTitle = "The basic data not found in the database";
                        return View("Error");
                    }
                }

                return RedirectToAction(nameof(Index));
            }

            // ModelState غير صحيح، نرجّع ViewModel
            var viewModel = new CompanyCreateVM
            {
                Company = company,
                CompanyTypes = await _companytype.Entity.GetAll().ToListAsync()
            };
            return View(viewModel);
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
