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
        [ViewLayout("_LayoutDashboard")]

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
        .ThenInclude(f => f.FileType)
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
                            Inspect = "",
                            Nots = "",
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

                var message = new Message(new string[] { AirPortRequestViewModel.AirPortRequest.Email },"Air Request", content, null);
                var message2 = new Message(new string[] { EmailInfo.Email }, "Air Request" + AirPortRequestViewModel.AirPortRequest.Email, content, null);

                try
                {
                    await _emailSender.SendEmailAsync(message);
                    await _emailSender.SendEmailAsync(message2);
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
            ViewData["FileTypes"] = _filetype.Entity.GetAll();


            var airPortRequest = await _airportrequest.Entity
    .Include(r => r.ApplicationUser)
    .Include(r => r.RequestFiles)
        .ThenInclude(f => f.FileType)
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
        public async Task<IActionResult> Edit(Guid id, AirPortRequest airPortRequest, AirPortReqeustVM AirPortRequestViewModel)
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
                                Inspect = "",
                                Nots = "",
                                FilePath = filePath,
                                FileTypeId = item.FileTypeId,
                                AirPortRequestId = id
                            };

                            _airportrequestfiles.Entity.Insert(attachment);
                        }
                    }


                    await _airportrequestfiles.SaveAsync();
                }
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

       
        private async Task<bool> AirPortRequestExists(Guid id)
        {
            var entity = await _airportrequest.Entity.GetByIdAsync(id);
            return entity != null;
        }

















        [HttpPost]
        // إذا كنت تستخدم AntiForgeryToken، أضف: [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateAttachmentDetails(Guid requestFileId, string inspect, string nots)
        {
            // يمكنك إضافة التحقق من صلاحيات المستخدم هنا

            // البحث عن الملف في قاعدة البيانات
            // تأكد من استبدال RequestFile بالاسم الصحيح للموديل و Id بالاسم الصحيح للـ Primary Key
            var fileToUpdate = await _airportrequestfiles .Entity.GetByIdAsync( requestFileId);

            if (fileToUpdate == null)
            {
                return NotFound(new { message = "Attachment not found." });
            }

            // تحديث الخصائص
            fileToUpdate.Inspect = inspect; // تأكد من أن نوع البيانات متوافق أو قم بالتحويل اللازم

            // تحديث الملاحظات فقط إذا كانت Inspect هي 'Ns' أو حسب منطقك
            // إذا كانت Inspect ليست 'Ns'، قد ترغب في مسح الملاحظات
            if (inspect?.ToLower() == "ns")
            {
                fileToUpdate.Nots = nots;
            }
            else
            {
                fileToUpdate.Nots = null; // أو string.Empty حسب تصميم قاعدة البيانات
            }


            try
            {
                _airportrequestfiles.Entity.Update(fileToUpdate); // أو _context.Entry(fileToUpdate).State = EntityState.Modified;
                await _airportrequestfiles.SaveAsync();

                // إرجاع رد ناجح (يمكن أن يكون فارغًا أو يحتوي على رسالة)
                return Ok(new { message = "Attachment updated successfully." });
            }
            catch (DbUpdateException ex)
            {
                // التعامل مع أخطاء قاعدة البيانات
                // تسجيل الخطأ (Logging)
                Console.WriteLine($"Error updating attachment {requestFileId}: {ex.Message}"); // مثال بسيط للتسجيل
                return BadRequest(new { message = "Database error occurred while updating." });
            }
            catch (Exception ex)
            {
                // التعامل مع أي أخطاء أخرى غير متوقعة
                Console.WriteLine($"Unexpected error updating attachment {requestFileId}: {ex.Message}"); // مثال بسيط للتسجيل
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An unexpected error occurred." });
            }
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeStatus(Guid id, string newStatus)
        {
            var validStatuses = new[] { "Pending", "Approved", "Rejected" };
            if (string.IsNullOrEmpty(newStatus) || !validStatuses.Contains(newStatus))
            {
                TempData["ErrorMessage"] = "Invalid status value provided.";
                return RedirectToAction(nameof(Details), new { id = id });
            }

            var requestToUpdate = await _airportrequest.Entity.GetByIdAsync(id);

            if (requestToUpdate == null)
            {
                TempData["ErrorMessage"] = "Request not found.";
                return NotFound($"Unable to find request with ID {id}.");
            }

            requestToUpdate.RequestStatus = newStatus;

            try
            {
                _airportrequest.Entity.Update(requestToUpdate);
                await _airportrequest.SaveAsync();

                TempData["SuccessMessage"] = "Request status updated successfully.";
                return RedirectToAction(nameof(Details), new { id = requestToUpdate.Id });
            }
            catch (DbUpdateException ex)
            {
                TempData["ErrorMessage"] = "An error occurred while updating the status. Please try again.";
                // Consider logging the exception details: _logger.LogError(ex, "Error updating status for request ID {RequestId}", id);
                return RedirectToAction(nameof(Details), new { id = id });
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An unexpected error occurred.";
                // Consider logging the exception details: _logger.LogError(ex, "Unexpected error changing status for request ID {RequestId}", id);
                return RedirectToAction(nameof(Details), new { id = id });
            }
        }

    }
}