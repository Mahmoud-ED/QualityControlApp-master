using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using QualityControlApp.Models.Entities;
using QualityControlApp.Models;
using QualityControlApp.Models.Interfaces;
using QualityControlApp.Classes;
using Microsoft.AspNetCore.Authorization;
using QualityControlApp.ViewModels;
using System.IO; // Added for Path and FileStream
using System;   // Added for Guid

namespace QualityControlApp.Controllers
{
    public class LandingController : BaseController
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _host;
        private readonly IUnitOfWork<Contact> _contact;
        private readonly IUnitOfWork<SiteInfo> _siteInfo;
        private readonly IEmailSender _emailSender;
        private readonly IUnitOfWork<Landing> _landing;
        private readonly IUnitOfWork<ApplicationUser> _applicationUser;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IViewHelper _viewHelper;

        public LandingController(
              UserManager<ApplicationUser> userManager,
              IUnitOfWork<ApplicationUser> applicationUser,
              IWebHostEnvironment host,
              IEmailSender emailSender,
              IUnitOfWork<SiteInfo> siteInfo,
              IUnitOfWork<Contact> contact,
              IUnitOfWork<Landing> landing,
              IViewHelper viewHelper,
              IServiceProvider serviceProvider,
              IHttpContextAccessor httpContextAccessor) : base(host)
        {
            _landing = landing;
            _userManager = userManager;
            _siteInfo = siteInfo;
            _contact = contact;
            _emailSender = emailSender;
            _applicationUser = applicationUser;
            _viewHelper = viewHelper;
            _httpContextAccessor = httpContextAccessor;
            _host = host; // Ensure _host is assigned if not done by BaseController
        }

        [Authorize]
        [ViewLayout("_LayoutDashboard")]

        public async Task<IActionResult> Index(string? operatorName, string? aircraftReg, DateTime? flightDateFrom, DateTime? flightDateTo)
        {
            var landings = _landing.Entity.GetAll();

            if (!string.IsNullOrEmpty(operatorName))
            {
                landings = landings.Where(l => l.OperatorName.Contains(operatorName));
            }

            if (!string.IsNullOrEmpty(aircraftReg))
            {
                landings = landings.Where(l => l.AircraftRegistration.Contains(aircraftReg));
            }

            if (flightDateFrom.HasValue)
            {
                landings = landings.Where(l => l.DateOfFlights >= flightDateFrom.Value.Date);
            }

            if (flightDateTo.HasValue)
            {
                landings = landings.Where(l => l.DateOfFlights < flightDateTo.Value.Date.AddDays(1));
            }

            landings = landings.OrderByDescending(l => l.DateOfFlights).ThenByDescending(l => l.ETA);

            var result = await landings.ToListAsync();
            return View(result);
        }

        [Authorize]
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var landing = await _landing.Entity.GetByIdAsync(id);
            if (landing == null)
            {
                return NotFound();
            }
            return View(landing);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Landing landing, IFormFile? AocDocumentFile)
        {
            //if (!ModelState.IsValid)
            //{
            //    return View(landing);
            //}
            landing.Created = DateTime.Now;
            landing.RequestStatus = "Pending";

            string? uniqueFileName = null;
            string? relativeFilePath = null;
            string fullFilePathToSave = null; // To hold the path for potential deletion on DB error

            if (AocDocumentFile != null && AocDocumentFile.Length > 0)
            {
                string uploadsFolder = Path.Combine(_host.WebRootPath, "pictures", "landing", "aoc");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                var fileName = Path.GetFileNameWithoutExtension(AocDocumentFile.FileName);
                var extension = Path.GetExtension(AocDocumentFile.FileName);
                uniqueFileName = $"{fileName}_{Guid.NewGuid()}{extension}";
                fullFilePathToSave = Path.Combine(uploadsFolder, uniqueFileName);
                relativeFilePath = $"/pictures/landing/aoc/{uniqueFileName}"; // Store relative path

                try
                {
                    using (var stream = new FileStream(fullFilePathToSave, FileMode.Create))
                    {
                        await AocDocumentFile.CopyToAsync(stream);
                    }
                    landing.AocDocumentPath = relativeFilePath;
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("AocDocumentFile", $"File upload failed: {ex.Message}");
                    return View(landing);
                }
            }
            else
            {
                landing.AocDocumentPath = null;
            }

            try
            {
                _landing.Entity.Insert(landing);
                await _landing.SaveAsync();
                TempData["SuccessMessage"] = "Landing record created successfully.";
                //---------------------------------------

                var filePathEmail = _host.WebRootPath + "\\templates" + "\\LandingOnCreate.html";


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
                var editUrl = Url.Action("Edit", "landing", new { id = landing.Id }, Request.Scheme);

                //تم استعماله مرتين: مرة ضمن الرسالة ومرة اخرى في عنوان الايميل ولكنه نفس العنوان// Subject
                //content = content.Replace("{SiteName}", SiteInfo.Name); // يظهر داخل الرسالة
                //content = content.Replace("{Phone}", contact.Phone);
                //content = content.Replace("{ActionUrl}", editUrl);
                //content = content.Replace("{Mail}", contact.Email);


                content = content.Replace("{SiteName}", SiteInfo.Name);
                content = content.Replace("{ApplicantName}", landing.OperatorName); // أو اسم جهة اتصال إذا كان لديك
                content = content.Replace("{AircraftRegistration}", landing.AircraftRegistration);
                content = content.Replace("{AircraftType}", landing.AircraftType);
                content = content.Replace("{OperatorName}", landing.OperatorName);
                content = content.Replace("{FlightDate}", landing.DateOfFlights.ToString("dd MMM yyyy"));
                content = content.Replace("{ETA}", landing.ETA.ToString("dd MMM yyyy, HH:mm"));
                content = content.Replace("{ETD}", landing.ETD.ToString("dd MMM yyyy, HH:mm"));
                content = content.Replace("{Route}", landing.Route);
                content = content.Replace("{PurposeOfFlight}", landing.PurposeOfFlight );
                content = content.Replace("{ActionUrl}", editUrl);
                content = content.Replace("{SupportEmail}", contact.Email);
                content = content.Replace("{SupportPhone}", contact.Phone);
                content = content.Replace("{Mail}", contact.Email);
                content = content.Replace("{Phone}", contact.Phone);
                content = content.Replace("{CurrentYear}", DateTime.Now.Year.ToString());
                var message = new Message(new string[] { landing.Email }, "Landing Request", content, null);
                var message2 = new Message(new string[] { EmailInfo.Email }, "Landing Request"+"-"+landing.Email, content, null);

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














            //--------------------------------------------



                return RedirectToAction(nameof(Create));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An error occurred while saving the landing record. Please try again.");
                // Attempt to delete the uploaded file if DB save failed
                if (!string.IsNullOrEmpty(fullFilePathToSave) && System.IO.File.Exists(fullFilePathToSave))
                {
                    try { System.IO.File.Delete(fullFilePathToSave); }
                    catch (IOException ioEx) { /* Log IO exception during cleanup */ }
                }
                return View(landing);
            }
        }


        [Authorize]
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var landing = await _landing.Entity.GetByIdAsync(id.Value);
            if (landing == null)
            {
                return NotFound();
            }
            return View(landing);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Edit(Guid id, Landing landing, IFormFile? AocDocumentFile)
        {
            if (id != landing.Id)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(landing);
            }

            string? oldRelativePath = null;
            string? newRelativePath = null;
            string? fullPathToNewFile = null;

            var existingLanding = await _landing.Entity.GetByIdAsync(id);
            if (existingLanding != null)
            {
                oldRelativePath = existingLanding.AocDocumentPath;
            }
            else // Should not happen if id check passes, but good practice
            {
                return NotFound("Original record not found for update comparison.");
            }


            if (AocDocumentFile != null && AocDocumentFile.Length > 0)
            {
                string uploadsFolder = Path.Combine(_host.WebRootPath, "documents", "landing", "aoc");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                var fileName = Path.GetFileNameWithoutExtension(AocDocumentFile.FileName);
                var extension = Path.GetExtension(AocDocumentFile.FileName);
                var uniqueFileName = $"{fileName}_{Guid.NewGuid()}{extension}";
                fullPathToNewFile = Path.Combine(uploadsFolder, uniqueFileName);
                newRelativePath = $"/documents/landing/aoc/{uniqueFileName}";

                try
                {
                    using (var stream = new FileStream(fullPathToNewFile, FileMode.Create))
                    {
                        await AocDocumentFile.CopyToAsync(stream);
                    }
                    landing.AocDocumentPath = newRelativePath; // Set new path for update

                    // Try deleting old file after successful new file upload
                    if (!string.IsNullOrEmpty(oldRelativePath))
                    {
                        string oldFullPath = Path.Combine(_host.WebRootPath, oldRelativePath.TrimStart('/'));
                        if (System.IO.File.Exists(oldFullPath))
                        {
                            try { System.IO.File.Delete(oldFullPath); }
                            catch (IOException ioEx) { /* Log failure to delete old file */ }
                        }
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("AocDocumentFile", $"New file upload failed: {ex.Message}");
                    // Don't proceed with DB update if file fails
                    return View(landing);
                }
            }
            else
            {
                landing.AocDocumentPath = oldRelativePath; // Keep the old path if no new file uploaded
            }

            try
            {
                _landing.Entity.Update(landing);
                await _landing.SaveAsync();
                TempData["SuccessMessage"] = "Landing record updated successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await LandingExists(landing.Id))
                {
                    return NotFound();
                }
                else
                {
                    ModelState.AddModelError("", "The record you attempted to edit was modified by another user. The edit operation was canceled.");
                    return View(landing);
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An error occurred while updating the record. Please try again.");
                // Consider if the newly uploaded file should be deleted if DB update fails here
                if (!string.IsNullOrEmpty(fullPathToNewFile) && System.IO.File.Exists(fullPathToNewFile))
                {
                    // Maybe delete fullPathToNewFile ? Depends on desired behavior.
                }
                return View(landing);
            }
        }

        [Authorize(Roles = "Admin,Prog")]
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var landing = await _landing.Entity.GetByIdAsync(id.Value);
            if (landing == null)
            {
                return NotFound();
            }
            return View(landing);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Prog")]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var landingToDelete = await _landing.Entity.GetByIdAsync(id);
            if (landingToDelete == null)
            {
                TempData["ErrorMessage"] = "Landing record not found or already deleted.";
                return RedirectToAction(nameof(Index));
            }

            string? relativeFilePathToDelete = landingToDelete.AocDocumentPath;

            try
            {
                _landing.Entity.Delete(landingToDelete);
                await _landing.SaveAsync();

                if (!string.IsNullOrEmpty(relativeFilePathToDelete))
                {
                    string fullPathToDelete = Path.Combine(_host.WebRootPath, relativeFilePathToDelete.TrimStart('/'));
                    if (System.IO.File.Exists(fullPathToDelete))
                    {
                        try { System.IO.File.Delete(fullPathToDelete); }
                        catch (IOException ioEx) { /* Log failure to delete file, but DB record is gone */ }
                    }
                }

                TempData["SuccessMessage"] = "Landing record deleted successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An error occurred while deleting the record. It might be in use.";
                return RedirectToAction(nameof(Index));
            }
        }

        private async Task<bool> LandingExists(Guid id)
        {
            var entity = await _landing.Entity.GetByIdAsync(id);
            return entity != null;
        }


        [Authorize(Roles = "Prog,Admin")]
        public async Task<IActionResult> ChangeStatus(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var airPortRequest = await _landing.Entity
                .Include(r => r.ApplicationUser)
                .Include(r => r.RequestFiles)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (airPortRequest == null)
            {
                return NotFound();
            }

            return View(airPortRequest);
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

            var requestToUpdate = await _landing.Entity.GetByIdAsync(id);

            if (requestToUpdate == null)
            {
                TempData["ErrorMessage"] = "Request not found.";
                return NotFound($"Unable to find request with ID {id}.");
            }

            requestToUpdate.RequestStatus = newStatus;

            try
            {
                _landing.Entity.Update(requestToUpdate);
                await _landing.SaveAsync();

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