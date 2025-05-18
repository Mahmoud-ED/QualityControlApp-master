using QualityControlApp.Models.Entities;
using QualityControlApp.Models.Interfaces;
using QualityControlApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace QualityControlApp.Controllers
{
    public class LocationController :BaseController
    {
        private readonly ApplicationDbContext _context;
        private readonly IUnitOfWork<Location> _location;

        public LocationController(
    ApplicationDbContext context,
                         IUnitOfWork<Location> location,
                              IWebHostEnvironment host) : base(host)
        {
            _context = context;
            _location = location;
        }



        public async Task<IActionResult> Index()
        {
            var location = await _location.Entity.GetAll().ToListAsync();

            return View(location);

        }

        public async Task<IActionResult> Details(Guid id)
        {






            var location = await _location.Entity.GetByIdAsync(id);


            return View(location);

        }
        public async Task<IActionResult> Create()
        {

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name")] Location location)
        {
            //هذه لا تستعمل ابدا مع المفتاح الأجنبي لكن لو احتجنا لها في حقول اخرى
            //ModelState.Remove("Sections");// طريقة أخرى لمنع التحقق داخل وظيفة معينة وليس في الكلاس سيمنع التحقق بالمطلق لكل الوظائف

            if (ModelState.IsValid)
            {
                try
                {
                    if (locationExistsadd(location.Name)) //في حال اسم موجود
                    {
                        ViewBag.Message = " الاسم موجود مسبقا ";
                        return View();
                    }

                    location.Created = DateTime.Now;
                    location.Name = location.Name;
                    _location.Entity.Insert(location);
                    await _location.SaveAsync();
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


            var location = await _location.Entity
                                         .GetByIdAsync(id);
            if (location == null)
            {
                return View("NotFound");
            }


            return View(location);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Name,Id,Created")] Location location)
        {

            // (Cross-Site Request Forgery)

            if (id != location.Id)
            {
                return View("NotFound");
            }

            if (locationExistsEdit(location.Name, location.Id)) //في حال اسم موجود
            {
                ViewBag.Message = " الاسم موجود مسبقا ";
                return View();
            }

            if (ModelState.IsValid)
            {

                try
                {
                    location.Modified = DateTime.Now;
                    _location.Entity.Update(location);
                    await _location.SaveAsync();
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    if (!locationExists(location.Id)) //في حال محذوف
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
            return View(location);
        }



        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return View("NotFound");
            }

            var location = await _location.Entity.GetByIdAsync(id);
            if (location == null)
            {
                return View("NotFound");
            }

            return View(location);

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
                var location = await _location.Entity.GetByIdAsync(id);
                if (location == null)
                {
                    return View("NotFound");
                }

                _location.Entity.Delete(location);
                await _location.SaveAsync();

            }
            catch (Exception ex)
            {
                ViewBag.ErrorTitle = "The basic data not found in the database ";
                // ViewBag.ErrorMessage = "Missing data row- " + ex;  // ارسالها للإيميل وعدم عرضها
                return View("Error");

            }
            return RedirectToAction(nameof(Index));
        }

        private bool locationExists(Guid id)
        {
            return (_location.Entity.GetAll()?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        private bool locationExistsadd(String Name)
        {
            return (_location.Entity.GetAll()?.Any(e => e.Name == Name)).GetValueOrDefault();
        }

        private bool locationExistsEdit(String Name, Guid id)
        {
            return (_location.Entity.GetAll()?.Any(e => e.Name == Name && e.Id != id)).GetValueOrDefault();
        }




    }
}
