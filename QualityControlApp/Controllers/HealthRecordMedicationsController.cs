// QualityControlApp.Controllers.HealthRecordMedicationsController.cs

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QualityControlApp.Models.Entities;
using QualityControlApp.Models.Interfaces;

namespace QualityControlApp.Controllers
{
    [Authorize]
    public class HealthRecordMedicationsController : Controller
    {
        private readonly IUnitOfWork<HealthRecordMedication> _healthRecordMedication;
        private readonly IUnitOfWork<HealthRecord> _healthRecord;
        private readonly IUnitOfWork<Medicine> _medicine;

        public HealthRecordMedicationsController(
            IUnitOfWork<HealthRecordMedication> healthRecordMedication,
            IUnitOfWork<HealthRecord> healthRecord,
            IUnitOfWork<Medicine> medicine)
        {
            _healthRecordMedication = healthRecordMedication;
            _healthRecord = healthRecord;
            _medicine = medicine;
        }

        // GET: HealthRecordMedications/Create?healthRecordId=xxx
        public async Task<IActionResult> Create(Guid? healthRecordId)
        {
            if (healthRecordId == null)
            {
                return NotFound();
            }

            var healthRecord = await _healthRecord.Entity
                .GetWhere(hr => hr.Id == healthRecordId)
                .Include(hr => hr.Employee)
                .Include(hr => hr.ChronicDisease)
                .FirstOrDefaultAsync();

            if (healthRecord == null)
            {
                return NotFound();
            }

            ViewBag.HealthRecord = healthRecord;
            ViewBag.Medicines = new SelectList(
                await _medicine.Entity.GetWhere(m => m.IsAvailable).OrderBy(m => m.Name).ToListAsync(),
                "Id",
                "Name"
            );

            var model = new HealthRecordMedication
            {
                HealthRecordId = healthRecordId.Value,
                StartDate = DateTime.Now,
                IsActive = true
            };

            return View(model);
        }

        // POST: HealthRecordMedications/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("HealthRecordId,MedicineId,Dosage,Frequency,StartDate,EndDate,Instructions,IsActive,Notes")] HealthRecordMedication healthRecordMedication)
        {
            if (ModelState.IsValid)
            {
                healthRecordMedication.Id = Guid.NewGuid();
                healthRecordMedication.Created = DateTime.UtcNow;
                _healthRecordMedication.Entity.Insert(healthRecordMedication);
                await _healthRecordMedication.SaveAsync();

                TempData["SuccessMessage"] = "تم إضافة الدواء للسجل الصحي بنجاح";
                var createdHealthRecord = await _healthRecord.Entity.GetByIdAsync(healthRecordMedication.HealthRecordId);
                if (createdHealthRecord != null)
                {
                    return RedirectToAction("Details", "Employees", new { id = createdHealthRecord.EmployeeId });
                }
                return RedirectToAction("Employees", "employees");
            }

            var healthRecord = await _healthRecord.Entity
                .GetWhere(hr => hr.Id == healthRecordMedication.HealthRecordId)
                .Include(hr => hr.Employee)
                .Include(hr => hr.ChronicDisease)
                .FirstOrDefaultAsync();

            ViewBag.HealthRecord = healthRecord;
            ViewBag.Medicines = new SelectList(
                await _medicine.Entity.GetWhere(m => m.IsAvailable).OrderBy(m => m.Name).ToListAsync(),
                "Id",
                "Name",
                healthRecordMedication.MedicineId
            );

            return View(healthRecordMedication);
        }

        // GET: HealthRecordMedications/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var healthRecordMedication = await _healthRecordMedication.Entity
                .GetWhere(hrm => hrm.Id == id)
                .Include(hrm => hrm.HealthRecord)
                    .ThenInclude(hr => hr.Employee)
                .Include(hrm => hrm.HealthRecord)
                    .ThenInclude(hr => hr.ChronicDisease)
                .Include(hrm => hrm.Medicine)
                .FirstOrDefaultAsync();

            if (healthRecordMedication == null)
            {
                return NotFound();
            }

            ViewBag.Medicines = new SelectList(
                await _medicine.Entity.GetAll().OrderBy(m => m.Name).ToListAsync(),
                "Id",
                "Name",
                healthRecordMedication.MedicineId
            );

            return View(healthRecordMedication);
        }

        // POST: HealthRecordMedications/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,HealthRecordId,MedicineId,Dosage,Frequency,StartDate,EndDate,Instructions,IsActive,Notes,Created")] HealthRecordMedication healthRecordMedication)
        {
            if (id != healthRecordMedication.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    healthRecordMedication.Modified = DateTime.UtcNow;
                    _healthRecordMedication.Entity.Update(healthRecordMedication);
                    await _healthRecordMedication.SaveAsync();

                    TempData["SuccessMessage"] = "تم تحديث معلومات الدواء بنجاح";
                    var updatedHealthRecord = await _healthRecord.Entity.GetByIdAsync(healthRecordMedication.HealthRecordId);
                    if (updatedHealthRecord != null)
                    {
                        return RedirectToAction("Details", "Employees", new { id = updatedHealthRecord.EmployeeId });
                    }
                    return RedirectToAction("Employees", "employees");
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await HealthRecordMedicationExists(healthRecordMedication.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            ViewBag.Medicines = new SelectList(
                await _medicine.Entity.GetAll().OrderBy(m => m.Name).ToListAsync(),
                "Id",
                "Name",
                healthRecordMedication.MedicineId
            );

            return View(healthRecordMedication);
        }

        // GET: HealthRecordMedications/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var healthRecordMedication = await _healthRecordMedication.Entity
                .GetWhere(hrm => hrm.Id == id)
                .Include(hrm => hrm.HealthRecord)
                    .ThenInclude(hr => hr.Employee)
                .Include(hrm => hrm.HealthRecord)
                    .ThenInclude(hr => hr.ChronicDisease)
                .Include(hrm => hrm.Medicine)
                .FirstOrDefaultAsync();

            if (healthRecordMedication == null)
            {
                return NotFound();
            }

            return View(healthRecordMedication);
        }

        // POST: HealthRecordMedications/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var healthRecordMedication = await _healthRecordMedication.Entity.GetByIdAsync(id);
            if (healthRecordMedication == null)
            {
                return NotFound();
            }

            var healthRecordId = healthRecordMedication.HealthRecordId;

            _healthRecordMedication.Entity.Delete(healthRecordMedication);
            await _healthRecordMedication.SaveAsync();

            TempData["SuccessMessage"] = "تم حذف الدواء من السجل الصحي بنجاح";
            var deletedHealthRecord = await _healthRecord.Entity.GetByIdAsync(healthRecordId);
            if (deletedHealthRecord != null)
            {
                return RedirectToAction("Details", "Employees", new { id = deletedHealthRecord.EmployeeId });
            }
            return RedirectToAction("Employees", "employees");
        }

        // GET: HealthRecordMedications/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var healthRecordMedication = await _healthRecordMedication.Entity
                .GetWhere(hrm => hrm.Id == id)
                .Include(hrm => hrm.HealthRecord)
                    .ThenInclude(hr => hr.Employee)
                .Include(hrm => hrm.HealthRecord)
                    .ThenInclude(hr => hr.ChronicDisease)
                .Include(hrm => hrm.Medicine)
                .FirstOrDefaultAsync();

            if (healthRecordMedication == null)
            {
                return NotFound();
            }

            return View(healthRecordMedication);
        }

        private async Task<bool> HealthRecordMedicationExists(Guid id)
        {
            return await _healthRecordMedication.Entity.GetWhere(e => e.Id == id).AnyAsync();
        }
    }
}
