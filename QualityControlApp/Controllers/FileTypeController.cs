using Microsoft.AspNetCore.Mvc;
using QualityControlApp.Models.Entities;
using QualityControlApp.Models.Interfaces;
using QualityControlApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace QualityControlApp.Controllers
{
    public class FileTypeController : BaseController
    {


        private readonly ApplicationDbContext _context;
        private readonly IUnitOfWork<FileType> _filetype;
        //private readonly IWebHostEnvironment _host;

        public FileTypeController(
                              ApplicationDbContext context,
                              IUnitOfWork<FileType> filetype,
                              IWebHostEnvironment host) : base(host)
        
        {
            _context = context;
            _filetype = filetype;
            //_host = host; // نفعله فقط لو احتجناه في هذا الكونترولر
        }



        public IActionResult Index()
        {
            var fileTypes = _filetype.Entity.GetAll();
            return View(fileTypes);
        }

      
        public IActionResult Create()
        {
            return View();
        }

        // POST: FileType/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(FileType fileType)
        {
            if (ModelState.IsValid)
            {
                _filetype.Entity .Insert (fileType);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(fileType);
        }


        // GET: FileType/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
                return NotFound();

            var fileType = await _filetype.Entity .GetByIdAsync(id);
            if (fileType == null)
                return NotFound();

            return View(fileType);
        }

        // POST: FileType/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid  id, FileType fileType)
        {
            if (id != fileType.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _filetype.Entity.Update(fileType);
                    await _filetype.SaveAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FileTypeExists(fileType.Id))
                        return NotFound();
                    else
                        throw;
                }
            }

            return View(fileType);
        }

        // Helper method
        private bool FileTypeExists(Guid id)
        {
            return (_filetype.Entity.GetAll()?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
      
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return View("NotFound");
            }

            try
            {
                var filetype = await _filetype.Entity.GetByIdAsync(id);
                if (filetype == null)
                {
                    return View("NotFound");
                }

                _filetype.Entity.Delete(filetype);
                await _filetype.SaveAsync();

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
            return (_filetype.Entity.GetAll()?.Any(e => e.Id == id)).GetValueOrDefault();
        }







    }
}
