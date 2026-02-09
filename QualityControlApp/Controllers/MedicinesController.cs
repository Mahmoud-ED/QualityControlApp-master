// QualityControlApp.Controllers.MedicinesController.cs

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QualityControlApp.Models.Entities;
using QualityControlApp.Models.Interfaces;
using QualityControlApp.Classes;

namespace QualityControlApp.Controllers
{
    [Authorize]
    public class MedicinesController : Controller
    {
        private readonly IUnitOfWork<Medicine> _medicine;

        public MedicinesController(IUnitOfWork<Medicine> medicine)
        {
            _medicine = medicine;
        }

        // GET: Medicines
        public async Task<IActionResult> Index(string searchString, string sortOrder, int? pageNumber)
        {
            ViewData["CurrentFilter"] = searchString;
            ViewData["NameSortParm"] = string.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["TypeSortParm"] = sortOrder == "Type" ? "type_desc" : "Type";
            ViewData["AvailableSortParm"] = sortOrder == "Available" ? "available_desc" : "Available";

            var medicines = _medicine.Entity.GetAll();

            // البحث
            if (!string.IsNullOrEmpty(searchString))
            {
                medicines = medicines.Where(m =>
                    m.Name.Contains(searchString) ||
                    (m.GenericName != null && m.GenericName.Contains(searchString)) ||
                    (m.Manufacturer != null && m.Manufacturer.Contains(searchString)));
            }

            // الترتيب
            medicines = sortOrder switch
            {
                "name_desc" => medicines.OrderByDescending(m => m.Name),
                "Type" => medicines.OrderBy(m => m.Type),
                "type_desc" => medicines.OrderByDescending(m => m.Type),
                "Available" => medicines.OrderBy(m => m.IsAvailable),
                "available_desc" => medicines.OrderByDescending(m => m.IsAvailable),
                _ => medicines.OrderBy(m => m.Name),
            };

            int pageSize = 10;
            return View(await PaginatedList<Medicine>.CreateAsync(medicines.AsNoTracking(), pageNumber ?? 1, pageSize));
        }

        // GET: Medicines/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var medicine = await _medicine.Entity
                .GetWhere(m => m.Id == id)
                .Include(m => m.HealthRecordMedications)
                    .ThenInclude(hrm => hrm.HealthRecord)
                        .ThenInclude(hr => hr.Employee)
                .FirstOrDefaultAsync();

            if (medicine == null)
            {
                return NotFound();
            }

            return View(medicine);
        }

        // GET: Medicines/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Medicines/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,GenericName,Manufacturer,Type,Description,IsAvailable,Notes")] Medicine medicine)
        {
            if (ModelState.IsValid)
            {
                // التحقق من عدم وجود دواء بنفس الاسم
                var existingMedicine = await _medicine.Entity
                    .GetWhere(m => m.Name == medicine.Name)
                    .FirstOrDefaultAsync();

                if (existingMedicine != null)
                {
                    ModelState.AddModelError("Name", "يوجد دواء بنفس الاسم بالفعل");
                    return View(medicine);
                }

                medicine.Id = Guid.NewGuid();
                medicine.Created = DateTime.UtcNow;
                _medicine.Entity.Insert(medicine);
                await _medicine.SaveAsync();

                TempData["SuccessMessage"] = "تم إضافة الدواء بنجاح";
                return RedirectToAction(nameof(Index));
            }
            return View(medicine);
        }

        // GET: Medicines/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var medicine = await _medicine.Entity.GetByIdAsync(id.Value);
            if (medicine == null)
            {
                return NotFound();
            }
            return View(medicine);
        }

        // POST: Medicines/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,Name,GenericName,Manufacturer,Type,Description,IsAvailable,Notes,Created")] Medicine medicine)
        {
            if (id != medicine.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // التحقق من عدم وجود دواء آخر بنفس الاسم
                    var existingMedicine = await _medicine.Entity
                        .GetWhere(m => m.Name == medicine.Name && m.Id != medicine.Id)
                        .FirstOrDefaultAsync();

                    if (existingMedicine != null)
                    {
                        ModelState.AddModelError("Name", "يوجد دواء آخر بنفس الاسم");
                        return View(medicine);
                    }

                    medicine.Modified = DateTime.UtcNow;
                    _medicine.Entity.Update(medicine);
                    await _medicine.SaveAsync();

                    TempData["SuccessMessage"] = "تم تحديث الدواء بنجاح";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await MedicineExists(medicine.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            return View(medicine);
        }

        // GET: Medicines/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var medicine = await _medicine.Entity
                .GetWhere(m => m.Id == id)
                .Include(m => m.HealthRecordMedications)
                .FirstOrDefaultAsync();

            if (medicine == null)
            {
                return NotFound();
            }

            return View(medicine);
        }

        // POST: Medicines/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var medicine = await _medicine.Entity
                .GetWhere(m => m.Id == id)
                .Include(m => m.HealthRecordMedications)
                .FirstOrDefaultAsync();

            if (medicine == null)
            {
                return NotFound();
            }

            // التحقق من عدم وجود سجلات صحية مرتبطة
            if (medicine.HealthRecordMedications.Any())
            {
                TempData["ErrorMessage"] = "لا يمكن حذف هذا الدواء لأنه مرتبط بسجلات صحية";
                return RedirectToAction(nameof(Index));
            }

            _medicine.Entity.Delete(medicine);
            await _medicine.SaveAsync();

            TempData["SuccessMessage"] = "تم حذف الدواء بنجاح";
            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> MedicineExists(Guid id)
        {
            return await _medicine.Entity.GetWhere(e => e.Id == id).AnyAsync();
        }
    }
}
