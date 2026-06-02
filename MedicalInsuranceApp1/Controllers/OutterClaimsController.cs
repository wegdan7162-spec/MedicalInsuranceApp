using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MedicalInsuranceApp1.Data;
using MedicalInsuranceApp1.Models.Entities;
using MedicalInsuranceApp1.Models.Identity;
using MedicalInsuranceApp1.Models.ViewModels;

namespace MedicalInsuranceApp1.Controllers
{
    [Authorize]
    public class OutterClaimsController : Controller
    {
        private readonly AppDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;

        public OutterClaimsController(
            AppDbContext db,
            UserManager<ApplicationUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        // ===== قائمة =====
        public async Task<IActionResult> Index(
            string? search, int? branchId,
            string? status, int? year)
        {
            var query = _db.OutterClaims
                .Include(x => x.Branch)
                .Include(x => x.Plaintiff)
                .Include(x => x.Court)
                .AsQueryable();

            if (!string.IsNullOrEmpty(search))
                query = query.Where(x =>
                    (x.Plaintiff != null && x.Plaintiff.PlainitiffName.Contains(search)) ||
                    (x.Place != null && x.Place.Contains(search)));

            if (branchId.HasValue)
                query = query.Where(x => x.BranchId == branchId);

            if (!string.IsNullOrEmpty(status))
                query = query.Where(x => x.ClaimStatus == status);

            if (year.HasValue)
                query = query.Where(x => x.Year == year);

            var items = await query.OrderBy(x => x.Year).ThenBy(x => x.Num).ToListAsync();

            var vm = items.Select(x => new OutterClaimListVM
            {
                Id = x.Id,
                ClaimNumber = $"{x.Year}-{x.Num}",
                Year = x.Year,
                PlaintiffName = x.Plaintiff?.PlainitiffName ?? "-",
                BranchName = x.Branch?.BranchName ?? "-",
                CourtName = x.Court?.CourtName ?? "-",
                Place = x.Place ?? "-",
                Reserve = x.Reserve,
                Setteld = x.Setteld,
                ClaimStatus = x.ClaimStatus ?? "تحت التسوية",
                FileStatus = x.FileStatus ?? "نشط",
                RegDate = x.RegDate,
                IncidentDate = x.IncidentDate
            }).ToList();

            ViewBag.Search = search;
            ViewBag.BranchId = branchId;
            ViewBag.Status = status;
            ViewBag.Year = year;
            ViewBag.Total = vm.Count;
            ViewBag.Branches = await _db.Branches.Where(x => (x.Del == null || x.Del == false)).OrderBy(x => x.BranchName).ToListAsync();
            ViewBag.StatusList = GetStatusList();

            return View(vm);
        }

        // ===== جرد أولى =====
        public async Task<IActionResult> Inventory(string? search, int? branchId, int? year)
        {
            var query = _db.OutterClaims
                .Include(x => x.Branch)
                .Include(x => x.Plaintiff)
                .Include(x => x.Court)
                .AsQueryable();

            if (!string.IsNullOrEmpty(search))
                query = query.Where(x =>
                    (x.Plaintiff != null && x.Plaintiff.PlainitiffName.Contains(search)) ||
                    (x.Place != null && x.Place.Contains(search)));

            if (branchId.HasValue)
                query = query.Where(x => x.BranchId == branchId);

            if (year.HasValue)
                query = query.Where(x => x.Year == year);

            var items = await query.OrderBy(x => x.Year).ThenBy(x => x.Num).ToListAsync();

            var vm = items.Select(x => new OutterClaimListVM
            {
                Id = x.Id,
                ClaimNumber = $"{x.Year}-{x.Num}",
                Year = x.Year,
                PlaintiffName = x.Plaintiff?.PlainitiffName ?? "-",
                BranchName = x.Branch?.BranchName ?? "-",
                CourtName = x.Court?.CourtName ?? "-",
                Place = x.Place ?? "-",
                Reserve = x.Reserve,
                Setteld = x.Setteld,
                ClaimStatus = x.ClaimStatus ?? "-",
                FileStatus = x.FileStatus ?? "-",
                RegDate = x.RegDate,
                IncidentDate = x.IncidentDate
            }).ToList();

            ViewBag.Search = search;
            ViewBag.BranchId = branchId;
            ViewBag.Year = year;
            ViewBag.Branches = await _db.Branches.Where(x => (x.Del == null || x.Del == false)).OrderBy(x => x.BranchName).ToListAsync();
            ViewBag.TotalReserve = vm.Sum(x => x.Reserve);
            ViewBag.TotalSettled = vm.Sum(x => x.Setteld);
            ViewBag.Total = vm.Count;

            return View(vm);
        }

        // ===== تفاصيل =====
        public async Task<IActionResult> Details(int id)
        {
            var claim = await _db.OutterClaims
                .Include(x => x.Branch)
                .Include(x => x.Plaintiff)
                .Include(x => x.Court)
                .Include(x => x.Files)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (claim == null) return NotFound();

            var user = claim.UserId != null
                ? await _userManager.FindByIdAsync(claim.UserId)
                : null;

            var vm = new OutterClaimDetailsVM
            {
                Id = claim.Id,
                Num = claim.Num,
                Year = claim.Year,
                PlaintiffName = claim.Plaintiff?.PlainitiffName ?? "-",
                BranchName = claim.Branch?.BranchName ?? "-",
                CourtName = claim.Court?.CourtName ?? "-",
                Place = claim.Place ?? "-",
                Reserve = claim.Reserve,
                Setteld = claim.Setteld,
                Note = claim.Note,
                ClaimStatus = claim.ClaimStatus ?? "تحت التسوية",
                FileStatus = claim.FileStatus ?? "نشط",
                RegDate = claim.RegDate,
                IncidentDate = claim.IncidentDate,
                CreatedBy = user?.FullName ?? user?.UserName ?? "-",
                Files = claim.Files?.Select(f => new OutterFileVM
                {
                    Id = f.Id,
                    OutterId = f.OutterId,
                    FileName = $"ملف-{f.Id}"
                }).ToList() ?? new()
            };

            return View(vm);
        }

        // ===== إضافة =====
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var vm = await BuildCreateVM();
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(OutterClaimCreateVM model)
        {
            if (!ModelState.IsValid)
                return View(await BuildCreateVM(model));

            // التحقق من التكرار
            var duplicate = await _db.OutterClaims.AnyAsync(x =>
                x.PlaintiffNameId == model.PlaintiffNameId &&
                x.Place == model.Place &&
                x.IncidentDate.HasValue &&
                model.IncidentDate.HasValue &&
                x.IncidentDate.Value.Date == model.IncidentDate.Value.Date);

            if (duplicate)
            {
                ModelState.AddModelError("",
                    "⚠ يوجد مطالبة مسبقة بنفس المدعي والمكان والتاريخ");
                return View(await BuildCreateVM(model));
            }

            var lastNum = await _db.OutterClaims
                .Where(x => x.Year == model.RegDate.Year)
                .MaxAsync(x => (long?)x.Num) ?? 0;

            var user = await _userManager.GetUserAsync(User);

            var claim = new OutterClaim
            {
                Num = lastNum + 1,
                Year = model.RegDate.Year,
                PlaintiffNameId = model.PlaintiffNameId,
                BranchId = model.BranchId,
                CourtId = model.CourtId,
                Place = model.Place,
                IncidentDate = model.IncidentDate,
                RegDate = model.RegDate,
                Reserve = model.Reserve,
                Setteld = model.Setteld,
                Note = model.Note,
                ClaimStatus = model.ClaimStatus,
                FileStatus = model.FileStatus,
                UserId = user?.Id
            };

            _db.OutterClaims.Add(claim);
            await _db.SaveChangesAsync();

            // رفع المرفق
            var file = Request.Form.Files.FirstOrDefault();
            if (file != null && file.Length > 0)
            {
                using var ms = new MemoryStream();
                await file.CopyToAsync(ms);
                _db.OutterFiles.Add(new OutterFile
                {
                    OutterId = claim.Id,
                    FileData = ms.ToArray()
                });
                await _db.SaveChangesAsync();
            }

            TempData["Success"] =
                $"تم تسجيل المطالبة الخارجية رقم {claim.Year}-{claim.Num:D4} بنجاح";
            return RedirectToAction("Index");
        }

        // ===== تعديل =====
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var claim = await _db.OutterClaims.FindAsync(id);
            if (claim == null) return NotFound();

            var baseVm = await BuildCreateVM();
            var vm = new OutterClaimEditVM
            {
                Id = claim.Id,
                PlaintiffNameId = claim.PlaintiffNameId ?? 0,
                BranchId = claim.BranchId ?? 0,
                CourtId = claim.CourtId,
                Place = claim.Place ?? "",
                IncidentDate = claim.IncidentDate,
                RegDate = claim.RegDate,
                Reserve = claim.Reserve,
                Setteld = claim.Setteld,
                Note = claim.Note,
                ClaimStatus = claim.ClaimStatus ?? "تحت التسوية",
                FileStatus = claim.FileStatus ?? "نشط",
                Plaintiffs = baseVm.Plaintiffs,
                Branches = baseVm.Branches,
                Courts = baseVm.Courts,
                StatusList = baseVm.StatusList,
                FileStatusList = baseVm.FileStatusList
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(OutterClaimEditVM model)
        {
            if (!ModelState.IsValid)
                return View(await BuildCreateVM(model));

            var claim = await _db.OutterClaims.FindAsync(model.Id);
            if (claim == null) return NotFound();

            claim.PlaintiffNameId = model.PlaintiffNameId;
            claim.BranchId = model.BranchId;
            claim.CourtId = model.CourtId;
            claim.Place = model.Place;
            claim.IncidentDate = model.IncidentDate;
            claim.RegDate = model.RegDate;
            claim.Reserve = model.Reserve;
            claim.Setteld = model.Setteld;
            claim.Note = model.Note;
            claim.ClaimStatus = model.ClaimStatus;
            claim.FileStatus = model.FileStatus;

            await _db.SaveChangesAsync();

            TempData["Success"] = "تم تحديث المطالبة الخارجية بنجاح";
            return RedirectToAction("Index");
        }

        // ===== تحديث الحالة =====
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(int id, string status)
        {
            var claim = await _db.OutterClaims.FindAsync(id);
            if (claim == null) return NotFound();

            claim.ClaimStatus = status;
            await _db.SaveChangesAsync();

            TempData["Success"] = $"تم تحديث الحالة إلى: {status}";
            return RedirectToAction("Details", new { id });
        }

        // ===== تحميل مرفق =====
        public async Task<IActionResult> DownloadFile(int id)
        {
            var file = await _db.OutterFiles.FindAsync(id);
            if (file == null) return NotFound();
            return File(file.FileData, "application/octet-stream", $"file-{id}.pdf");
        }

        // ===== حذف =====
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, string? reason)
        {
            var claim = await _db.OutterClaims.FirstOrDefaultAsync(x => x.Id == id);
            if (claim == null) return NotFound();

            claim.Del = true;
            claim.DeletedAt = DateTime.Now;
            claim.DeletedBy = User.Identity?.Name;
            claim.DeleteReason = reason;
            await _db.SaveChangesAsync();

            TempData["Success"] = "تم حذف المطالبة بنجاح";
            return RedirectToAction("Index");
        }

        // ===== دوال مساعدة =====
        private async Task<OutterClaimCreateVM> BuildCreateVM(OutterClaimCreateVM? model = null)
        {
            var branches   = await _db.Branches.Where(x => (x.Del == null || x.Del == false)).OrderBy(x => x.BranchName).ToListAsync();
            var courts     = await _db.Courts.Where(x => (x.Del == null || x.Del == false)).OrderBy(x => x.CourtName).ToListAsync();
            var plaintiffs = await _db.PlaintiffNames.Where(x => (x.Del == null || x.Del == false)).OrderBy(x => x.PlainitiffName).ToListAsync();

            var vm = model ?? new OutterClaimCreateVM { RegDate = DateTime.Now };

            vm.Branches = branches.Select(b => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
            {
                Value = b.Id.ToString(), Text = b.BranchName
            }).ToList();

            vm.Courts = courts.Select(c => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
            {
                Value = c.Id.ToString(), Text = c.CourtName
            }).ToList();

            vm.Plaintiffs = plaintiffs.Select(p => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
            {
                Value = p.Id.ToString(), Text = p.PlainitiffName
            }).ToList();

            vm.StatusList    = GetStatusList();
            vm.FileStatusList = new() { "نشط", "موقوف", "مغلق" };

            return vm;
        }

        private List<string> GetStatusList() => new()
        {
            "تحت التسوية",
            "قيد المراجعة",
            "محالة للقانوني",
            "جلسات المحكمة",
            "حكم لصالح الشركة",
            "حكم ضد الشركة",
            "محالة للمالية",
            "مسددة",
            "مغلقة"
        };
    }
}