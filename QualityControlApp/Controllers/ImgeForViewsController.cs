using Microsoft.AspNetCore.Hosting; // مهم للوصول إلى wwwroot
using Microsoft.AspNetCore.Http; // مهم لـ IFormFile
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QualityControlApp.Models;
using QualityControlApp.Models.Entities;
using System;
using System.IO; // مهم لعمليات الملفات
using System.Linq;
using System.Threading.Tasks;

namespace QualityControlApp.Controllers
{
    public class ImgeForViewsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment; // للوصول إلى مسار wwwroot

        // مجلد لحفظ الصور داخل wwwroot
        private const string ImagesUploadPath = "pictures/ImageForViews";

        public ImgeForViewsController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: ImgeForViews
        public async Task<IActionResult> Index()
        {
            return View(await _context.ImgeForViews.ToListAsync());
        }

        // GET: ImgeForViews/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var imgeForViews = await _context.ImgeForViews
                .FirstOrDefaultAsync(m => m.Id == id);
            if (imgeForViews == null)
            {
                return NotFound();
            }

            return View(imgeForViews);
        }

        // GET: ImgeForViews/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: ImgeForViews/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ViewName,CoverImage")] ImgeForViews imgeForViews) // لا نربط CoverImageUrl مباشرة
        {
            if (ModelState.IsValid)
            {
                if (imgeForViews.CoverImage != null && imgeForViews.CoverImage.Length > 0)
                {
                    // حفظ الصورة والحصول على مسارها
                    string? imageUrl = await SaveImageAsync(imgeForViews.CoverImage);
                    if (imageUrl != null)
                    {
                        imgeForViews.CoverImageUrl = imageUrl;
                    }
                    else
                    {
                        // يمكنك إضافة خطأ للموديل إذا فشل تحميل الصورة
                        ModelState.AddModelError("CoverImage", "حدث خطأ أثناء تحميل الصورة.");
                        return View(imgeForViews);
                    }
                }

                _context.Add(imgeForViews);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(imgeForViews);
        }

        // GET: ImgeForViews/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var imgeForViews = await _context.ImgeForViews.FindAsync(id);
            if (imgeForViews == null)
            {
                return NotFound();
            }
            return View(imgeForViews);
        }

        // POST: ImgeForViews/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,ViewName,CoverImage,CoverImageUrl")] ImgeForViews imgeForViews)
        {
            if (id != imgeForViews.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    string? oldImageUrl = null;
                    // إذا تم تحميل صورة جديدة
                    if (imgeForViews.CoverImage != null && imgeForViews.CoverImage.Length > 0)
                    {
                        // قبل تحميل الصورة الجديدة، نحتاج إلى مسار الصورة القديمة لحذفها إذا كانت موجودة
                        // لا نستخدم AsNoTracking هنا لأننا سنقوم بتحديث هذا الكائن
                        var existingEntity = await _context.ImgeForViews.FindAsync(id);
                        if (existingEntity != null)
                        {
                            oldImageUrl = existingEntity.CoverImageUrl; // احفظ المسار القديم
                        }

                        string? newImageUrl = await SaveImageAsync(imgeForViews.CoverImage);
                        if (newImageUrl != null)
                        {
                            imgeForViews.CoverImageUrl = newImageUrl; // قم بتحديث المسار بالمسار الجديد
                        }
                        else
                        {
                            ModelState.AddModelError("CoverImage", "حدث خطأ أثناء تحميل الصورة الجديدة.");
                            // لا تعدل imgeForViews.CoverImageUrl إذا فشل التحميل، استخدم القيمة القديمة
                            // التي جاءت من الـ hidden field في الـ form (أو أعد تحميلها من قاعدة البيانات)
                            var entityFromDb = await _context.ImgeForViews.AsNoTracking().FirstOrDefaultAsync(e => e.Id == id);
                            imgeForViews.CoverImageUrl = entityFromDb?.CoverImageUrl;
                            return View(imgeForViews);
                        }
                    }
                    // إذا لم يتم تحميل صورة جديدة، سيبقى `imgeForViews.CoverImageUrl` كما هو من الـ form (hidden field)
                    // أو إذا كنت لا تثق بالـ hidden field، يمكنك إعادة تحميله:
                    // else
                    // {
                    //    var entityFromDb = await _context.ImgeForViews.AsNoTracking().FirstOrDefaultAsync(e => e.Id == id);
                    //    imgeForViews.CoverImageUrl = entityFromDb?.CoverImageUrl;
                    // }


                    _context.Update(imgeForViews);
                    await _context.SaveChangesAsync();

                    // إذا تم تحميل صورة جديدة بنجاح، احذف الصورة القديمة
                    if (oldImageUrl != null && oldImageUrl != imgeForViews.CoverImageUrl) // تأكد أن الصورة تغيرت فعلاً
                    {
                        DeleteImage(oldImageUrl);
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ImgeForViewsExists(imgeForViews.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(imgeForViews);
        }

        // GET: ImgeForViews/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var imgeForViews = await _context.ImgeForViews
                .FirstOrDefaultAsync(m => m.Id == id);
            if (imgeForViews == null)
            {
                return NotFound();
            }

            return View(imgeForViews);
        }

        // POST: ImgeForViews/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var imgeForViews = await _context.ImgeForViews.FindAsync(id);
            if (imgeForViews != null)
            {
                // حذف الصورة من الخادم قبل حذف السجل من قاعدة البيانات
                if (!string.IsNullOrEmpty(imgeForViews.CoverImageUrl))
                {
                    DeleteImage(imgeForViews.CoverImageUrl);
                }
                _context.ImgeForViews.Remove(imgeForViews);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool ImgeForViewsExists(Guid id)
        {
            return _context.ImgeForViews.Any(e => e.Id == id);
        }

        // دالة مساعدة لحفظ الصورة
        private async Task<string?> SaveImageAsync(IFormFile imageFile)
        {
            if (imageFile == null || imageFile.Length == 0)
            {
                return null;
            }

            // المسار الكامل لمجلد الرفع داخل wwwroot
            var uploadFolderPath = Path.Combine(_webHostEnvironment.WebRootPath, ImagesUploadPath);

            // تأكد من وجود المجلد، وإذا لم يكن موجودًا، قم بإنشائه
            if (!Directory.Exists(uploadFolderPath))
            {
                Directory.CreateDirectory(uploadFolderPath);
            }

            // إنشاء اسم ملف فريد لتجنب الكتابة فوق الملفات الموجودة
            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
            var filePath = Path.Combine(uploadFolderPath, fileName);

            try
            {
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await imageFile.CopyToAsync(stream);
                }
                // إرجاع المسار النسبي للصورة ليتم حفظه في قاعدة البيانات وعرضه في الـ HTML
                // يجب أن يبدأ بـ '/'
                return $"/{ImagesUploadPath.Replace("\\", "/")}/{fileName}";
            }
            catch (Exception ex)
            {
                // يمكنك تسجيل الخطأ هنا
                Console.WriteLine($"Error saving image: {ex.Message}");
                return null;
            }
        }

        // دالة مساعدة لحذف الصورة
        private void DeleteImage(string relativeImagePath)
        {
            if (string.IsNullOrEmpty(relativeImagePath))
            {
                return;
            }

            // تحويل المسار النسبي إلى مسار كامل
            // إزالة '/' البادئة إذا كانت موجودة، لأن Path.Combine مع WebRootPath لا يحتاجها
            var imagePath = Path.Combine(_webHostEnvironment.WebRootPath, relativeImagePath.TrimStart('/'));

            try
            {
                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }
            }
            catch (Exception ex)
            {
                // يمكنك تسجيل الخطأ هنا
                Console.WriteLine($"Error deleting image: {ex.Message}");
            }
        }
    }
}