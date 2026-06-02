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
    public class FriendlyClaimsController : Controller
    {
        private readonly AppDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;

        public FriendlyClaimsController(
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
            var query = _db.FriendlyClaims
                .Include(x => x.Branch)
                .Include(x => x.Plaintiff)
                .AsQueryable();

            if (!string.IsNullOrEmpty(search))
                query = query.Where(x =>
                    x.Plaintiff != null &&
                    x.Plaintiff.PlainitiffName.Contains(search) ||
                    x.Place.Contains(search));

            if (branchId.HasValue)
                query = query.Where(x => x.BranchId == branchId);

            if (!string.IsNullOrEmpty(status))
                query = query.Where(x => x.ClaimStatus == status);

            if (year.HasValue)
                query = query.Where(x => x.Year == year);

            var items = await query.OrderBy(x => x.Year).ThenBy(x => x.Num).ToListAsync();

            var vm = items.Select(x => new FriendlyClaimListVM
            {
                Id = x.Id,
                ClaimNumber = $"{x.Year}-{x.Num}",
                Year = x.Year,
                PlaintiffName = x.Plaintiff?.PlainitiffName ?? "-",
                BranchName = x.Branch?.BranchName ?? "-",
                Place = x.Place ?? "-",
                AgentName = x.AgentName ?? "-",
                Reserve = x.Reserve,
                Setteld = x.Setteld,
                ClaimStatus = x.ClaimStatus ?? "تحت التسوية",
                FileStatus = x.FileStatus ?? "نشط",
                IncidentDate = x.IncidentDate,
                RegDate = x.RegDate
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

        // ===== تفاصيل =====
        public async Task<IActionResult> Details(int id)
        {
            var claim = await _db.FriendlyClaims
                .Include(x => x.Branch)
                .Include(x => x.Plaintiff)
                .Include(x => x.Files)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (claim == null) return NotFound();

            var user = claim.UserId != null
                ? await _userManager.FindByIdAsync(claim.UserId)
                : null;
            var court = claim.CourtId.HasValue
                ? await _db.Courts.FindAsync(claim.CourtId.Value)
                : null;

            var vm = new FriendlyClaimDetailsVM
            {
                Id = claim.Id,
                Num = claim.Num,
                Year = claim.Year,
                PlaintiffName = claim.Plaintiff?.PlainitiffName ?? "-",
                BranchName = claim.Branch?.BranchName ?? "-",
                Place = claim.Place ?? "-",
                Subject = claim.Subject,
                AgentName = claim.AgentName,
                CourtName = court?.CourtName,
                Reserve = claim.Reserve,
                Setteld = claim.Setteld,
                Note = claim.Note,
                ClaimStatus = claim.ClaimStatus ?? "تحت التسوية",
                FileStatus = claim.FileStatus ?? "نشط",
                IncidentDate = claim.IncidentDate,
                RegDate = claim.RegDate,
                CreatedBy = user?.FullName ?? user?.UserName ?? "-",
                Files = claim.Files?.Select(f => new FriendlyFileVM
                {
                    Id = f.Id,
                    FriendlyId = f.FriendlyId,
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
        public async Task<IActionResult> Create(FriendlyClaimCreateVM model)
        {
            if (!ModelState.IsValid)
                return View(await BuildCreateVM(model));

            // التحقق من التكرار
            var duplicate = await _db.FriendlyClaims.AnyAsync(x =>
                x.PlaintiffNameId == model.PlaintiffNameId &&
                x.Place == model.Place &&
                x.IncidentDate.Date == model.IncidentDate.Date);

            if (duplicate)
            {
                ModelState.AddModelError("",
                    "⚠ يوجد مطالبة مسبقة بنفس المدعي والمكان والتاريخ");
                return View(await BuildCreateVM(model));
            }

            var lastNum = await _db.FriendlyClaims
                .Where(x => x.Year == model.RegDate.Year)
                .MaxAsync(x => (long?)x.Num) ?? 0;

            var user = await _userManager.GetUserAsync(User);

            var claim = new FriendlyClaim
            {
                Num = lastNum + 1,
                Year = model.RegDate.Year,
                PlaintiffNameId = model.PlaintiffNameId,
                BranchId = model.BranchId,
                Place = model.Place,
                IncidentDate = model.IncidentDate,
                RegDate = model.RegDate,
                Subject = model.Subject,
                AgentName = model.AgentName,
                Reserve = model.Reserve,
                Setteld = model.Setteld,
                Note = model.Note,
                ClaimStatus = model.ClaimStatus,
                FileStatus = model.FileStatus,
                CourtId = model.CourtId,
                UserId = user?.Id
            };

            _db.FriendlyClaims.Add(claim);
            await _db.SaveChangesAsync();

            // رفع المرفق
            var file = Request.Form.Files.FirstOrDefault();
            if (file != null && file.Length > 0)
            {
                using var ms = new MemoryStream();
                await file.CopyToAsync(ms);
                _db.FriendlyFiles.Add(new FriendlyFile
                {
                    FriendlyId = claim.Id,
                    FileData = ms.ToArray()
                });
                await _db.SaveChangesAsync();
            }

            TempData["Success"] =
                $"تم تسجيل مطالبة الصلح رقم {claim.Year}-{claim.Num:D4} بنجاح";
            return RedirectToAction("Index");
        }

        // ===== تعديل =====
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var claim = await _db.FriendlyClaims.FindAsync(id);
            if (claim == null) return NotFound();

            var baseVm = await BuildCreateVM();
            var vm = new FriendlyClaimEditVM
            {
                Id = claim.Id,
                PlaintiffNameId = claim.PlaintiffNameId ?? 0,
                BranchId = claim.BranchId ?? 0,
                Place = claim.Place ?? "",
                IncidentDate = claim.IncidentDate,
                RegDate = claim.RegDate,
                Subject = claim.Subject,
                AgentName = claim.AgentName,
                Reserve = claim.Reserve,
                Setteld = claim.Setteld,
                Note = claim.Note,
                ClaimStatus = claim.ClaimStatus ?? "تحت التسوية",
                FileStatus = claim.FileStatus ?? "نشط",
                CourtId = claim.CourtId,
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
        public async Task<IActionResult> Edit(FriendlyClaimEditVM model)
        {
            if (!ModelState.IsValid)
                return View(await BuildCreateVM(model));

            var claim = await _db.FriendlyClaims.FindAsync(model.Id);
            if (claim == null) return NotFound();

            claim.PlaintiffNameId = model.PlaintiffNameId;
            claim.BranchId = model.BranchId;
            claim.Place = model.Place;
            claim.IncidentDate = model.IncidentDate;
            claim.RegDate = model.RegDate;
            claim.Subject = model.Subject;
            claim.AgentName = model.AgentName;
            claim.Reserve = model.Reserve;
            claim.Setteld = model.Setteld;
            claim.Note = model.Note;
            claim.ClaimStatus = model.ClaimStatus;
            claim.FileStatus = model.FileStatus;
            claim.CourtId = model.CourtId;

            await _db.SaveChangesAsync();

            TempData["Success"] = "تم تحديث مطالبة الصلح بنجاح";
            return RedirectToAction("Index");
        }

        // ===== تحديث الحالة =====
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(int id, string status)
        {
            var claim = await _db.FriendlyClaims.FindAsync(id);
            if (claim == null) return NotFound();

            claim.ClaimStatus = status;
            await _db.SaveChangesAsync();

            TempData["Success"] = $"تم تحديث الحالة إلى: {status}";
            return RedirectToAction("Details", new { id });
        }

        // ===== تحميل مرفق =====
        public async Task<IActionResult> DownloadFile(int id)
        {
            var file = await _db.FriendlyFiles.FindAsync(id);
            if (file == null) return NotFound();
            return File(file.FileData, "application/octet-stream", $"file-{id}.pdf");
        }

        // ===== حذف =====
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, string? reason)
        {
            var claim = await _db.FriendlyClaims.FirstOrDefaultAsync(x => x.Id == id);
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
        private async Task<FriendlyClaimCreateVM> BuildCreateVM(
            FriendlyClaimCreateVM? model = null)
        {
            var plaintiffs = await _db.PlaintiffNames.Where(x => (x.Del == null || x.Del == false)).OrderBy(x => x.PlainitiffName).ToListAsync();
            var branches   = await _db.Branches.Where(x => (x.Del == null || x.Del == false)).OrderBy(x => x.BranchName).ToListAsync();
            var courts     = await _db.Courts.Where(x => (x.Del == null || x.Del == false)).OrderBy(x => x.CourtName).ToListAsync();

            var vm = model ?? new FriendlyClaimCreateVM();

            vm.Plaintiffs = plaintiffs.Select(p => new SelectListItem
            {
                Value = p.Id.ToString(),
                Text = p.PlainitiffName
            }).ToList();

            vm.Branches = branches.Select(b => new SelectListItem
            {
                Value = b.Id.ToString(),
                Text = b.BranchName
            }).ToList();

            vm.Courts = courts.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.CourtName
            }).ToList();

            vm.StatusList = GetStatusList();
            vm.FileStatusList = new() { "نشط", "موقوف", "مغلق" };

            return vm;
        }

        private List<string> GetStatusList() => new()
        {
            "تحت التسوية",
            "قيد المراجعة",
            "محالة للقانوني",
            "تسوية ودية",
            "مغلقة"
        };
    }
}