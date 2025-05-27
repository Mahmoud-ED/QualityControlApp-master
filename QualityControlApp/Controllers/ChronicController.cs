// QualityControlApp.Controllers.ChronicDiseaseController.cs

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QualityControlApp.Classes;
using QualityControlApp.Models;
using QualityControlApp.Models.Entities;
using QualityControlApp.Models.Interfaces;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using QualityControlApp.ViewModels; // Add this using statement

namespace QualityControlApp.Controllers
{
    [ViewLayout("_LayoutDashboard")]
    public class ChronicDiseaseController : BaseController
    {
        private readonly ApplicationDbContext _context;
        private readonly IUnitOfWork<ChronicDisease> _chronicDiseaseUnitOfWork;
        // private readonly IWebHostEnvironment _host;

        public ChronicDiseaseController(
            ApplicationDbContext context,
            IUnitOfWork<ChronicDisease> chronicDiseaseUnitOfWork,
            IWebHostEnvironment host) : base(host)
        {
            _context = context;
            _chronicDiseaseUnitOfWork = chronicDiseaseUnitOfWork;
            // _host = host;
        }

        // GET: ChronicDisease
        public IActionResult Index() // Changed to non-async for simplicity if GetAll is sync
        {
            var viewModel = new ChronicDiseaseIndexViewModel
            {
                ChronicDiseases = _chronicDiseaseUnitOfWork.Entity.GetAll().OrderBy(cd => cd.Name), // Or OrderByDescending(cd => cd.CreatedAt)
                NewChronicDisease = new ChronicDisease() // Initialize for the form
            };
            return View(viewModel);
        }

        // POST: ChronicDisease/Create (This action will be called by the form on the Index page)
        [HttpPost]
        [ValidateAntiForgeryToken]
        // The parameter name 'newChronicDisease' should match the property name in the ViewModel used in the form's asp-for
        public async Task<IActionResult> Create(ChronicDiseaseIndexViewModel model)
        {
            // We are interested in model.NewChronicDisease for validation and saving
            if (ModelState.IsValid) // This will validate model.NewChronicDisease based on its DataAnnotations
            {
                _chronicDiseaseUnitOfWork.Entity.Insert(model.NewChronicDisease);
                await _chronicDiseaseUnitOfWork.SaveAsync();
                TempData["success"] = "تم إضافة المرض المزمن بنجاح.";
                return RedirectToAction(nameof(Index)); // Redirect to refresh the list and clear the form
            }

            // If ModelState is invalid, we need to re-populate the list and return to the Index view
            TempData["error"] = "فشل إضافة المرض المزمن. يرجى مراجعة البيانات المدخلة.";
            // Re-populate the list of chronic diseases for the view
            model.ChronicDiseases = _chronicDiseaseUnitOfWork.Entity.GetAll().OrderBy(cd => cd.Name);
            // The model.NewChronicDisease already contains the user's input and validation errors
            return View("Index", model);
        }

        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound("معرف المرض المزمن غير متوفر.");
            }

            var chronicDisease = await _chronicDiseaseUnitOfWork.Entity.GetByIdAsync(id);
            if (chronicDisease == null)
            {
                TempData["error"] = "المرض المزمن المطلوب تعديله غير موجود.";
                return RedirectToAction(nameof(Index)); // Or return NotFound();
            }
            return View(chronicDisease); // This will render Views/ChronicDisease/Edit.cshtml
        }

        // POST: ChronicDisease/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, ChronicDisease chronicDisease)
        {
            if (id != chronicDisease.Id)
            {
                TempData["error"] = "عدم تطابق في معرف المرض المزمن.";
                return RedirectToAction(nameof(Index)); // Or return BadRequest();
            }

            // Ensure CreatedAt is not unintentionally changed by model binding if it's not on the form
            // One way is to fetch the original entity and only update specific fields.
            // Another is to mark CreatedAt as not modified if using Entity Framework directly.
            // For simplicity with UoW, if BaseEntity handles CreatedAt on creation only, this should be fine.
            // If your BaseEntity or update logic might overwrite CreatedAt, fetch original:
            /*
            var existingDisease = await _chronicDiseaseUnitOfWork.Entity.GetByIdAsync(id);
            if (existingDisease == null) return NotFound();
            existingDisease.Name = chronicDisease.Name;
            existingDisease.Description = chronicDisease.Description;
            // existingDisease.UpdatedAt will be set by BaseEntity logic
            chronicDisease = existingDisease; // now use this updated existingDisease for the UoW
            */


            if (ModelState.IsValid)
            {
                try
                {
                    // If BaseEntity handles UpdatedAt automatically, just call Update.
                    _chronicDiseaseUnitOfWork.Entity.Update(chronicDisease);
                    await _chronicDiseaseUnitOfWork.SaveAsync();
                    TempData["success"] = "تم تعديل المرض المزمن بنجاح.";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await ChronicDiseaseExistsAsync(chronicDisease.Id))
                    {
                        TempData["error"] = "المرض المزمن لم يعد موجوداً (ربما تم حذفه بواسطة مستخدم آخر).";
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "تم تعديل هذا السجل بواسطة مستخدم آخر. تم إلغاء تغييراتك. الرجاء إعادة تحميل البيانات والمحاولة مرة أخرى.");
                        // Optional: Reload the entity from the database to show the conflicting values
                        // var databaseValues = await _chronicDiseaseUnitOfWork.Entity.GetByIdAsync(id);
                        // var databaseEntry = _context.Entry(databaseValues); // If UoW gives access to context or similar
                        // // You can then show databaseValues.Name etc. to the user or log them.
                        // ModelState.AddModelError("Name", $"القيمة الحالية في قاعدة البيانات: {databaseValues.Name}");
                    }
                }
                catch (Exception ex) // Catch other potential errors during update
                {
                    // Log the error (ex)
                    TempData["error"] = "حدث خطأ غير متوقع أثناء محاولة تعديل المرض المزمن.";
                    ModelState.AddModelError(string.Empty, "حدث خطأ غير متوقع. الرجاء المحاولة مرة أخرى.");
                }
            }
            // If ModelState is invalid or an error occurred, return to the Edit view
            // The chronicDisease object still contains the user's attempted changes and validation errors
            TempData["error"] = TempData["error"] as string ?? "فشل تعديل المرض المزمن. يرجى مراجعة البيانات المدخلة.";
            return View(chronicDisease);
        }

        // POST: ChronicDisease/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid? id) // Name matches asp-action in form
        {
            if (id == null)
            {
                TempData["error"] = "معرف المرض المزمن غير موجود.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                var chronicDisease = await _chronicDiseaseUnitOfWork.Entity.GetByIdAsync(id);
                if (chronicDisease == null)
                {
                    TempData["error"] = "المرض المزمن المراد حذفه غير موجود.";
                    return RedirectToAction(nameof(Index));
                }

                // Check for related HealthRecords
                // This assumes your ApplicationDbContext (_context) has a DbSet<HealthRecord> HealthRecords
                // and HealthRecord entity has a ChronicDiseaseId foreign key.
                bool isLinkedToHealthRecord = await _context.HealthRecord
                                                    .AnyAsync(hr => hr.ChronicDiseaseId == id);

                if (isLinkedToHealthRecord)
                {
                    TempData["error"] = "لا يمكن حذف هذا المرض المزمن لأنه مرتبط بسجلات صحية حالية. يرجى إزالة الارتباطات أولاً.";
                    return RedirectToAction(nameof(Index));
                }

                _chronicDiseaseUnitOfWork.Entity.Delete(chronicDisease);
                await _chronicDiseaseUnitOfWork.SaveAsync();
                TempData["success"] = "تم حذف المرض المزمن بنجاح.";
            }
            catch (DbUpdateException ex) // Handles potential FK issues not caught by the manual check or other DB errors during save
            {
                // Log the error (ex)
                TempData["error"] = "لا يمكن حذف هذا المرض المزمن بسبب ارتباطه ببيانات أخرى في النظام أو حدوث خطأ في قاعدة البيانات.";
                // Consider more specific error messages based on ex.InnerException if possible
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                // Log the generic error (ex)
                TempData["error"] = "حدث خطأ غير متوقع أثناء محاولة الحذف.";
                return RedirectToAction(nameof(Index));
            }
            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> ChronicDiseaseExistsAsync(Guid id)
        {
            return await _chronicDiseaseUnitOfWork.Entity.GetByIdAsync(id) != null;
        }
    }
}