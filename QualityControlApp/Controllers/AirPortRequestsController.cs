using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QualityControlApp.Models.Entities;
using QualityControlApp.Models;
using QualityControlApp.Models.Interfaces;
using System.ComponentModel.Design;
using System.Diagnostics.Contracts;
using QualityControlApp.Models.Repositories;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using System.Reflection.Metadata;
using QualityControlApp.Classes;
using Microsoft.AspNetCore.Http;
using SkiaSharp;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using Microsoft.AspNetCore.Authorization;
using System.Net;
using QualityControlApp.ViewModels;

namespace QualityControlApp.Controllers
{
    public class AirPortRequestsController : BaseController
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _host;
        private readonly IUnitOfWork<Contact> _contact;
        private readonly IUnitOfWork<SiteInfo> _siteInfo;
        private readonly IEmailSender _emailSender;
        private readonly IUnitOfWork<AirPortRequest> _airportrequest;
        private readonly IUnitOfWork<AirPortRequestFiles> _airportrequestfiles;
        private readonly IUnitOfWork<ApplicationUser> _applicationUser;
        private readonly IUnitOfWork<FileType> _filetype;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _configuration;
        private readonly IViewHelper _viewHelper;
        public AirPortRequestsController(
              UserManager<ApplicationUser> userManager,
              IUnitOfWork<ApplicationUser> applicationUser,
              IUnitOfWork<FileType> filetype,
              IWebHostEnvironment host,
              IEmailSender emailSender,
              IUnitOfWork<SiteInfo> siteInfo,
              IUnitOfWork<Contact> contact,
              IUnitOfWork<AirPortRequest> airportrequest,
              IUnitOfWork<AirPortRequestFiles> airportrequestfiles,
              IViewHelper viewHelper,
          IServiceProvider serviceProvider,
          IHttpContextAccessor httpContextAccessor) : base(host)
        {
            _host = host;
            _airportrequest = airportrequest;
            _airportrequestfiles = airportrequestfiles;
            _userManager = userManager;
            _filetype =filetype;
            _siteInfo = siteInfo;
            _contact = contact;
            _emailSender = emailSender;
            _applicationUser = applicationUser;
            _viewHelper = viewHelper;
            _httpContextAccessor = httpContextAccessor;
        }

        // GET: AirPortRequests
        [Authorize(Roles = "Prog,Admin")]

        public async Task<IActionResult> Index(string email, DateTime? requestDateFrom, DateTime? requestDateTo, DateTime? flightDateFrom, DateTime? flightDateTo, string? status)
        {
            var requests = _airportrequest.Entity.GetAll();

            // إذا تم إدخال بريد إلكتروني
            if (!string.IsNullOrEmpty(email))
            {
                requests = requests.Where(r => r.Email.Contains(email)).OrderByDescending(r => r.Created).ThenBy(d => d.FlightDate);
            }

            // إذا تم إدخال تاريخ "من" و "إلى" للطلب
            if (requestDateFrom.HasValue)
            {
                requests = requests.Where(r => r.RequestTime >= requestDateFrom.Value).OrderByDescending(r => r.Created).ThenBy(d => d.FlightDate);
            }
            if (requestDateTo.HasValue)
            {
                requests = requests.Where(r => r.RequestTime <= requestDateTo.Value).OrderByDescending(r => r.Created).ThenBy(d => d.FlightDate);
            }

            // إذا تم إدخال تاريخ "من" و "إلى" للرحلة
            if (flightDateFrom.HasValue)
            {
                requests = requests.Where(r => r.FlightDate >= flightDateFrom.Value).OrderByDescending(r => r.Created).ThenBy(d => d.FlightDate);
            }
            if (flightDateTo.HasValue)
            {
                requests = requests.Where(r => r.FlightDate <= flightDateTo.Value).OrderByDescending(r => r.Created).ThenBy(d => d.FlightDate);
            }

            // إذا تم اختيار حالة الطلب
            if (!string.IsNullOrEmpty(status) & status != "All")
            {
                requests = requests.Where(r => r.RequestStatus == status).OrderByDescending(r => r.Created).ThenBy(d => d.FlightDate);
            }
            else if (status != "All")
            {
                requests = requests.Where(r => r.RequestStatus == "Pending").OrderByDescending(r => r.Created).ThenBy(d => d.FlightDate);
            }
            else
            {

            }
            // تنفيذ الاستعلام وتحديد النتائج
            var result = await requests.ToListAsync();

            return View(result);
        }


        // GET: AirPortRequests/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var airPortRequest = await _airportrequest.Entity
                .Include(r => r.ApplicationUser)
                .Include(r => r.RequestFiles)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (airPortRequest == null)
            {
                return NotFound();
            }

            return View(airPortRequest);
        }

        [Authorize(Roles = "Prog,Admin")]
        public async Task<IActionResult> ChangeStatus(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var airPortRequest = await _airportrequest.Entity
                .Include(r => r.ApplicationUser)
                .Include(r => r.RequestFiles)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (airPortRequest == null)
            {
                return NotFound();
            }

            return View(airPortRequest);
        }

        // GET: AirPortRequests/Create
        public IActionResult Create()
        {
            ViewData["FileTypes"] = _filetype.Entity.GetAll(); // تحميل أنواع الملفات
            return View();
        }

        // POST: AirPortRequests/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AirPortReqeustVM AirPortRequestViewModel)
        {
            //if (ModelState.IsValid)
            //{
            // تعيين وقت الطلب إلى الوقت الحالي
            AirPortRequestViewModel.AirPortRequest.RequestTime = DateTime.Now;
            AirPortRequestViewModel.AirPortRequest.Created = DateTime.Now;
            AirPortRequestViewModel.AirPortRequest.RequestStatus = "Pending";

            // حفظ الطلب في قاعدة البيانات
            _airportrequest.Entity.Insert(AirPortRequestViewModel.AirPortRequest);
            await _airportrequest.SaveAsync();

            // معالجة الملفات المرفقة إذا وجدت
            if (AirPortRequestViewModel.FileTypes != null && AirPortRequestViewModel.FileTypes.Count > 0)
            {
                string uploadsFolder = Path.Combine(_host.WebRootPath, "pictures/requestfiles");

                // التأكد من وجود المجلد
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                foreach (var item in AirPortRequestViewModel.FileTypes)
                {
                    if (item.File != null && item.File.Length > 0)
                    {
                        var fileName = Path.GetFileNameWithoutExtension(item.File.FileName);
                        var extension = Path.GetExtension(item.File.FileName);
                        var uniqueName = $"{fileName}_{Guid.NewGuid()}{extension}";
                        var filePath = Path.Combine("wwwroot/pictures/requestfiles", uniqueName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await item.File.CopyToAsync(stream);
                        }

                        var attachment = new AirPortRequestFiles
                        {
                            FileName = uniqueName,
                            FilePath = filePath,
                            FileTypeId = item.FileTypeId,
                            AirPortRequestId = AirPortRequestViewModel.AirPortRequest.Id
                        };

                        _airportrequestfiles.Entity.Insert(attachment);
                    }
                }


                await _airportrequestfiles.SaveAsync();


                var filePathEmail = _host.WebRootPath + "\\templates" + "\\AirPortRequestOnCreate.html";


                var EmailInfo = await _contact.Entity.GetAll().FirstOrDefaultAsync();
                if (EmailInfo == null)
                {
                    return NotFound();
                }
                var SiteInfo = await _siteInfo.Entity.GetAll().FirstOrDefaultAsync();
                if (SiteInfo == null)
                {
                    return NotFound();
                }

                var contact = await _contact.Entity.GetAll().FirstOrDefaultAsync();
                if (contact == null)
                {
                    return NotFound();
                }

                StreamReader htmlFile = new StreamReader(filePathEmail);
                string content = htmlFile.ReadToEnd();
                htmlFile.Close();
                var editUrl = Url.Action("Edit", "AirPortRequests", new { id = AirPortRequestViewModel.AirPortRequest.Id }, Request.Scheme);

                //تم استعماله مرتين: مرة ضمن الرسالة ومرة اخرى في عنوان الايميل ولكنه نفس العنوان// Subject
                content = content.Replace("{SiteName}", SiteInfo.Name); // يظهر داخل الرسالة
                content = content.Replace("{Phone}", contact.Phone);
                content = content.Replace("{ActionUrl}", editUrl);
                content = content.Replace("{Mail}", contact.Email);

                var message = new Message(new string[] { EmailInfo.Email }, AirPortRequestViewModel.AirPortRequest.Email, content, null);

                try
                {
                    await _emailSender.SendEmailAsync(message);
                    TempData["SuccessMessage"] = "The email has been sent successfully";
                }
                catch
                {
                    ViewBag.errorMessage = "Failed to send email";
                    TempData["ErrorMessage"] = "Failed to send email";
                }

            }

            //return RedirectToAction(nameof(Index));

            return RedirectToAction("Create", "AirPortRequests");


        }

        // GET: AirPortRequests/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var airPortRequest = await _airportrequest.Entity
                .Include(r => r.RequestFiles)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (airPortRequest == null)
            {
                return NotFound();
            }

            return View(airPortRequest);
        }

        // POST: AirPortRequests/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, AirPortRequest airPortRequest, List<IFormFile> files)
        {
            if (id != airPortRequest.Id)
            {
                return NotFound();
            }

            //if (ModelState.IsValid)
            //{
            try
            {
                // الحصول على الطلب الحالي من قاعدة البيانات
                //var existingRequest = await _airportrequest.Entity
                //    .GetByIdAsync(id);

                // الاحتفاظ بوقت الطلب الأصلي

                airPortRequest.Modified = DateTime.Now;

                _airportrequest.Entity.Update(airPortRequest);

                // معالجة الملفات المرفقة الجديدة إذا وجدت
                if (files != null && files.Count > 0)
                {
                    string uploadsFolder = Path.Combine(_host.WebRootPath, "pictures/requestfiles");

                    // التأكد من وجود المجلد
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    foreach (var file in files)
                    {
                        if (file.Length > 0)
                        {
                            // إنشاء اسم فريد للملف
                            string uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
                            string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                            // حفظ الملف
                            using (var fileStream = new FileStream(filePath, FileMode.Create))
                            {
                                await file.CopyToAsync(fileStream);
                            }

                            // إنشاء سجل للملف في قاعدة البيانات
                            AirPortRequestFiles fileRecord = new AirPortRequestFiles
                            {
                                FileName = file.FileName,
                                Created = DateTime.Now,
                                FilePath = "/pictures/requestfiles/" + uniqueFileName,
                                AirPortRequestId = airPortRequest.Id
                            };

                            _airportrequestfiles.Entity.Insert(fileRecord);
                        }
                    }


                }

                await _airportrequestfiles.SaveAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await AirPortRequestExists(id))
                {
                    return NotFound();
                }
            }
            return RedirectToAction(nameof(Edit));
            //}
            //return View(airPortRequest);
        }

        // GET: AirPortRequests/Delete/5
        [Authorize(Roles = "Prog,Admin")]
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var airPortRequest = await _airportrequest.Entity
                .Include(r => r.ApplicationUser)
                .Include(r => r.RequestFiles)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (airPortRequest == null)
            {
                return NotFound();
            }

            return View(airPortRequest);
        }

        // POST: AirPortRequests/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var airPortRequest = await _airportrequest.Entity
                .Include(r => r.RequestFiles)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (airPortRequest != null)
            {
                // حذف الملفات المرفقة من المجلد
                foreach (var file in airPortRequest.RequestFiles)
                {
                    string filePath = Path.Combine(_host.WebRootPath, file.FilePath.TrimStart('/'));
                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);
                    }
                }

                _airportrequest.Entity.Delete(airPortRequest);
                await _airportrequest.SaveAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: AirPortRequests/Approve/5
        [Authorize(Roles = "Prog,Admin")]
        public async Task<IActionResult> Approve(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var airPortRequest = await _airportrequest.Entity
                .GetByIdAsync(id);

            if (airPortRequest == null)
            {
                return NotFound();
            }

            return View(airPortRequest);
        }

        [Authorize(Roles = "Prog,Admin")]
        // POST: AirPortRequests/Approve/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Approve(Guid id, string newStatus)
        {
            var airPortRequest = await _airportrequest.Entity.GetByIdAsync(id);

            if (airPortRequest == null)
            {
                return NotFound();
            }

            // التحقق من إذا كانت قيمة newStatus موجودة وغير فارغة
            if (!string.IsNullOrEmpty(newStatus))
            {
                // تحديث حالة الطلب بناءً على الحالة المختارة
                airPortRequest.RequestStatus = newStatus;
            }

            // تعيين المستخدم الحالي كموافق
            airPortRequest.ApproverUserId = _userManager.GetUserId(User);

            await _airportrequest.SaveAsync();

            return RedirectToAction(nameof(Index));
        }


        // GET: AirPortRequests/Reject/5
        public async Task<IActionResult> Reject(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var airPortRequest = await _airportrequest.Entity
                .GetByIdAsync(id);

            if (airPortRequest == null)
            {
                return NotFound();
            }

            return View(airPortRequest);
        }

        // POST: AirPortRequests/Reject/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reject(Guid id)
        {
            var airPortRequest = await _airportrequest.Entity.GetByIdAsync(id);

            if (airPortRequest == null)
            {
                return NotFound();
            }

            // تحديث حالة الطلب
            airPortRequest.RequestStatus = "مرفوض";

            await _airportrequest.SaveAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: AirPortRequests/DeleteFile/5
        public async Task<IActionResult> DeleteFile(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var file = await _airportrequestfiles.Entity
                .GetByIdAsync(id);

            if (file == null)
            {
                return NotFound();
            }

            // حذف الملف من المجلد
            string filePath = Path.Combine(_host.WebRootPath, file.FilePath.TrimStart('/'));
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }

            // حذف السجل من قاعدة البيانات
            _airportrequestfiles.Entity.Delete(file);
            await _airportrequestfiles.SaveAsync();

            return RedirectToAction(nameof(Edit), new { id = file.AirPortRequestId });
        }

        private async Task<bool> AirPortRequestExists(Guid id)
        {
            var entity = await _airportrequest.Entity.GetByIdAsync(id);
            return entity != null;
        }

    }
}