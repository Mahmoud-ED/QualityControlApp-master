using Microsoft.AspNetCore.Mvc;
using QualityControlApp.Classes;
using QualityControlApp.Models.Entities;
using QualityControlApp.Models.Interfaces;
using QualityControlApp.Models;
using Microsoft.EntityFrameworkCore;
using QualityControlApp.ViewModels;

namespace QualityControlApp.Controllers
{
        [ViewLayout("_LayoutDashboard")]

    public class CompanyTypeController : BaseController
    {


    

            private readonly ApplicationDbContext _context;
            private readonly IUnitOfWork<CompanyType> _companytype;
            private readonly IUnitOfWork<QuestionCategoryType> _questioncategoryytpe;
            //private readonly IWebHostEnvironment _host;

            public CompanyTypeController(
        ApplicationDbContext context,
                             IUnitOfWork<Company> company,
                             IUnitOfWork<CompanyType> compnayType,
                             IUnitOfWork<QuestionCategoryType> questioncategoryytpe,
                                  IWebHostEnvironment host) : base(host)
            {
                _context = context;
                _companytype = compnayType;
            _questioncategoryytpe = questioncategoryytpe;
        }


          

        public IActionResult Index()
        {
            var companytype = _companytype.Entity.GetAll();
            return View(companytype);
        }
        public async Task<IActionResult> Create()
        {
            // Fetch all QuestionCategoryType entities first
            var allCategories =  _questioncategoryytpe.Entity.GetAll().ToList(); 

            var viewModel = new CompanyTypeCreateViewModel
            {
                AvailableCategories = allCategories
                                        .Select(qct => new QuestionCategoryTypeViewModel
                                        {
                                            Id = qct.Id, // Assuming Id is int
                                            CategoryName = qct.CategoryName,
                                            IsSelected = false
                                        }).ToList() // Use ToList() if allCategories is already IEnumerable<QuestionCategoryType>
                                                    // If allCategories is IQueryable, you can do the Select before ToListAsync
            };

          
            return View(viewModel);
        }
        public async Task<IActionResult> Details(Guid id)
        {

            var companytype = await _companytype.Entity.GetByIdAsync(id);


            return View(companytype);

        }



        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create([Bind("Name")] CompanyType companytype)
        //{
        //    //هذه لا تستعمل ابدا مع المفتاح الأجنبي لكن لو احتجنا لها في حقول اخرى
        //    //ModelState.Remove("Sections");// طريقة أخرى لمنع التحقق داخل وظيفة معينة وليس في الكلاس سيمنع التحقق بالمطلق لكل الوظائف

        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            if (companyExistsadd(companytype.Name)) //في حال اسم موجود
        //            {
        //                ViewBag.Message = " الاسم موجود مسبقا ";
        //                return View();
        //            }

        //            companytype.Created = DateTime.Now;
        //            companytype.Name = companytype.Name;

        //            _companytype.Entity.Insert(companytype);
        //            await _companytype.SaveAsync();
        //            return RedirectToAction(nameof(Index));
        //        }
        //        catch (Exception)
        //        {
        //            throw;
        //        }
        //    }
        //    return View();
        //}




        // In CompanyTypeController.cs

        // Make sure you have injected your DbContext (e.g., _context)
        // and your repository for QuestionCategoryType (e.g., _questioncategorytypeRepository)

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CompanyTypeCreateViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var companyType = new CompanyType
                {
                    Name = viewModel.Name,
                    AvailableCategories = new List<CompanyTypeCategoryAvailable>()
                };

                if (viewModel.AvailableCategories != null && viewModel.AvailableCategories.Any())
                {
                    foreach (var categoryVM in viewModel.AvailableCategories)
                    {
                        if (categoryVM.IsSelected) 
                        {
                            var companyTypeCategoryLink = new CompanyTypeCategoryAvailable
                            {

                                QuestionCategoryTypeId = categoryVM.Id 
                            };
                            companyType.AvailableCategories.Add(companyTypeCategoryLink);
                        }
                    }
                }

                _companytype.Entity.Insert(companyType); 

                try
                {
                    await _context.SaveChangesAsync(); 

                    TempData["success"] = "Company Type created successfully!"; 
                    return RedirectToAction(nameof(Index)); 
                }
                catch (DbUpdateException ex)
                {
                    ModelState.AddModelError("", $"An error occurred while saving: {ex.Message}");
                }
            }

            if (viewModel.AvailableCategories == null || !viewModel.AvailableCategories.Any())
            {
                var allCategoriesFromDb =  _questioncategoryytpe.Entity.GetAll().ToList(); 

                var postedSelections = viewModel.AvailableCategories?
                                        .Where(vm => vm.IsSelected)
                                        .ToDictionary(vm => vm.Id, vm => true)
                                        ?? new Dictionary<Guid, bool>(); 

                viewModel.AvailableCategories = allCategoriesFromDb
                                                .Select(qct => new QuestionCategoryTypeViewModel
                                                {
                                                    Id = qct.Id,
                                                    CategoryName = qct.CategoryName,
                                                    IsSelected = postedSelections.ContainsKey(qct.Id)
                                                }).ToList();
            }
            else
            {
            }

            return View(viewModel); 
        }
        //[Authorize(Policy = "EditPolicy")]

        public async Task<IActionResult> Edit(Guid? id)
        {

            if (id == null)
            {
                return View("NotFound");
            }


            var companytype = await _companytype.Entity
                                         .GetByIdAsync(id);
            if (companytype == null)
            {
                return View("NotFound");
            }

            return View(companytype);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Name,Id,Created")] CompanyType companytype)
        {

            // (Cross-Site Request Forgery)

            if (id != companytype.Id)
            {
                return View("NotFound");
            }

            if (companyExistsEdit(companytype.Name, companytype.Id)) //في حال اسم موجود
            {
                ViewBag.Message = " الاسم موجود مسبقا ";
                return View();
            }

            if (ModelState.IsValid)
            {

                try
                {
                    companytype.Modified = DateTime.Now;
                    _companytype.Entity.Update(companytype);
                    await _companytype.SaveAsync();
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    if (!companyExists(companytype.Id)) //في حال محذوف
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
            return View(companytype);
        }



        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return View("NotFound");
            }

            var company = await _companytype.Entity.GetByIdAsync(id);
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
                var companytype = await _companytype.Entity.GetByIdAsync(id);
                if (companytype == null)
                {
                    return View("NotFound");
                }

                _companytype.Entity.Delete(companytype);
                await _companytype.SaveAsync();

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
            return (_companytype.Entity.GetAll()?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        private bool companyExistsadd(String Name)
        {
            return (_companytype.Entity.GetAll()?.Any(e => e.Name == Name)).GetValueOrDefault();
        }

        private bool companyExistsEdit(String Name, Guid id)
        {
            return (_companytype.Entity.GetAll()?.Any(e => e.Name == Name && e.Id != id)).GetValueOrDefault();
        }




    }
}
