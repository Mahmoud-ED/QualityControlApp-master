using Microsoft.AspNetCore.Mvc;
using QualityControlApp.Classes;
using QualityControlApp.Models.Entities;
using QualityControlApp.Models.Interfaces;
using QualityControlApp.Models;
using QualityControlApp.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Runtime.InteropServices;

namespace QualityControlApp.Controllers
{
    [ViewLayout("_LayoutDashboard")]

    public class BookingAppointmentController : BaseController
    {

        private readonly IViewHelper _viewHelper;
        private readonly ApplicationDbContext _context;
        private readonly IUnitOfWork<Company> _company;
        private readonly IUnitOfWork<Contact> _contact;
        private readonly IUnitOfWork<SiteInfo> _siteInfo;
        private readonly IUnitOfWork<Employee> _employee;
        private readonly IUnitOfWork<ApplicationUser> _applicationUser;
        private readonly IUnitOfWork<BookingAppointment > _bookingappointment;
        private readonly IEmailSender _emailSender;
        private readonly IWebHostEnvironment _host;
        private readonly IServiceProvider _serviceProvider;

        public BookingAppointmentController(
            ApplicationDbContext context,
            IEmailSender emailSender,
            IUnitOfWork<Company> company,
            IUnitOfWork<SiteInfo> siteInfo,
            IUnitOfWork<Contact> contact,
            IUnitOfWork<BookingAppointment> bookingappointment,
            IUnitOfWork<Employee> employee,
            IUnitOfWork<ApplicationUser> applicationUser,
            IWebHostEnvironment host,
            IServiceProvider serviceProvider,
            IViewHelper viewHelper
            ) : base(host)
        {
            _context = context;
            _siteInfo = siteInfo;
            _company = company;
            _bookingappointment = bookingappointment;
            _contact = contact;
            _emailSender = emailSender;
            _applicationUser = applicationUser;
            _host = host;
            _serviceProvider = serviceProvider;
            _viewHelper = viewHelper;

        }

        public async Task<IActionResult> Index()
        {


            var Company = await _company.Entity.GetAll().ToListAsync();
            var appointments = await _bookingappointment.Entity
                                        .Include(b => b.Company) // Include company data
                                        .OrderByDescending(b => b.BookingDate) // Default sort
                                        .ToListAsync();

        var viewModel = new BookingAppointmentIndexVM
        {
            Appointments = appointments,
            CompanySelectList = new SelectList(Company, "Id", "Name") // Create SelectList for filter
        };

     

            return View(viewModel);



        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var companies = await _company.Entity.GetAll().ToListAsync();

            var viewModel = new BookingAppointmentVM
            {
                BookingDate = DateTime.Today,
                CompanySelectList = new SelectList(companies, "Id", "Name")

            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BookingAppointmentVM model)
        {
            if (ModelState.IsValid)
            {
                // إنشاء كائن جديد من BookingAppointment
                var bookingAppointment = new BookingAppointment
                {
                    CompanyId = model.CompanyId,
                    BookingDate = model.BookingDate,
                    Notes = model.Notes
                };

                // حفظ الكائن في قاعدة البيانات
                _bookingappointment.Entity.Insert(bookingAppointment);
                await _bookingappointment.SaveAsync();

                // إعادة التوجيه إلى قائمة الحجوزات أو أي صفحة أخرى بعد الحفظ
                return RedirectToAction("Index"); // أو أي اسم آخر للإجراء الذي يعرض قائمة الحجوزات
            }

            // إذا كان الـ ModelState غير صالح، نعيد تحميل الـ SelectList للشركات في حالة حدوث خطأ
            var companies = await _company.Entity.GetAll().ToListAsync();
            model.CompanySelectList = new SelectList(companies, "Id", "Name");

            // إعادة عرض النموذج مع رسائل الخطأ
            return View(model);
        }


        [HttpGet]
        public async Task<IActionResult> GetCalendarEvents(DateTime? start, DateTime? end)
        {

            if (!start.HasValue || !end.HasValue)
            {
                // يمكنك تحديد نطاق افتراضي أو إرجاع خطأ
                start = DateTime.Today.AddMonths(-1);
                end = DateTime.Today.AddMonths(2);
            }

            var events = await _bookingappointment.Entity
                                .Include(b => b.Company) // تحميل اسم الشركة
                                .Select(b => new // تحويل إلى تنسيق FullCalendar Event Object
                                {
                                    id = b.Id, // معرف الحجز (اختياري لكن مفيد)
                                    title = b.Company != null ? b.Company.Name : "شركة غير محددة", // اسم الشركة كعنوان للحدث
                                    start = b.BookingDate.ToString("yyyy-MM-dd"), // تاريخ البدء (تنسيق ISO 8601)
                                                                                  // يمكنك إضافة خصائص أخرى هنا
                                                                                  // backgroundColor = GetCompanyColor(b.CompanyId), // لتلوين الحدث بناءً على الشركة
                                                                                  // borderColor = GetCompanyColor(b.CompanyId)
                                    extendedProps = new
                                    { // لحمل بيانات إضافية
                                        notes = b.Notes,
                                        companyId = b.CompanyId
                                    }
                                })
                                .ToListAsync();

            return Json(events); // إرجاع البيانات كـ JSON
        }



        // GET: BookingAppointment/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound(); // Or BadRequest()
            }

            // Fetch the existing booking appointment from the database
            var bookingAppointment = await _bookingappointment.Entity.GetByIdAsync(id); // Assuming GetByIdAsync exists

            // Alternative using FirstOrDefaultAsync if GetByIdAsync is not available
            // var bookingAppointment = await _bookingappointment.Entity.GetAll()
            //                                   .FirstOrDefaultAsync(b => b.Id == id.Value);

            if (bookingAppointment == null)
            {
                return NotFound(); // Appointment with this ID doesn't exist
            }

            // Map the domain model to the ViewModel
            var model = new BookingAppointmentVM
            {
                Id = bookingAppointment.Id,
                CompanyId = bookingAppointment.CompanyId,
                BookingDate = bookingAppointment.BookingDate,
                Notes = bookingAppointment.Notes
                // Do NOT map CompanySelectList here yet
            };

            // Populate the Company SelectList and set the selected value
            await PopulateCompanySelectList(model, bookingAppointment.CompanyId);

            return View(model); // Pass the ViewModel to the Edit view
        }

        // POST: BookingAppointment/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, BookingAppointmentVM model)
        {
            // Ensure the ID from the route matches the ID in the submitted model
            if (id != model.Id)
            {
                return NotFound(); // Or BadRequest("ID mismatch")
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Fetch the original entity from the database *again* to update it
                    // Avoids updating fields not present in the VM if you didn't fetch first.
                    var bookingAppointmentToUpdate = await _bookingappointment.Entity.GetByIdAsync(id);

                    if (bookingAppointmentToUpdate == null)
                    {
                        return NotFound(); // Could have been deleted between GET and POST
                    }

                    // Update the properties of the fetched entity from the ViewModel
                    bookingAppointmentToUpdate.CompanyId = model.CompanyId;
                    bookingAppointmentToUpdate.BookingDate = model.BookingDate;
                    bookingAppointmentToUpdate.Notes = model.Notes;
                    // Do not update the Id or any other properties not meant to be edited here

                    // Mark the entity as modified and save changes
                    _bookingappointment.Entity.Update(bookingAppointmentToUpdate); // Tell the repository/context to track the update
                    await _bookingappointment.SaveAsync(); // Commit changes to the database

                    // Redirect back to the index page (or details page) after successful update
                    // Add TempData message for user feedback (optional)
                    TempData["SuccessMessage"] = "تم تعديل الحجز بنجاح.";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    // Handle concurrency conflict (optional but good practice)
                    // Check if the entity still exists
                    if (!await BookingAppointmentExists(model.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "تم تعديل هذا السجل بواسطة مستخدم آخر. تم إلغاء التغييرات الخاصة بك. الرجاء المحاولة مرة أخرى.");
                        // Optionally reload data or provide specific instructions
                    }
                }
                catch (Exception ex) // Catch other potential exceptions during save
                {
                    ModelState.AddModelError(string.Empty, "حدث خطأ غير متوقع أثناء محاولة حفظ التعديلات.");
                }
            }

            // If ModelState is invalid OR an error occurred during save, repopulate the SelectList
            await PopulateCompanySelectList(model, model.CompanyId);
            return View(model); // Return the view with the model (including validation errors)
        }

        // Helper method to populate the Company SelectList
        private async Task PopulateCompanySelectList(BookingAppointmentVM model, Guid? selectedCompanyId = null)
        {
            var companies = await _company.Entity.GetAll()
                                        .OrderBy(c => c.Name) // Optional: Order companies by name
                                        .ToListAsync();
            model.CompanySelectList = new SelectList(companies, "Id", "Name", selectedCompanyId);
        }

        // Helper method for concurrency check (optional)
        private async Task<bool> BookingAppointmentExists(Guid id)
        {
            // Adjust based on your repository/context implementation
            return await _bookingappointment.Entity.GetAll().AnyAsync(e => e.Id == id);
        }



        // GET: BookingAppointment/FilterAppointments?companyId=1&bookingDate=2023-10-27
        [HttpGet]
        public async Task<IActionResult> FilterAppointments(Guid? companyId, string? bookingDate)
        {
            try
            {
                // --- Use 'var' here ---
                // The compiler will initially infer this as IIncludableQueryable<...>
                IQueryable<BookingAppointment> query = _bookingappointment.Entity.GetAll()
                                             .Include(b => b.Company);

                // --- Apply Filters Conditionally ---
                if (companyId.HasValue)
                {
                    query = query.Where(b => b.CompanyId == companyId.Value);
                }

                if (!string.IsNullOrEmpty(bookingDate) && DateTime.TryParse(bookingDate, out DateTime parsedDate))
                {
                    query = query.Where(b => b.BookingDate.Date == parsedDate.Date);
                }

                // --- Order the results ---
                // The result of OrderBy is typically IOrderedQueryable<BookingAppointment>
                // 'var' handles this too.
                query = query.OrderByDescending(b => b.BookingDate).ThenBy(b => b.Company.Name);

                // Execute the query asynchronously and get the results
                // ToListAsync operates on the final IQueryable or IOrderedQueryable
                var filteredAppointments = await query.ToListAsync();

                // Prepare the ViewModel
                var viewModel = new BookingAppointmentIndexVM
                {
                    Appointments = filteredAppointments
                };

                // Return the partial view
                return PartialView("_BookingAppointmentsTable", viewModel);
            }
            catch (Exception ex)
            {
                return Content("<div class='alert alert-danger text-center mt-3'>حدث خطأ أثناء تحميل البيانات. يرجى المحاولة مرة أخرى.</div>");
            }
        }
    }














    }






