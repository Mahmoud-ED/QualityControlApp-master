using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using QualityControlApp.Models.Entities;
using QualityControlApp.Models;
using QualityControlApp.Models.Interfaces;
using QualityControlApp.Classes;
using Microsoft.AspNetCore.Authorization;
using QualityControlApp.ViewModels;
using QualityControlApp.Services;
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
        private readonly IFileService _fileService;
        private readonly IValidationService _validationService;
        private readonly ILoggingService _loggingService;
        private readonly ILogger<LandingController> _logger;

        public LandingController(
              UserManager<ApplicationUser> userManager,
              IUnitOfWork<ApplicationUser> applicationUser,
              IWebHostEnvironment host,
              IConfiguration configuration,
              IEmailSender emailSender,
              IUnitOfWork<SiteInfo> siteInfo,
              IUnitOfWork<Contact> contact,
              IUnitOfWork<Landing> landing,
              IViewHelper viewHelper,
              IServiceProvider serviceProvider,
              IHttpContextAccessor httpContextAccessor,
              IFileService fileService,
              IValidationService validationService,
              ILoggingService loggingService,
              ILogger<LandingController> logger) : base(host, configuration)
        {
            _landing = landing;
            _userManager = userManager;
            _siteInfo = siteInfo;
            _contact = contact;
            _emailSender = emailSender;
            _applicationUser = applicationUser;
            _viewHelper = viewHelper;
            _httpContextAccessor = httpContextAccessor;
            _fileService = fileService;
            _validationService = validationService;
            _loggingService = loggingService;
            _logger = logger;
            _host = host; // Ensure _host is assigned if not done by BaseController
        }

        [Authorize]
        [ViewLayout("_LayoutDashboard")]
        public async Task<IActionResult> Index(string? operatorName, string? aircraftReg, DateTime? flightDateFrom, DateTime? flightDateTo)
        {
            ViewData["CurrentOperatorName"] = operatorName;
            ViewData["CurrentAircraftReg"] = aircraftReg;
            ViewData["CurrentFlightDateFrom"] = flightDateFrom?.ToString("yyyy-MM-dd");
            ViewData["CurrentFlightDateTo"] = flightDateTo?.ToString("yyyy-MM-dd");

            IQueryable<Landing> landingsQuery = _landing.Entity.GetAll();

            if (!string.IsNullOrEmpty(operatorName))
            {
                landingsQuery = landingsQuery.Where(l => l.OperatorName.Contains(operatorName));
            }
            if (!string.IsNullOrEmpty(aircraftReg))
            {
                landingsQuery = landingsQuery.Where(l => l.AircraftRegistration.Contains(aircraftReg));
            }
            if (flightDateFrom.HasValue)
            {
                landingsQuery = landingsQuery.Where(l => l.DateOfFlights >= flightDateFrom.Value.Date);
            }
            if (flightDateTo.HasValue)
            {
                landingsQuery = landingsQuery.Where(l => l.DateOfFlights < flightDateTo.Value.Date.AddDays(1));
            }

            var result = landingsQuery
                                .OrderByDescending(l => l.DateOfFlights)
                                .ThenByDescending(l => l.ETA) // ????? ?? ETA ?? ???
                                .ToList();
            return View(result);
        }

        // --- API Endpoints for Landing KPIs ---

        [HttpGet]
        public async Task<IActionResult> GetLandingsByOperatorKpiData() // ?? ????? ?????
        {
            var dataForChart = _landing.Entity.GetAll()
                .GroupBy(l => l.RequestStatus) // ??????? ??? ??? ??????
                .Select(g => new { RequestStatus = g.Key ?? "Unknown", Count = g.Count() })
                .OrderByDescending(x => x.Count) // ????? ??? ?????? ??????
                .Take(10) // ??? ???? 10 ?????? ?????
                .ToList();

            var labels = dataForChart.Select(x => x.RequestStatus).ToList();
            var data = dataForChart.Select(x => x.Count).ToList();

            return Json(new { labels = labels, data = data });
        }

        [HttpGet]
        public async Task<IActionResult> GetLast30DaysLandingsKpiData() // ?? ????? ?????
        {
            var today = DateTime.UtcNow.Date.AddDays(1);
            var labels = new List<string>();
            var data = new List<int>();

            for (int i = 29; i >= 0; i--)
            {
                var date = today.AddDays(-i);
                var count = _landing.Entity.GetAll()
                                    .Count(l => l.Created.Date == date); // ??????? DateOfFlights
                labels.Add(date.ToString("MMM dd"));
                data.Add(count);
            }
            return Json(new { labels = labels, data = data });
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
            try
            {
                // Validate model state
                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Model state is invalid for landing creation");
                    return View(landing);
                }

                // Additional validation using our validation service
                var (isValid, validationErrors) = await _validationService.ValidateLandingAsync(landing);
                if (!isValid)
                {
                    foreach (var error in validationErrors)
                    {
                        ModelState.AddModelError("", error);
                    }
                    _logger.LogWarning("Validation failed for landing creation: {Errors}", string.Join(", ", validationErrors));
                    return View(landing);
                }

                // Set default values
                landing.Created = DateTime.UtcNow;
                landing.RequestStatus = "Pending";

                // Handle file upload if provided
                if (AocDocumentFile != null && AocDocumentFile.Length > 0)
                {
                    var (success, filePath, errorMessage) = await _fileService.UploadAocDocumentAsync(AocDocumentFile);
                    if (!success)
                    {
                        ModelState.AddModelError("AocDocumentFile", errorMessage ?? "File upload failed");
                        _logger.LogWarning("File upload failed: {ErrorMessage}", errorMessage);
                        return View(landing);
                    }
                    landing.AocDocumentPath = filePath;

                    // Log file upload
                    await _loggingService.LogFileUploadedAsync(AocDocumentFile.FileName, filePath, _userManager.GetUserId(User));
                }
                else
                {
                    landing.AocDocumentPath = null;
                }

                // Save to database
                _landing.Entity.Insert(landing);
                await _landing.SaveAsync();

                // Log successful creation
                await _loggingService.LogLandingCreatedAsync(landing, _userManager.GetUserId(User));

                TempData["SuccessMessage"] = "Landing record created successfully.";
                    //---------------------------------------

                    var filePathEmail = _host.WebRootPath + "\\templates" + "\\LandingOnCreate.html";


                    var EmailInfo = _contact.Entity.GetAll().FirstOrDefault();
                    if (EmailInfo == null)
                    {
                        return NotFound();
                    }
                    var SiteInfo = _siteInfo.Entity.GetAll().FirstOrDefault();
                    if (SiteInfo == null)
                    {
                        return NotFound();
                    }

                    var contact = _contact.Entity.GetAll().FirstOrDefault();
                    if (contact == null)
                    {
                        return NotFound();
                    }

                    StreamReader htmlFile = new StreamReader(filePathEmail);
                    string content = htmlFile.ReadToEnd();
                    htmlFile.Close();
                    var editUrl = Url.Action("Edit", "landing", new { id = landing.Id }, Request.Scheme);

                    //?? ???????? ?????: ??? ??? ??????? ???? ???? ?? ????? ??????? ????? ??? ???????// Subject
                    //content = content.Replace("{SiteName}", SiteInfo.Name); // ???? ???? ???????
                    //content = content.Replace("{Phone}", contact.Phone);
                    //content = content.Replace("{ActionUrl}", editUrl);
                    //content = content.Replace("{Mail}", contact.Email);


                    content = content.Replace("{SiteName}", SiteInfo.Name);
                    content = content.Replace("{ApplicantName}", landing.OperatorName); // ?? ??? ??? ????? ??? ??? ????
                    content = content.Replace("{AircraftRegistration}", landing.AircraftRegistration);
                    content = content.Replace("{AircraftType}", landing.AircraftType);
                    content = content.Replace("{OperatorName}", landing.OperatorName);
                    content = content.Replace("{FlightDate}", landing.DateOfFlights.ToString("dd MMM yyyy"));
                    content = content.Replace("{ETA}", landing.ETA.ToString("dd MMM yyyy, HH:mm"));
                    content = content.Replace("{ETD}", landing.ETD.ToString("dd MMM yyyy, HH:mm"));
                    content = content.Replace("{Route}", landing.Route);
                    content = content.Replace("{PurposeOfFlight}", landing.PurposeOfFlight);
                    content = content.Replace("{ActionUrl}", editUrl);
                    content = content.Replace("{SupportEmail}", contact.Email);
                    content = content.Replace("{SupportPhone}", contact.Phone);
                    content = content.Replace("{Mail}", contact.Email);
                    content = content.Replace("{Phone}", contact.Phone);
                    content = content.Replace("{CurrentYear}", DateTime.Now.Year.ToString());
                    var message = new Message(new string[] { landing.Email }, "Landing Request", content, null);
                    var message2 = new Message(new string[] { EmailInfo.Email }, "Landing Request" + "-" + landing.Email, content, null);

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














                // Send email notifications
                await SendLandingCreatedEmailAsync(landing);

                return RedirectToAction(nameof(Create));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating landing record");
                await _loggingService.LogErrorAsync("Error creating landing record", ex, _userManager.GetUserId(User));
                
                ModelState.AddModelError("", "An error occurred while saving the landing record. Please try again.");
                
                // Clean up uploaded file if database save failed
                if (!string.IsNullOrEmpty(landing.AocDocumentPath))
                {
                    await _fileService.DeleteFileAsync(landing.AocDocumentPath);
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

            var airPortRequest = _landing.Entity.GetByIdAsync(id).Result;

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
            requestToUpdate.Modified =DateTime.Now;
            requestToUpdate.ApproverUserId = _userManager.GetUserId(User);

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

        // ==================== REPORTS SECTION ====================

        [Authorize]
        [ViewLayout("_LayoutDashboard")]
        public IActionResult Reports()
        {
            ViewData["Title"] = "Passengers per Operator";
            return View();
        }

        [Authorize]
        [ViewLayout("_LayoutDashboard")]
        public async Task<IActionResult> DetailedReport(
            string? operatorName,
            string? aircraftReg,
            string? requestStatus,
            DateTime? dateFrom,
            DateTime? dateTo,
            int page = 1,
            int pageSize = 50)
        {
            ViewData["Title"] = "Detailed Landing Report";

            // Store filter values for view
            ViewBag.OperatorName = operatorName;
            ViewBag.AircraftReg = aircraftReg;
            ViewBag.RequestStatus = requestStatus;
            ViewBag.DateFrom = dateFrom?.ToString("yyyy-MM-dd");
            ViewBag.DateTo = dateTo?.ToString("yyyy-MM-dd");

            // Build query with filters
            var query = _landing.Entity.GetAll();

            if (!string.IsNullOrEmpty(operatorName))
                query = query.Where(l => l.OperatorName.Contains(operatorName));

            if (!string.IsNullOrEmpty(aircraftReg))
                query = query.Where(l => l.AircraftRegistration.Contains(aircraftReg));

            if (!string.IsNullOrEmpty(requestStatus))
                query = query.Where(l => l.RequestStatus == requestStatus);

            if (dateFrom.HasValue)
                query = query.Where(l => l.DateOfFlights >= dateFrom.Value);

            if (dateTo.HasValue)
                query = query.Where(l => l.DateOfFlights <= dateTo.Value);

            // Get total count for pagination
            var totalCount = query.Count();
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            // Apply pagination
            var landings = query
                .OrderByDescending(l => l.Created)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.TotalCount = totalCount;

            return View(landings);
        }

        [Authorize]
        [ViewLayout("_LayoutDashboard")]
        public async Task<IActionResult> StatusReport()
        {
            ViewData["Title"] = "Landing Status Report";

            // Get status distribution
            var totalCount = _landing.Entity.GetAll().Count();
            var statusStats = _landing.Entity.GetAll()
                .GroupBy(l => l.RequestStatus)
                .Select(g => new
                {
                    Status = g.Key ?? "Unknown",
                    Count = g.Count(),
                    Percentage = Math.Round((double)g.Count() / totalCount * 100, 2)
                })
                .OrderByDescending(x => x.Count)
                .ToList();

            // Get status by operator
            var statusByOperator = _landing.Entity.GetAll()
                .GroupBy(l => new { l.OperatorName, l.RequestStatus })
                .Select(g => new
                {
                    OperatorName = g.Key.OperatorName,
                    Status = g.Key.RequestStatus ?? "Unknown",
                    Count = g.Count()
                })
                .OrderByDescending(x => x.Count)
                .ToList();

            ViewBag.StatusStats = statusStats;
            ViewBag.StatusByOperator = statusByOperator;

            return View();
        }

        [Authorize]
        [ViewLayout("_LayoutDashboard")]
        public IActionResult AdvancedCharts()
        {
            ViewData["Title"] = "??????? ?????? ???????";
            return View();
        }

        [Authorize]
        [ViewLayout("_LayoutDashboard")]
        public async Task<IActionResult> DateRangeReport(
            DateTime? startDate,
            DateTime? endDate,
            string? groupBy = "day")
        {
            ViewData["Title"] = "Date Range Landing Report";

            // Set default dates if not provided
            if (!startDate.HasValue)
                startDate = DateTime.Now.AddMonths(-1);
            if (!endDate.HasValue)
                endDate = DateTime.Now;

            ViewBag.StartDate = startDate.Value.ToString("yyyy-MM-dd");
            ViewBag.EndDate = endDate.Value.ToString("yyyy-MM-dd");
            ViewBag.GroupBy = groupBy;

            // Get data based on groupBy parameter
            var query = _landing.Entity.GetAll()
                .Where(l => l.DateOfFlights >= startDate && l.DateOfFlights <= endDate);

            object groupedData = null;

            switch (groupBy?.ToLower())
            {
                case "month":
                    groupedData = query
                        .GroupBy(l => new { l.DateOfFlights.Year, l.DateOfFlights.Month })
                        .Select(g => new
                        {
                            Year = g.Key.Year,
                            Month = g.Key.Month,
                            Count = g.Count()
                        })
                        .AsEnumerable()
                        .Select(g => new
                        {
                            Period = $"{g.Year}-{g.Month:D2}",
                            DisplayPeriod = $"{new DateTime(g.Year, g.Month, 1):MMM yyyy}",
                            Count = g.Count
                        })
                        .OrderBy(x => x.Period)
                        .ToList();
                    break;

                case "week":
                    groupedData = query
                        .GroupBy(l => new
                        {
                            Year = l.DateOfFlights.Year,
                            Week = (l.DateOfFlights.DayOfYear - 1) / 7 + 1
                        })
                        .Select(g => new
                        {
                            Year = g.Key.Year,
                            Week = g.Key.Week,
                            Count = g.Count()
                        })
                        .AsEnumerable()
                        .Select(g => new
                        {
                            Period = $"{g.Year}-W{g.Week:D2}",
                            DisplayPeriod = $"Week {g.Week} ({g.Year})",
                            Count = g.Count
                        })
                        .OrderBy(x => x.Period)
                        .ToList();
                    break;

                default: // day
                    groupedData = query
                        .GroupBy(l => l.DateOfFlights.Date)
                        .Select(g => new
                        {
                            Period = g.Key,
                            Count = g.Count()
                        })
                        .AsEnumerable()
                        .Select(g => new
                        {
                            Period = g.Period.ToString("yyyy-MM-dd"),
                            DisplayPeriod = g.Period.ToString("dd MMM yyyy"),
                            Count = g.Count
                        })
                        .OrderBy(x => x.Period)
                        .ToList();
                    break;
            }

            ViewBag.GroupedData = groupedData;

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> ExportToExcel(
            string? operatorName,
            string? aircraftReg,
            string? requestStatus,
            DateTime? dateFrom,
            DateTime? dateTo)
        {
            // Build query with filters
            var query = _landing.Entity.GetAll();

            if (!string.IsNullOrEmpty(operatorName))
                query = query.Where(l => l.OperatorName.Contains(operatorName));

            if (!string.IsNullOrEmpty(aircraftReg))
                query = query.Where(l => l.AircraftRegistration.Contains(aircraftReg));

            if (!string.IsNullOrEmpty(requestStatus))
                query = query.Where(l => l.RequestStatus == requestStatus);

            if (dateFrom.HasValue)
                query = query.Where(l => l.DateOfFlights >= dateFrom.Value);

            if (dateTo.HasValue)
                query = query.Where(l => l.DateOfFlights <= dateTo.Value);

            var landings = query
                .OrderByDescending(l => l.Created)
                .ToList();

            // For now, return JSON. In a real implementation, you'd use a library like EPPlus or ClosedXML
            // to generate actual Excel files
            var data = landings.Select(l => new
            {
                l.Id,
                l.Email,
                l.OperatorName,
                l.AircraftRegistration,
                l.AircraftType,
                FlightDate = l.DateOfFlights.ToString("yyyy-MM-dd"),
                ETA = l.ETA.ToString("HH:mm"),
                ETD = l.ETD.ToString("HH:mm"),
                l.Route,
                l.RequestStatus,
                CreatedDate = l.Created.ToString("yyyy-MM-dd HH:mm")
            });

            return Json(new { success = true, data = data, totalRecords = landings.Count });
        }

        [HttpGet]
        public async Task<IActionResult> GetLandingChartsData()
        {
            try
            {
                // ???????? ????
                var total = _landing.Entity.GetAll().Count();
                var pending = _landing.Entity.GetAll().Count(l => l.RequestStatus == "Pending");
                var approved = _landing.Entity.GetAll().Count(l => l.RequestStatus == "Approved");
                var rejected = _landing.Entity.GetAll().Count(l => l.RequestStatus == "Rejected");

                var stats = new
                {
                    total = total,
                    pending = pending,
                    approved = approved,
                    rejected = rejected
                };

                Console.WriteLine($"???????? ???????: Total={total}, Pending={pending}, Approved={approved}, Rejected={rejected}");

                // ????? ????????
                Console.WriteLine("??? ????? ?????? ????????...");
                var operators = _landing.Entity.GetAll()
                    .GroupBy(l => l.OperatorName)
                    .Select(g => new { label = g.Key ?? "??? ????", value = g.Count() })
                    .OrderByDescending(x => x.value)
                    .Take(10)
                    .ToList();

                Console.WriteLine($"??? ????????: {operators.Count}");
                foreach (var op in operators)
                {
                    Console.WriteLine($"????: {op.label}, ??? ???????: {op.value}");
                }

                // ????????? ??????? (??? 12 ?????)
                var monthly = new List<object>();
                for (int i = 11; i >= 0; i--)
                {
                    var date = DateTime.Now.AddMonths(-i);
                    var count = _landing.Entity.GetAll().Count(l =>
                        l.Created.Year == date.Year && l.Created.Month == date.Month);
                    monthly.Add(new
                    {
                        month = date.ToString("MMM yyyy"),
                        count = count
                    });
                }

                // ????? ????? ????????
                var aircraftTypes = _landing.Entity.GetAll()
                    .GroupBy(l => l.AircraftType)
                    .Select(g => new { label = g.Key ?? "??? ????", value = g.Count() })
                    .OrderByDescending(x => x.value)
                    .Take(10)
                    .ToList();

                // ????? ????????
                var routes = _landing.Entity.GetAll()
                    .GroupBy(l => l.Route)
                    .Select(g => new { label = g.Key ?? "??? ????", value = g.Count() })
                    .OrderByDescending(x => x.value)
                    .Take(10)
                    .ToList();

                // ????? ????????
                var airports = _landing.Entity.GetAll()
                    .GroupBy(l => l.AirportOfLanding)
                    .Select(g => new { label = g.Key ?? "??? ????", value = g.Count() })
                    .OrderByDescending(x => x.value)
                    .Take(10)
                    .ToList();

                // ????? ??????? ??????? ?? ??? NatureOfPaxOrCargo
                var natureOfCargo = _landing.Entity.GetAll()
                    .Where(l => !string.IsNullOrEmpty(l.NatureOfPaxOrCargo))
                    .Select(l => l.NatureOfPaxOrCargo)
                    .ToList();

                // ????? ???? ??????? (???? ?????? ??? ???????? ???????)
                var cargoTypes = new List<object>
                {
                    new { label = "????? ??????", value = natureOfCargo.Count(c => c.Contains("????") || c.Contains("commercial")) },
                    new { label = "???? ??????", value = natureOfCargo.Count(c => c.Contains("????") || c.Contains("food") || c.Contains("????")) },
                    new { label = "????? ????", value = natureOfCargo.Count(c => c.Contains("???") || c.Contains("medical") || c.Contains("????")) },
                    new { label = "?????? ?????????", value = natureOfCargo.Count(c => c.Contains("???????") || c.Contains("electronic")) },
                    new { label = "????? ??????", value = natureOfCargo.Count(c => c.Contains("?????") || c.Contains("????") || c.Contains("textile")) },
                    new { label = "???? ????????", value = natureOfCargo.Count(c => c.Contains("???????") || c.Contains("chemical")) }
                }.Where(x => ((dynamic)x).value > 0).ToList();

                // ????? ?????? ??? ?????? (??????? ????? ??? ???????? ???????)
                var passengersByDestination = new List<object>
                {
                    new { label = "?????? (RUH)", value = _landing.Entity.GetAll().Count(l => l.AirportOfLanding.Contains("RUH") || l.Route.Contains("RUH")) },
                    new { label = "??? (JED)", value = _landing.Entity.GetAll().Count(l => l.AirportOfLanding.Contains("JED") || l.Route.Contains("JED")) },
                    new { label = "??? (DXB)", value = _landing.Entity.GetAll().Count(l => l.AirportOfLanding.Contains("DXB") || l.Route.Contains("DXB")) },
                    new { label = "??????? (CAI)", value = _landing.Entity.GetAll().Count(l => l.AirportOfLanding.Contains("CAI") || l.Route.Contains("CAI")) },
                    new { label = "?????? (KWI)", value = _landing.Entity.GetAll().Count(l => l.AirportOfLanding.Contains("KWI") || l.Route.Contains("KWI")) },
                    new { label = "??????? (BAH)", value = _landing.Entity.GetAll().Count(l => l.AirportOfLanding.Contains("BAH") || l.Route.Contains("BAH")) }
                }.Where(x => ((dynamic)x).value > 0).ToList();

                var result = new
                {
                    success = true,
                    stats = stats,
                    operators = new
                    {
                        labels = operators.Select(x => x.label).ToList(),
                        data = operators.Select(x => ((dynamic)x).value).ToList()
                    },
                    monthly = new
                    {
                        labels = monthly.Select(x => ((dynamic)x).month).ToList(),
                        data = monthly.Select(x => ((dynamic)x).count).ToList()
                    },
                    aircraftTypes = new
                    {
                        labels = aircraftTypes.Select(x => x.label).ToList(),
                        data = aircraftTypes.Select(x => ((dynamic)x).value).ToList()
                    },
                    routes = new
                    {
                        labels = routes.Select(x => x.label).ToList(),
                        data = routes.Select(x => ((dynamic)x).value).ToList()
                    },
                    airports = new
                    {
                        labels = airports.Select(x => x.label).ToList(),
                        data = airports.Select(x => ((dynamic)x).value).ToList()
                    },
                    cargoTypes = new
                    {
                        labels = cargoTypes.Select(x => ((dynamic)x).label).ToList(),
                        data = cargoTypes.Select(x => ((dynamic)x).value).ToList()
                    },
                    passengersByDestination = new
                    {
                        labels = passengersByDestination.Select(x => ((dynamic)x).label).ToList(),
                        data = passengersByDestination.Select(x => ((dynamic)x).value).ToList()
                    }
                };

                Console.WriteLine("?? ????? ??????? ????? ?? GetLandingChartsData");
                return Json(result);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
        [HttpGet]
        public IActionResult GetPassengersByOperator()
        {
            // ?????? ??????? ??? ?????? ?? NatureOfPaxOrCargo ???? ???
            var data = _landing.Entity.GetAll()
                .Select(l => new
                {
                    l.OperatorName,
                    PaxText = l.NatureOfPaxOrCargo
                })
                .AsEnumerable()
                .Select(x => new
                {
                    Operator = string.IsNullOrWhiteSpace(x.OperatorName) ? "??? ????" : x.OperatorName,
                    Pax = TryParsePassengers(x.PaxText)
                })
                .GroupBy(x => x.Operator)
                .Select(g => new { Operator = g.Key, Passengers = g.Sum(z => z.Pax) })
                .OrderByDescending(x => x.Passengers)
                .ToList();

            return Json(new { success = true, labels = data.Select(d => d.Operator), data = data.Select(d => d.Passengers) });
        }

        private int TryParsePassengers(string? text)
        {
            if (string.IsNullOrWhiteSpace(text)) return 0;
            // ????? ??????: "PAX: 45", "Passengers=30", "????: 28"
            var digits = new string(text.Where(char.IsDigit).ToArray());
            if (int.TryParse(digits, out var pax)) return pax;
            return 0;
        }

        [HttpGet]
        public async Task<IActionResult> GetAdvancedChartData(string type = "operators", string range = "30")
        {
            try
            {
                // ????? ???? ???????
                DateTime startDate = DateTime.Now.AddDays(-int.Parse(range));

                var baseQuery = _landing.Entity.GetAll();
                if (range != "all")
                {
                    baseQuery = baseQuery.Where(l => l.Created >= startDate);
                }

                object result = null;
                string title = "";
                string datasetLabel = "";

                switch (type.ToLower())
                {
                    case "operators":
                        var operatorsData = baseQuery
                            .GroupBy(l => l.OperatorName)
                            .Select(g => new { label = g.Key ?? "??? ????", value = g.Count() })
                                                .OrderByDescending(x => x.value)
                    .Take(10)
                    .ToList();

                        result = new
                        {
                            labels = operatorsData.Select(x => x.label).ToList(),
                            values = operatorsData.Select(x => ((dynamic)x).value).ToList(),
                            title = "????? ????????",
                            datasetLabel = "??? ???????"
                        };
                        break;

                    case "aircraft":
                        var aircraftData = baseQuery
                            .GroupBy(l => l.AircraftType)
                            .Select(g => new { label = g.Key ?? "??? ????", value = g.Count() })
                                                        .OrderByDescending(x => x.value)
                    .Take(10)
                    .ToList();

                        result = new
                        {
                            labels = aircraftData.Select(x => x.label).ToList(),
                            values = aircraftData.Select(x => ((dynamic)x).value).ToList(),
                            title = "????? ????? ????????",
                            datasetLabel = "??? ???????"
                        };
                        break;

                    case "routes":
                        var routesData = baseQuery
                            .GroupBy(l => l.Route)
                            .Select(g => new { label = g.Key ?? "??? ????", value = g.Count() })
                                                        .OrderByDescending(x => x.value)
                    .Take(10)
                    .ToList();

                        result = new
                        {
                            labels = routesData.Select(x => x.label).ToList(),
                            values = routesData.Select(x => ((dynamic)x).value).ToList(),
                            title = "????? ????????",
                            datasetLabel = "??? ???????"
                        };
                        break;

                    case "temporal":
                        var temporalData = new List<object>();
                        for (int i = 29; i >= 0; i--)
                        {
                            var date = DateTime.Now.AddDays(-i);
                            var count = baseQuery.Count(l =>
                                l.Created.Date == date.Date);
                            temporalData.Add(new
                            {
                                label = date.ToString("dd/MM"),
                                value = count
                            });
                        }

                        result = new
                        {
                            labels = temporalData.Select(x => ((dynamic)x).label).ToList(),
                            values = temporalData.Select(x => ((dynamic)x).count).ToList(),
                            title = "????? ???? ????",
                            datasetLabel = "??? ???????"
                        };
                        break;

                    case "cargo":
                        var cargoData = baseQuery
                            .Where(l => !string.IsNullOrEmpty(l.NatureOfPaxOrCargo))
                            .Select(l => l.NatureOfPaxOrCargo)
                            .ToList();

                        var cargoAnalysis = new List<object>
                        {
                            new { label = "????? ??????", value = cargoData.Count(c => c.Contains("????") || c.Contains("commercial")) },
                            new { label = "???? ??????", value = cargoData.Count(c => c.Contains("????") || c.Contains("food")) },
                            new { label = "????? ????", value = cargoData.Count(c => c.Contains("???") || c.Contains("medical")) },
                            new { label = "?????? ?????????", value = cargoData.Count(c => c.Contains("???????") || c.Contains("electronic")) },
                            new { label = "????? ??????", value = cargoData.Count(c => c.Contains("?????") || c.Contains("????")) },
                            new { label = "???? ????????", value = cargoData.Count(c => c.Contains("???????") || c.Contains("chemical")) }
                        }.Where(x => ((dynamic)x).value > 0).ToList();

                        result = new
                        {
                            labels = cargoAnalysis.Select(x => ((dynamic)x).label).ToList(),
                            values = cargoAnalysis.Select(x => ((dynamic)x).value).ToList(),
                            title = "????? ???????",
                            datasetLabel = "???? ???????"
                        };
                        break;

                    case "passengers":
                        var passengerData = baseQuery
                            .GroupBy(l => l.AirportOfLanding)
                            .Select(g => new { label = g.Key ?? "??? ????", value = g.Count() * 150 }) // ??????? ??????
                            .OrderByDescending(x => x.value)
                            .Take(10)
                            .ToList();

                        result = new
                        {
                            labels = passengerData.Select(x => x.label).ToList(),
                            values = passengerData.Select(x => ((dynamic)x).value).ToList(),
                            title = "????? ?????? ??? ??????",
                            datasetLabel = "??? ??????"
                        };
                        break;

                    default:
                        result = new
                        {
                            labels = new List<string>(),
                            values = new List<int>(),
                            title = "????? ??? ????",
                            datasetLabel = "????????"
                        };
                        break;
                }

                // ???? ??????????
                var resultDynamic = result as dynamic;
                var values = resultDynamic?.values as List<int>;
                var stats = new
                {
                    total = values?.Sum() ?? 0,
                    avg = values != null && values.Count > 0 ? values.Average() : 0,
                    max = values?.Max() ?? 0,
                    min = values?.Min() ?? 0
                };

                Console.WriteLine("?? ????? ??????? ????? ?? GetAdvancedChartData");
                return Json(new
                {
                    success = true,
                    data = result,
                    stats = stats
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> ExportToPDF(
            string? operatorName,
            string? aircraftReg,
            string? requestStatus,
            DateTime? dateFrom,
            DateTime? dateTo)
        {
            // Build query with filters
            var query = _landing.Entity.GetAll();

            if (!string.IsNullOrEmpty(operatorName))
                query = query.Where(l => l.OperatorName.Contains(operatorName));

            if (!string.IsNullOrEmpty(aircraftReg))
                query = query.Where(l => l.AircraftRegistration.Contains(aircraftReg));

            if (!string.IsNullOrEmpty(requestStatus))
                query = query.Where(l => l.RequestStatus == requestStatus);

            if (dateFrom.HasValue)
                query = query.Where(l => l.DateOfFlights >= dateFrom.Value);

            if (dateTo.HasValue)
                query = query.Where(l => l.DateOfFlights <= dateTo.Value);

            var landings = query
                .OrderByDescending(l => l.Created)
                .Take(1000) // Limit for PDF generation
                .ToList();

            // For now, return JSON with data that can be used to generate PDF on client side
            // In a real implementation, you'd use a library like iTextSharp or PdfSharp
            var data = landings.Select(l => new
            {
                l.Id,
                l.Email,
                l.OperatorName,
                l.AircraftRegistration,
                l.AircraftType,
                FlightDate = l.DateOfFlights.ToString("yyyy-MM-dd"),
                ETA = l.ETA.ToString("HH:mm"),
                ETD = l.ETD.ToString("HH:mm"),
                l.Route,
                l.RequestStatus,
                CreatedDate = l.Created.ToString("yyyy-MM-dd HH:mm")
            });

            return Json(new { success = true, data = data, totalRecords = landings.Count });
        }

        private async Task SendLandingCreatedEmailAsync(Landing landing)
        {
            try
            {
                var filePathEmail = Path.Combine(_host.WebRootPath, "templates", "LandingOnCreate.html");
                if (!System.IO.File.Exists(filePathEmail))
                {
                    _logger.LogWarning("Email template not found: {FilePath}", filePathEmail);
                    return;
                }

                var emailInfo = _contact.Entity.GetAll().FirstOrDefault();
                var siteInfo = _siteInfo.Entity.GetAll().FirstOrDefault();

                if (emailInfo == null || siteInfo == null)
                {
                    _logger.LogWarning("Email or site info not found");
                    return;
                }

                var content = await System.IO.File.ReadAllTextAsync(filePathEmail);
                var editUrl = Url.Action("Edit", "Landing", new { id = landing.Id }, Request.Scheme);

                // Replace placeholders in email template
                content = content.Replace("{SiteName}", siteInfo.Name);
                content = content.Replace("{ApplicantName}", landing.OperatorName);
                content = content.Replace("{AircraftRegistration}", landing.AircraftRegistration);
                content = content.Replace("{AircraftType}", landing.AircraftType);
                content = content.Replace("{OperatorName}", landing.OperatorName);
                content = content.Replace("{FlightDate}", landing.DateOfFlights.ToString("dd MMM yyyy"));
                content = content.Replace("{ETA}", landing.ETA.ToString("dd MMM yyyy, HH:mm"));
                content = content.Replace("{ETD}", landing.ETD.ToString("dd MMM yyyy, HH:mm"));
                content = content.Replace("{Route}", landing.Route);
                content = content.Replace("{PurposeOfFlight}", landing.PurposeOfFlight ?? "");
                content = content.Replace("{ActionUrl}", editUrl);
                content = content.Replace("{SupportEmail}", emailInfo.Email);
                content = content.Replace("{SupportPhone}", emailInfo.Phone);
                content = content.Replace("{Mail}", emailInfo.Email);
                content = content.Replace("{Phone}", emailInfo.Phone);
                content = content.Replace("{CurrentYear}", DateTime.Now.Year.ToString());

                var message = new Message(new string[] { landing.Email }, "Landing Request", content, null);
                var adminMessage = new Message(new string[] { emailInfo.Email }, $"Landing Request - {landing.Email}", content, null);

                await _emailSender.SendEmailAsync(message);
                await _emailSender.SendEmailAsync(adminMessage);

                TempData["SuccessMessage"] = "Landing record created and email notifications sent successfully.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending landing created email");
                TempData["ErrorMessage"] = "Landing record created but failed to send email notifications.";
            }
        }
    }
}
