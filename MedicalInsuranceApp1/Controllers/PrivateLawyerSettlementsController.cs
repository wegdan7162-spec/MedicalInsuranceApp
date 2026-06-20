using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MedicalInsuranceApp1.Data;
using MedicalInsuranceApp1.Models.Entities;
using MedicalInsuranceApp1.Infrastrcture;

namespace MedicalInsuranceApp1.Controllers
{
    [Authorize]
    [RequirePermission(AppModules.PrivateLawyerSettlement)]
    public class PrivateLawyerSettlementsController : Controller
    {
        private readonly AppDbContext _db;
        public PrivateLawyerSettlementsController(AppDbContext db) => _db = db;

        public async Task<IActionResult> Index(string? search, int? branchId, int? year, string? status)
        {
            var query = _db.PrivateLawyerSettlements
                .Include(x => x.Court)
                .Include(x => x.Branch)
                .Where(x => x.Del != true);

            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(x => x.LawyerName.Contains(search) || x.CaseNumber.Contains(search));

            if (branchId.HasValue)
                query = query.Where(x => x.BranchId == branchId);

            if (year.HasValue)
                query = query.Where(x => x.Year == year);

            if (!string.IsNullOrWhiteSpace(status))
                query = query.Where(x => x.Status == status);

            var list = await query.OrderByDescending(x => x.CreatedAt).ToListAsync();

            ViewBag.Branches  = await _db.Branches.Where(x => x.Del != true).OrderBy(x => x.BranchName).ToListAsync();
            ViewBag.Courts    = await _db.Courts.Where(x => x.Del != true).OrderBy(x => x.CourtName).ToListAsync();
            ViewBag.TotalAgreed    = list.Sum(x => x.AgreeAmount);
            ViewBag.TotalPaid      = list.Sum(x => x.PaidAmount);
            ViewBag.TotalRemaining = list.Sum(x => x.AgreeAmount - x.PaidAmount);
            ViewBag.CountPaid      = list.Count(x => x.Status == "مدفوع كاملاً");
            ViewBag.CountPending   = list.Count(x => x.Status == "قيد الدفع" || x.Status == "مدفوع جزئياً");
            ViewBag.Search   = search;
            ViewBag.BranchId = branchId;
            ViewBag.Year     = year;
            ViewBag.Status   = status;

            return View(list);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            ViewBag.Branches = await _db.Branches.Where(x => x.Del != true).OrderBy(x => x.BranchName).ToListAsync();
            ViewBag.Courts   = await _db.Courts.Where(x => x.Del != true).OrderBy(x => x.CourtName).ToListAsync();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PrivateLawyerSettlement model)
        {
            if (string.IsNullOrWhiteSpace(model.LawyerName))
            {
                TempData["Error"] = "اسم المحامي مطلوب";
                ViewBag.Branches = await _db.Branches.Where(x => x.Del != true).OrderBy(x => x.BranchName).ToListAsync();
                ViewBag.Courts   = await _db.Courts.Where(x => x.Del != true).OrderBy(x => x.CourtName).ToListAsync();
                return View(model);
            }

            model.UserId    = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            model.CreatedAt = DateTime.Now;
            model.Del       = false;

            _db.PrivateLawyerSettlements.Add(model);
            await _db.SaveChangesAsync();
            TempData["Success"] = $"تم إضافة تسوية المحامي '{model.LawyerName}' بنجاح";
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var item = await _db.PrivateLawyerSettlements.FindAsync(id);
            if (item == null) return NotFound();

            ViewBag.Branches = await _db.Branches.Where(x => x.Del != true).OrderBy(x => x.BranchName).ToListAsync();
            ViewBag.Courts   = await _db.Courts.Where(x => x.Del != true).OrderBy(x => x.CourtName).ToListAsync();
            return View(item);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, PrivateLawyerSettlement model)
        {
            var item = await _db.PrivateLawyerSettlements.FindAsync(id);
            if (item == null) return NotFound();

            item.CaseNumber      = model.CaseNumber;
            item.Year            = model.Year;
            item.CourtId         = model.CourtId;
            item.BranchId        = model.BranchId;
            item.LawyerName      = model.LawyerName;
            item.LawyerPhone     = model.LawyerPhone;
            item.LawyerIBAN      = model.LawyerIBAN;
            item.AgreeAmount     = model.AgreeAmount;
            item.PaidAmount      = model.PaidAmount;
            item.PaymentMethod   = model.PaymentMethod;
            item.ContractDate    = model.ContractDate;
            item.ContractEndDate = model.ContractEndDate;
            item.Status          = model.Status;
            item.Notes           = model.Notes;

            await _db.SaveChangesAsync();
            TempData["Success"] = "تم تحديث تسوية أتعاب المحاماة بنجاح";
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, string? reason)
        {
            var item = await _db.PrivateLawyerSettlements.FindAsync(id);
            if (item != null)
            {
                item.Del          = true;
                item.DeletedAt    = DateTime.Now;
                item.DeletedBy    = User.Identity?.Name;
                item.DeleteReason = reason;
                await _db.SaveChangesAsync();
            }
            TempData["Success"] = "تم حذف تسوية أتعاب المحاماة بنجاح";
            return RedirectToAction("Index");
        }
    }
}
