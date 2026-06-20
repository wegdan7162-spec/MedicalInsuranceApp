using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MedicalInsuranceApp1.Data;
using MedicalInsuranceApp1.Models.Entities;
using MedicalInsuranceApp1.Models.ViewModels;
using MedicalInsuranceApp1.Infrastrcture;
using MedicalInsuranceApp1.Models.ViewModels.Identity;

namespace MedicalInsuranceApp1.Controllers
{
    [Authorize]
    [RequirePermission(AppModules.FriendlyClaims)]
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
            string? status, int? year, string? claimStatus)
        {
            var query = _db.FriendlyClaims
                .Include(x => x.Branch)
                .Include(x => x.Plaintiff)
                .Where(x => x.Del == null || x.Del == false)
                .AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                long? searchNum = null;
                int?  searchYear = null;
                if (search.Contains('-'))
                {
                    var parts = search.Split('-');
                    if (parts.Length == 2 && int.TryParse(parts[0], out int sy) && long.TryParse(parts[1], out long sn))
                    { searchYear = sy; searchNum = sn; }
                }

                query = query.Where(x =>
                    (x.Plaintiff != null && x.Plaintiff.PlainitiffName.Contains(search)) ||
                    (x.Place != null && x.Place.Contains(search)) ||
                    (x.AgentName != null && x.AgentName.Contains(search)) ||
                    (searchNum.HasValue && searchYear.HasValue && x.Num == searchNum && x.Year == searchYear) ||
                    x.Num.ToString().Contains(search));
            }

            if (branchId.HasValue)
                query = query.Where(x => x.BranchId == branchId);

            if (!string.IsNullOrEmpty(status))
            {
                // خريطة الأشكال القديمة لكل حالة (قيم قديمة من سيستم الديسك توب أو إنجليزية)
                var statusAliases = new Dictionary<string, List<string>>
                {
                    ["تحت التسوية"] = new() { "تحت التسوية", "UnderSettlement", "" },
                    ["مسدد"]       = new() {  "مسدد", "Paid" },
                    ["مغلقة"]       = new() { "مغلقة", "Closed" },
                   ["تسوية ودية"]  = new() { "تسوية ودية", "FriendlySettlement" },
                    ["قيد المراجعة"] = new() { "قيد المراجعة", "UnderReview" },
                    ["محالة للقانوني"] = new() { "محالة للقانوني", "Legal" },
                };

                if (status == "تحت التسوية")
                    query = query.Where(x => x.FileStatus == null || x.FileStatus == "" || x.FileStatus == "UnderSettlement" || x.FileStatus == "تحت التسوية");
                else if (status == "مسدد")
                    query = query.Where(x =>  x.FileStatus == "مسدد" || x.FileStatus == "Paid");
                else if (status == "مغلقة")
                    query = query.Where(x => x.FileStatus == "مغلقة" || x.FileStatus == "مغلق" || x.FileStatus == "Closed");
                else if (status == "تسوية ودية")
                    query = query.Where(x => x.FileStatus == "تسوية ودية" || x.FileStatus == "FriendlySettlement");
                else if (status == "قيد المراجعة")
                    query = query.Where(x => x.FileStatus == "قيد المراجعة" || x.FileStatus == "UnderReview");
                else if (status == "محالة للقانوني")
                    query = query.Where(x => x.FileStatus == "محالة للقانوني" || x.FileStatus == "Legal");
                else
                    query = query.Where(x => x.FileStatus == status);
            }

            if (year.HasValue)
                query = query.Where(x => x.Year == year);

            if (!string.IsNullOrEmpty(claimStatus))
            {
                if (claimStatus == "م.مغطاء")
                    query = query.Where(x => x.ClaimStatus == "م مغطاء"
                                          || x.ClaimStatus == "م.مغطاء"
                                          || x.ClaimStatus == " م.مغطاء"
                                          || x.ClaimStatus == "مغطى"
                                          || x.ClaimStatus == null
                                          || x.ClaimStatus == "");
                else if (claimStatus == "غير مغطاء")
                    query = query.Where(x => x.ClaimStatus == "غيرمغطاء"
                                          || x.ClaimStatus == "غير مغطاء"
                                          || x.ClaimStatus == "غير مغطى"
                                          || x.ClaimStatus == "غيرمغطى");
                else
                    query = query.Where(x => x.ClaimStatus == claimStatus);
            }

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
                ClaimStatus = string.IsNullOrEmpty(x.ClaimStatus) ? "م.مغطاء" : x.ClaimStatus,
                FileStatus = string.IsNullOrEmpty(x.FileStatus) ? "تحت التسوية" : x.FileStatus,
                IncidentDate = x.IncidentDate,
                RegDate = x.RegDate
            }).ToList();

            ViewBag.Search = search;
            ViewBag.BranchId = branchId;
            ViewBag.Status = status;
            ViewBag.ClaimStatus = claimStatus;
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
                ClaimStatus = claim.ClaimStatus ?? "م.مغطاء",
                FileStatus = claim.FileStatus ?? "تحت التسوية",
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

            // التحقق من تكرار رقم المطالبة في نفس السنة
            var numExists = await _db.FriendlyClaims.AnyAsync(x =>
                x.Num == model.Num && x.Year == model.ClaimYear);
            if (numExists)
            {
                ModelState.AddModelError("Num",
                    $"⚠ رقم المطالبة {model.Num}/{model.ClaimYear} مستخدم مسبقاً");
                return View(await BuildCreateVM(model));
            }

            var user = await _userManager.GetUserAsync(User);

            var claim = new FriendlyClaim
            {
                Num = model.Num,
                Year = model.ClaimYear,
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

            try
            {
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

                TempData["Success"] = $"✔ تم تسجيل مطالبة الصلح رقم {claim.Year}-{claim.Num} بنجاح";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"✖ فشل الحفظ: {ex.InnerException?.Message ?? ex.Message}";
                return View(await BuildCreateVM(model));
            }
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
                Num = claim.Num,
                ClaimYear = claim.Year,
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
                ClaimStatus = claim.ClaimStatus ?? "م.مغطاء",
                FileStatus = claim.FileStatus ?? "تحت التسوية",
                CourtId = claim.CourtId,
                Plaintiffs = baseVm.Plaintiffs,
                Branches = baseVm.Branches,
                Courts = baseVm.Courts,
                StatusList = baseVm.StatusList,
                ClaimStatusList = baseVm.ClaimStatusList,
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

            try
            {
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

                TempData["Success"] = "✔ تم تحديث مطالبة الصلح بنجاح";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"✖ فشل التحديث: {ex.InnerException?.Message ?? ex.Message}";
                return View(await BuildCreateVM(model));
            }
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

            try
            {
                claim.Del = true;
                claim.DeletedAt = DateTime.Now;
                claim.DeletedBy = User.Identity?.Name;
                claim.DeleteReason = reason;
                await _db.SaveChangesAsync();
                TempData["Success"] = "✔ تم حذف المطالبة بنجاح";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"✖ فشل الحذف: {ex.InnerException?.Message ?? ex.Message}";
            }
            return RedirectToAction("Index");
        }

        // ===== دوال مساعدة =====
        private async Task<FriendlyClaimCreateVM> BuildCreateVM(FriendlyClaimCreateVM? model = null)
        {
            var plaintiffs = await _db.PlaintiffNames.Where(x => x.Del == null || x.Del == false).OrderBy(x => x.PlainitiffName).ToListAsync();
            var branches   = await _db.Branches.Where(x => x.Del == null || x.Del == false).OrderBy(x => x.BranchName).ToListAsync();
            var courts     = await _db.Courts.Where(x => x.Del == null || x.Del == false).OrderBy(x => x.CourtName).ToListAsync();

            var vm = model ?? new FriendlyClaimCreateVM();

            vm.Plaintiffs = plaintiffs.Select(p => new SelectListItem { Value = p.Id.ToString(), Text = p.PlainitiffName }).ToList();
            vm.Branches   = branches.Select(b => new SelectListItem { Value = b.Id.ToString(), Text = b.BranchName }).ToList();
            vm.Courts     = courts.Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.CourtName }).ToList();

            vm.StatusList     = GetStatusList();
            vm.ClaimStatusList = new() { "غير مغطاء", "م.مغطاء" };

            return vm;
        }

        private List<string> GetStatusList() => new()
        {
            "تحت التسوية",
            "قيد المراجعة",
            "محالة للقانوني",
            "تسوية ودية",
            "مسدد",
            "مغلقة"
        };
    }
}
