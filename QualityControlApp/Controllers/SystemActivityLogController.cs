// Controllers/SystemActivityLogsController.cs
using Microsoft.AspNetCore.Authorization; // لـ [Authorize]
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QualityControlApp.Models.Entities; // مسار Entity
using QualityControlApp.Classes; // مسار Entity
using QualityControlApp.ViewModels; // مسار ViewModel
using QualityControlApp.Models; // مسار ViewModel
using System.Linq;
using System.Threading.Tasks;

namespace QualityControlApp.Controllers
{
    // [Authorize(Roles = "Admin")] // يجب تأمين هذا الكنترولر للمسؤولين فقط
    public class SystemActivityLogsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SystemActivityLogsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: SystemActivityLogs
        public async Task<IActionResult> Index(string searchTerm, string activityType, DateTime? startDate, DateTime? endDate, int pageNumber = 1)
        {
            ViewData["CurrentFilter"] = searchTerm;
            ViewData["CurrentActivityType"] = activityType;
            ViewData["CurrentStartDate"] = startDate?.ToString("yyyy-MM-dd");
            ViewData["CurrentEndDate"] = endDate?.ToString("yyyy-MM-dd");

            var query = _context.SystemActivityLogs.AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(s => s.Description.Contains(searchTerm) ||
                                         (s.UserName != null && s.UserName.Contains(searchTerm)) ||
                                         (s.RelatedEntityDescription != null && s.RelatedEntityDescription.Contains(searchTerm)));
            }

            if (!string.IsNullOrEmpty(activityType))
            {
                query = query.Where(s => s.ActivityType == activityType);
            }

            if (startDate.HasValue)
            {
                query = query.Where(s => s.ActivityDate >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                // Add 1 day to endDate to include all activities on that day
                query = query.Where(s => s.ActivityDate < endDate.Value.AddDays(1));
            }

            // جلب قائمة بأنواع الأنشطة الموجودة للفلترة (اختياري)
            ViewBag.ActivityTypes = await _context.SystemActivityLogs
                                        .Select(s => s.ActivityType)
                                        .Where(at => at != null) // تجاهل القيم الفارغة
                                        .Distinct()
                                        .OrderBy(at => at)
                                        .ToListAsync();


            int pageSize = 15; // عدد العناصر في كل صفحة
            var pagedResult = await PaginatedList<SystemActivityLog>.CreateAsync(query.OrderByDescending(s => s.ActivityDate), pageNumber, pageSize);

            return View(pagedResult);
        }

        // GET: SystemActivityLogs/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var systemActivityLog = await _context.SystemActivityLogs
                .FirstOrDefaultAsync(m => m.Id == id);
            if (systemActivityLog == null)
            {
                return NotFound();
            }

            return View(systemActivityLog);
        }

        // GET: SystemActivityLogs/Create
        // هذا قد لا يكون مطلوبًا لأن الإنشاء يتم برمجياً، لكنه موجود للإكمال.
        public IActionResult Create()
        {
            var vm = new SystemActivityLogVM();
            return View(vm);
        }

        // POST: SystemActivityLogs/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SystemActivityLogVM vm)
        {
            if (ModelState.IsValid)
            {
                var systemActivityLog = new SystemActivityLog
                {
                    ActivityDate = vm.ActivityDate,
                    Description = vm.Description,
                    ActivityType = vm.ActivityType,
                    RelatedEntityType = vm.RelatedEntityType,
                    RelatedEntityId = vm.RelatedEntityId,
                    RelatedEntityDescription = vm.RelatedEntityDescription,
                    UserId = vm.UserId,
                    UserName = vm.UserName
                    // IsRead و TargetUserGroupId إذا أضفتها للـ VM
                };
                _context.Add(systemActivityLog);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Activity log created successfully.";
                return RedirectToAction(nameof(Index));
            }
            return View(vm);
        }

        // GET: SystemActivityLogs/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var systemActivityLog = await _context.SystemActivityLogs.FindAsync(id);
            if (systemActivityLog == null)
            {
                return NotFound();
            }

            var vm = new SystemActivityLogVM
            {
                Id = systemActivityLog.Id,
                ActivityDate = systemActivityLog.ActivityDate,
                Description = systemActivityLog.Description,
                ActivityType = systemActivityLog.ActivityType,
                RelatedEntityType = systemActivityLog.RelatedEntityType,
                RelatedEntityId = systemActivityLog.RelatedEntityId,
                RelatedEntityDescription = systemActivityLog.RelatedEntityDescription,
                UserId = systemActivityLog.UserId,
                UserName = systemActivityLog.UserName
            };
            return View(vm);
        }

        // POST: SystemActivityLogs/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, SystemActivityLogVM vm)
        {
            if (id != vm.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var systemActivityLog = await _context.SystemActivityLogs.FindAsync(id);
                    if (systemActivityLog == null)
                    {
                        return NotFound();
                    }

                    systemActivityLog.ActivityDate = vm.ActivityDate;
                    systemActivityLog.Description = vm.Description;
                    systemActivityLog.ActivityType = vm.ActivityType;
                    systemActivityLog.RelatedEntityType = vm.RelatedEntityType;
                    systemActivityLog.RelatedEntityId = vm.RelatedEntityId;
                    systemActivityLog.RelatedEntityDescription = vm.RelatedEntityDescription;
                    systemActivityLog.UserId = vm.UserId;
                    systemActivityLog.UserName = vm.UserName;

                    _context.Update(systemActivityLog);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Activity log updated successfully.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SystemActivityLogExists(vm.Id))
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
            return View(vm);
        }

        // GET: SystemActivityLogs/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var systemActivityLog = await _context.SystemActivityLogs
                .FirstOrDefaultAsync(m => m.Id == id);
            if (systemActivityLog == null)
            {
                return NotFound();
            }

            return View(systemActivityLog);
        }

        // POST: SystemActivityLogs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var systemActivityLog = await _context.SystemActivityLogs.FindAsync(id);
            if (systemActivityLog != null)
            {
                _context.SystemActivityLogs.Remove(systemActivityLog);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Activity log deleted successfully.";
            }
            return RedirectToAction(nameof(Index));
        }

        private bool SystemActivityLogExists(Guid id)
        {
            return _context.SystemActivityLogs.Any(e => e.Id == id);
        }
    }
}