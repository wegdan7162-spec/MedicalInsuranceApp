using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MedicalInsuranceApp1.Data;
using MedicalInsuranceApp1.Models.Entities;
using MedicalInsuranceApp1.Infrastrcture;

namespace MedicalInsuranceApp1.Controllers
{
    [Authorize]
    [RequirePermission(AppModules.ReservationSettlement)]
    public class ReservationSettlementsController : Controller
    {
        private readonly AppDbContext _db;
        public ReservationSettlementsController(AppDbContext db) => _db = db;

        public async Task<IActionResult> Index(string? search, int? branchId, int? year, string? status)
        {
            var query = _db.ReservationSettlements
                .Include(x => x.Plaintiff)
                .Include(x => x.Branch)
                .Where(x => x.Del != true);

            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(x => x.Plaintiff!.PlainitiffName.Contains(search) || x.ClaimNum.ToString().Contains(search));

            if (branchId.HasValue)
                query = query.Where(x => x.BranchId == branchId);

            if (year.HasValue)
                query = query.Where(x => x.Year == year);

            if (!string.IsNullOrWhiteSpace(status))
                query = query.Where(x => x.Status == status);

            var list = await query.OrderByDescending(x => x.CreatedAt).ToListAsync();

            ViewBag.Branches = await _db.Branches.Where(x => x.Del != true).OrderBy(x => x.BranchName).ToListAsync();
            ViewBag.TotalReserve  = list.Sum(x => x.ReserveAmount);
            ViewBag.TotalSettled  = list.Sum(x => x.SettledAmount);
            ViewBag.TotalRemaining = list.Sum(x => x.ReserveAmount - x.SettledAmount);
            ViewBag.Search   = search;
            ViewBag.BranchId = branchId;
            ViewBag.Year     = year;
            ViewBag.Status   = status;

            return View(list);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            ViewBag.Branches   = await _db.Branches.Where(x => x.Del != true).OrderBy(x => x.BranchName).ToListAsync();
            ViewBag.Plaintiffs = await _db.PlaintiffNames.OrderBy(x => x.PlainitiffName).ToListAsync();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ReservationSettlement model)
        {
            model.UserId    = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            model.CreatedAt = DateTime.Now;
            model.Del       = false;

            _db.ReservationSettlements.Add(model);
            await _db.SaveChangesAsync();
            TempData["Success"] = "تم إضافة تسوية الحجز بنجاح";
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var item = await _db.ReservationSettlements.FindAsync(id);
            if (item == null) return NotFound();

            ViewBag.Branches   = await _db.Branches.Where(x => x.Del != true).OrderBy(x => x.BranchName).ToListAsync();
            ViewBag.Plaintiffs = await _db.PlaintiffNames.OrderBy(x => x.PlainitiffName).ToListAsync();
            return View(item);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ReservationSettlement model)
        {
            var item = await _db.ReservationSettlements.FindAsync(id);
            if (item == null) return NotFound();

            item.ClaimNum         = model.ClaimNum;
            item.Year             = model.Year;
            item.PlaintiffNameId  = model.PlaintiffNameId;
            item.BranchId         = model.BranchId;
            item.ReserveAmount    = model.ReserveAmount;
            item.SettledAmount    = model.SettledAmount;
            item.ReservationDate  = model.ReservationDate;
            item.Status           = model.Status;
            item.Notes            = model.Notes;

            await _db.SaveChangesAsync();
            TempData["Success"] = "تم تحديث تسوية الحجز بنجاح";
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, string? reason)
        {
            var item = await _db.ReservationSettlements.FindAsync(id);
            if (item != null)
            {
                item.Del          = true;
                item.DeletedAt    = DateTime.Now;
                item.DeletedBy    = User.Identity?.Name;
                item.DeleteReason = reason;
                await _db.SaveChangesAsync();
            }
            TempData["Success"] = "تم حذف تسوية الحجز بنجاح";
            return RedirectToAction("Index");
        }
    }
}
