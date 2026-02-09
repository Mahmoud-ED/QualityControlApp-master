using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using Newtonsoft.Json;
using QualityControlApp.Classes;
using QualityControlApp.Models;
using QualityControlApp.Models.Entities;
using QualityControlApp.Models.Interfaces;
using QualityControlApp.Models.Repositories;
using QualityControlApp.ViewModels;
using SkiaSharp;
using System.ComponentModel.Design;
using System.Diagnostics.Contracts;
using System.Net;
using System.Reflection.Metadata;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

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
                               IConfiguration configuration,
              IEmailSender emailSender,
              IUnitOfWork<SiteInfo> siteInfo,
              IUnitOfWork<Contact> contact,
              IUnitOfWork<AirPortRequest> airportrequest,
              IUnitOfWork<AirPortRequestFiles> airportrequestfiles,
              IViewHelper viewHelper,
          IServiceProvider serviceProvider,
          IHttpContextAccessor httpContextAccessor) : base(host, configuration)
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

        public async Task<IActionResult> Index(
           string? email,
           DateTime? requestDateFrom,
           DateTime? requestDateTo,
           DateTime? flightDateFrom,
           DateTime? flightDateTo,
           string? status)
        {
            ViewData["CurrentEmail"] = email;
            ViewData["CurrentRequestDateFrom"] = requestDateFrom?.ToString("yyyy-MM-dd");
            ViewData["CurrentRequestDateTo"] = requestDateTo?.ToString("yyyy-MM-dd");
            ViewData["CurrentFlightDateFrom"] = flightDateFrom?.ToString("yyyy-MM-dd");
            ViewData["CurrentFlightDateTo"] = flightDateTo?.ToString("yyyy-MM-dd");
            ViewData["CurrentStatus"] = status ?? "All";

            
            IQueryable<AirPortRequest> requestsQuery = _airportrequest.Entity.GetAll().AsQueryable();
            
            if (!string.IsNullOrEmpty(email))
            {
                requestsQuery = requestsQuery.Where(r => r.Email.Contains(email));
            }
            if (requestDateFrom.HasValue)
            {
                requestsQuery = requestsQuery.Where(r => r.RequestTime >= requestDateFrom.Value.Date);
            }
            if (requestDateTo.HasValue)
            {
                requestsQuery = requestsQuery.Where(r => r.RequestTime < requestDateTo.Value.Date.AddDays(1));
            }
            if (flightDateFrom.HasValue)
            {
                requestsQuery = requestsQuery.Where(r => r.FlightDate >= flightDateFrom.Value.Date);
            }
            if (flightDateTo.HasValue)
            {
                requestsQuery = requestsQuery.Where(r => r.FlightDate < flightDateTo.Value.Date.AddDays(1));
            }
            if (!string.IsNullOrEmpty(status) && status != "All")
            {
                requestsQuery = requestsQuery.Where(r => r.RequestStatus == status);
            }

            // ????? ????????? ??????? ???? ????????
            var result = await requestsQuery
                                .OrderByDescending(r => r.RequestTime)
                                .ThenBy(d => d.FlightDate)
                                .ToListAsync();

            // ?? ???? ?????? ?????? ??? KPIs ??? ViewBag ?? ???
            // ??? JavaScript ??????? ?????? ?? ??? API Endpoints.

            return View(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetRequestsByStatusKpiData()
        {
            // ??? ??? Endpoint ??? ?? ???? ???????? ?????? ???? ????? ?? ????? ???? Index
            // ??? ??? ???? ????? ??????? ???? (??? ?? ?? ????? ??? ????)
            var dataForChart = await _airportrequest.Entity.GetAll() // ?? _airportrequestUnitOfWork.Entity.GetAll()
                .GroupBy(r => r.RequestStatus)
                .Select(g => new { Status = g.Key ?? "Unknown", Count = g.Count() })
                .OrderBy(x => x.Status) // ????? ????? ???? ??? labels
                .ToListAsync();

            var labels = dataForChart.Select(x => x.Status).ToList();
            var data = dataForChart.Select(x => x.Count).ToList();

            return Json(new { labels = labels, data = data });
        }

        [HttpGet]
        public async Task<IActionResult> GetLast30DaysRequestsKpiData()
        {
            var today = DateTime.UtcNow.Date;
            var labels = new List<string>();
            var data = new List<int>();

            for (int i = 29; i >= 0; i--)
            {
                var date = today.AddDays(-i);
                // ???? ?? ??? ????????? ????
                var count = await _airportrequest.Entity.GetAll() // ?? _airportrequestUnitOfWork.Entity.GetAll()
                                    .CountAsync(r => r.RequestTime.Date == date);
                labels.Add(date.ToString("MMM dd"));
                data.Add(count);
            }
            return Json(new { labels = labels, data = data });
        }





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
            ViewData["FileTypes"] = _filetype.Entity.GetAll(); // ????? ????? ???????
            return View();
        }

        // POST: AirPortRequests/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AirPortReqeustVM AirPortRequestViewModel)
        {
            //if (ModelState.IsValid)
            //{
            // ????? ??? ????? ??? ????? ??????
            AirPortRequestViewModel.AirPortRequest.RequestTime = DateTime.Now;
            AirPortRequestViewModel.AirPortRequest.Created = DateTime.Now;
            AirPortRequestViewModel.AirPortRequest.RequestStatus = "0";

            // ??? ????? ?? ????? ????????
            _airportrequest.Entity.Insert(AirPortRequestViewModel.AirPortRequest);
            await _airportrequest.SaveAsync();

            // ?????? ??????? ??????? ??? ????
            if (AirPortRequestViewModel.FileTypes != null && AirPortRequestViewModel.FileTypes.Count > 0)
            {
                string uploadsFolder = Path.Combine(_host.WebRootPath, "pictures/requestfiles");

                // ?????? ?? ???? ??????
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

                //?? ???????? ?????: ??? ??? ??????? ???? ???? ?? ????? ??????? ????? ??? ???????// Subject
                content = content.Replace("{SiteName}", SiteInfo.Name); // ???? ???? ???????
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
                // ?????? ??? ????? ?????? ?? ????? ????????
                //var existingRequest = await _airportrequest.Entity
                //    .GetByIdAsync(id);

                // ???????? ???? ????? ??????

                airPortRequest.Modified = DateTime.Now;

                _airportrequest.Entity.Update(airPortRequest);

                // ?????? ??????? ??????? ??????? ??? ????


                if (AirPortRequestViewModel.FileTypes != null && AirPortRequestViewModel.FileTypes.Count > 0)
                {
                    string uploadsFolder = Path.Combine(_host.WebRootPath, "pictures/requestfiles");

                    // ?????? ?? ???? ??????
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
        // ??? ??? ?????? AntiForgeryToken? ???: [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateAttachmentDetails(Guid requestFileId, string inspect, string nots)
        {
            // ????? ????? ?????? ?? ??????? ???????? ???

            // ????? ?? ????? ?? ????? ????????
            // ???? ?? ??????? RequestFile ?????? ?????? ??????? ? Id ?????? ?????? ??? Primary Key
            var fileToUpdate = await _airportrequestfiles .Entity.GetByIdAsync( requestFileId);

            if (fileToUpdate == null)
            {
                return NotFound(new { message = "Attachment not found." });
            }

            // ????? ???????
            fileToUpdate.Inspect = inspect; // ???? ?? ?? ??? ???????? ?????? ?? ?? ???????? ??????

            // ????? ????????? ??? ??? ???? Inspect ?? 'Ns' ?? ??? ?????
            // ??? ???? Inspect ???? 'Ns'? ?? ???? ?? ??? ?????????
            if (inspect?.ToLower() == "ns")
            {
                fileToUpdate.Nots = nots;
            }
            else
            {
                fileToUpdate.Nots = null; // ?? string.Empty ??? ????? ????? ????????
            }


            try
            {
                _airportrequestfiles.Entity.Update(fileToUpdate); // ?? _context.Entry(fileToUpdate).State = EntityState.Modified;
                await _airportrequestfiles.SaveAsync();

                // ????? ?? ???? (???? ?? ???? ?????? ?? ????? ??? ?????)
                return Ok(new { message = "Attachment updated successfully." });
            }
            catch (DbUpdateException ex)
            {
                // ??????? ?? ????? ????? ????????
                // ????? ????? (Logging)
                Console.WriteLine($"Error updating attachment {requestFileId}: {ex.Message}"); // ???? ???? ???????
                return BadRequest(new { message = "Database error occurred while updating." });
            }
            catch (Exception ex)
            {
                // ??????? ?? ?? ????? ???? ??? ??????
                Console.WriteLine($"Unexpected error updating attachment {requestFileId}: {ex.Message}"); // ???? ???? ???????
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An unexpected error occurred." });
            }
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeStatus(Guid id, string newStatus)
        {
            var validStatuses = new[] { "0", "1", "2" };
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


            try
            {
                requestToUpdate.RequestStatus = newStatus;
                requestToUpdate.Modified = DateTime.Now;
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

        // Reports Section
        [HttpGet]
        public IActionResult Reports()
        {
            var reportVM = new AirPortRequestReportVM();
            return View(reportVM);
        }

        [HttpPost]
        public async Task<IActionResult> GenerateReport(AirPortRequestReportVM model)
        {
            if (!ModelState.IsValid)
            {
                return View("Reports", model);
            }

            var result = await GenerateReportData(model);
            
            if (model.ExportFormat == "PDF")
            {
                return await ExportToPdf(result);
            }
            else if (model.ExportFormat == "Excel")
            {
                return await ExportToExcel(result);
            }
            
            return View("ReportResults", result);
        }

        private async Task<AirPortRequestReportResultVM> GenerateReportData(AirPortRequestReportVM filters)
        {
            var query = _airportrequest.Entity.GetAll()
                .Include(r => r.ApplicationUser)
                .Include(r => r.RequestFiles)
                .AsQueryable();

            // Apply filters
            if (filters.DateFrom.HasValue)
            {
                query = query.Where(r => r.RequestTime >= filters.DateFrom.Value.Date);
            }

            if (filters.DateTo.HasValue)
            {
                query = query.Where(r => r.RequestTime < filters.DateTo.Value.Date.AddDays(1));
            }

            if (!string.IsNullOrEmpty(filters.CompanyName))
            {
                query = query.Where(r => r.CompanyName.Contains(filters.CompanyName));
            }

            if (!string.IsNullOrEmpty(filters.Department))
            {
                query = query.Where(r => r.Department.Contains(filters.Department));
            }

            if (!string.IsNullOrEmpty(filters.RequestStatus) && filters.RequestStatus != "All")
            {
                query = query.Where(r => r.RequestStatus == filters.RequestStatus);
            }

            if (!string.IsNullOrEmpty(filters.AircraftType))
            {
                query = query.Where(r => r.AircraftType.Contains(filters.AircraftType));
            }

            if (!string.IsNullOrEmpty(filters.FlightPurpose))
            {
                query = query.Where(r => r.FlightPurpose.Contains(filters.FlightPurpose));
            }

            if (filters.CrewCountFrom.HasValue)
            {
                query = query.Where(r => r.CrewCount >= filters.CrewCountFrom.Value);
            }

            if (filters.CrewCountTo.HasValue)
            {
                query = query.Where(r => r.CrewCount <= filters.CrewCountTo.Value);
            }

            if (!string.IsNullOrEmpty(filters.Email))
            {
                query = query.Where(r => r.Email.Contains(filters.Email));
            }

            if (!string.IsNullOrEmpty(filters.PilotName))
            {
                query = query.Where(r => r.PilotName != null && r.PilotName.Contains(filters.PilotName));
            }

            if (!string.IsNullOrEmpty(filters.FlightNumber))
            {
                query = query.Where(r => r.FlightNumber != null && r.FlightNumber.Contains(filters.FlightNumber));
            }

            if (!string.IsNullOrEmpty(filters.EntryPoint))
            {
                query = query.Where(r => r.EntryPoint != null && r.EntryPoint.Contains(filters.EntryPoint));
            }

            if (!string.IsNullOrEmpty(filters.ExitPoint))
            {
                query = query.Where(r => r.ExitPoint != null && r.ExitPoint.Contains(filters.ExitPoint));
            }

            // Apply sorting based on report type
            switch (filters.ReportType)
            {
                case "ByCompany":
                    query = query.OrderBy(r => r.CompanyName).ThenByDescending(r => r.RequestTime);
                    break;
                case "ByDate":
                    query = query.OrderByDescending(r => r.RequestTime);
                    break;
                case "ByPassengerCount":
                    query = query.OrderByDescending(r => r.CrewCount ?? 0).ThenByDescending(r => r.RequestTime);
                    break;
                case "ByStatus":
                    query = query.OrderBy(r => r.RequestStatus).ThenByDescending(r => r.RequestTime);
                    break;
                case "ByAircraftType":
                    query = query.OrderBy(r => r.AircraftType).ThenByDescending(r => r.RequestTime);
                    break;
                case "ByDepartment":
                    query = query.OrderBy(r => r.Department).ThenByDescending(r => r.RequestTime);
                    break;
                case "ByFlightPurpose":
                    query = query.OrderBy(r => r.FlightPurpose).ThenByDescending(r => r.RequestTime);
                    break;
                default:
                    query = query.OrderByDescending(r => r.RequestTime);
                    break;
            }

            var requests = await query.ToListAsync();
            var summary = await GenerateReportSummary(requests);

            return new AirPortRequestReportResultVM
            {
                Requests = requests,
                Filters = filters,
                Summary = summary
            };
        }

        private async Task<ReportSummaryVM> GenerateReportSummary(List<AirPortRequest> requests)
        {
            var summary = new ReportSummaryVM
            {
                TotalRequests = requests.Count,
                PendingRequests = requests.Count(r => r.RequestStatus == "0"),
                ApprovedRequests = requests.Count(r => r.RequestStatus == "1"),
                RejectedRequests = requests.Count(r => r.RequestStatus == "2"),
                TotalCrewCount = requests.Where(r => r.CrewCount.HasValue).Sum(r => r.CrewCount.Value),
                UniqueCompanies = requests.Select(r => r.CompanyName).Distinct().Count(),
                UniqueAircraftTypes = requests.Select(r => r.AircraftType).Distinct().Count()
            };

            // Group by status
            summary.RequestsByStatus = requests
                .GroupBy(r => r.RequestStatus)
                .ToDictionary(g => g.Key, g => g.Count());

            // Group by company
            summary.RequestsByCompany = requests
                .GroupBy(r => r.CompanyName)
                .ToDictionary(g => g.Key, g => g.Count())
                .OrderByDescending(x => x.Value)
                .Take(10)
                .ToDictionary(x => x.Key, x => x.Value);

            // Group by aircraft type
            summary.RequestsByAircraftType = requests
                .GroupBy(r => r.AircraftType)
                .ToDictionary(g => g.Key, g => g.Count())
                .OrderByDescending(x => x.Value)
                .Take(10)
                .ToDictionary(x => x.Key, x => x.Value);

            // Group by department
            summary.RequestsByDepartment = requests
                .GroupBy(r => r.Department)
                .ToDictionary(g => g.Key, g => g.Count())
                .OrderByDescending(x => x.Value)
                .ToDictionary(x => x.Key, x => x.Value);

            // Group by date
            summary.RequestsByDate = requests
                .GroupBy(r => r.RequestTime.Date)
                .ToDictionary(g => g.Key, g => g.Count())
                .OrderBy(x => x.Key)
                .ToDictionary(x => x.Key, x => x.Value);

            return summary;
        }

        private async Task<IActionResult> ExportToPdf(AirPortRequestReportResultVM result)
        {
            // TODO: Implement PDF export using a library like iTextSharp or similar
            // For now, return a placeholder
            TempData["InfoMessage"] = "PDF export functionality will be implemented soon.";
            return View("ReportResults", result);
        }

        private async Task<IActionResult> ExportToExcel(AirPortRequestReportResultVM result)
        {
            // Create CSV content
            var csv = new System.Text.StringBuilder();
            
            // Add headers
            csv.AppendLine("Request ID,Company Name,Department,Request Date,Flight Date,Aircraft Type,Flight Number,Pilot Name,Crew Count,Status,Email,Sender Name");
            
            // Add data rows
            foreach (var request in result.Requests)
            {
                csv.AppendLine($"{request.Id},{request.CompanyName},{request.Department},{request.RequestTime:yyyy-MM-dd},{request.FlightDate:yyyy-MM-dd},{request.AircraftType},{request.FlightNumber ?? ""},{request.PilotName ?? ""},{request.CrewCount ?? 0},{request.RequestStatus},{request.Email},{request.SenderName}");
            }
            
            var bytes = System.Text.Encoding.UTF8.GetBytes(csv.ToString());
            var fileName = $"AirPortRequests_Report_{DateTime.Now:yyyyMMdd_HHmmss}.csv";
            
            return File(bytes, "text/csv", fileName);
        }

        [HttpGet]
        public async Task<IActionResult> GetReportData(string reportType, DateTime? dateFrom, DateTime? dateTo, string? companyName, string? status)
        {
            var filters = new AirPortRequestReportVM
            {
                ReportType = reportType,
                DateFrom = dateFrom,
                DateTo = dateTo,
                CompanyName = companyName,
                RequestStatus = status
            };

            var result = await GenerateReportData(filters);
            return Json(result);
        }



   
    }

}

