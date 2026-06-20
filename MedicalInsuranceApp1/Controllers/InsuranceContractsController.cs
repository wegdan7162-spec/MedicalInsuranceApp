using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MedicalInsuranceApp1.Data;
using MedicalInsuranceApp1.Models.Entities;
using MedicalInsuranceApp1.Infrastrcture;

namespace MedicalInsuranceApp1.Controllers
{
    [Authorize]
    [RequirePermission(AppModules.InsuranceContracts)]
    public class InsuranceContractsController : Controller
    {
        private readonly AppDbContext _db;
        public InsuranceContractsController(AppDbContext db) => _db = db;

        public async Task<IActionResult> Index(string? search, string? status, string? facilityType, int? year)
        {
            var query = _db.InsuranceContracts.Where(x => x.Del != true);

            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(x => x.FacilityName.Contains(search) ||
                                        (x.ContractNumber != null && x.ContractNumber.Contains(search)));

            if (!string.IsNullOrWhiteSpace(status))
                query = query.Where(x => x.Status == status);

            if (!string.IsNullOrWhiteSpace(facilityType) && Enum.TryParse<FacilityType>(facilityType, out var ft))
                query = query.Where(x => x.FacilityType == ft);

            if (year.HasValue)
                query = query.Where(x => x.InsuranceStartDate.Year == year || x.InsuranceEndDate.Year == year);

            var list = await query.OrderByDescending(x => x.CreatedAt).ToListAsync();

            ViewBag.Search       = search;
            ViewBag.Status       = status;
            ViewBag.FacilityType = facilityType;
            ViewBag.Year         = year;
            ViewBag.TotalPrivate        = list.Sum(x => x.PrivatePremium);
            ViewBag.TotalPublic         = list.Sum(x => x.PublicPremium);
            ViewBag.TotalTreasury       = list.Sum(x => x.TreasuryAmount);
            ViewBag.TotalUnderCollection = list.Sum(x => x.UnderCollectionAmount);

            return View(list);
        }

        [HttpGet]
        public IActionResult Create() => View(new InsuranceContract
        {
            InsuranceStartDate = DateTime.Today,
            InsuranceEndDate   = DateTime.Today.AddYears(1)
        });

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(InsuranceContract model)
        {
            model.UserId    = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            model.CreatedAt = DateTime.Now;
            model.Del       = false;
            _db.InsuranceContracts.Add(model);
            await _db.SaveChangesAsync();
            TempData["Success"] = "تم إضافة عقد التأمين بنجاح";
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var item = await _db.InsuranceContracts.FindAsync(id);
            if (item == null) return NotFound();
            return View(item);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, InsuranceContract model)
        {
            var item = await _db.InsuranceContracts.FindAsync(id);
            if (item == null) return NotFound();

            item.ContractNumber        = model.ContractNumber;
            item.FacilityName          = model.FacilityName;
            item.FacilityType          = model.FacilityType;
            item.Phone                 = model.Phone;
            item.Address               = model.Address;
            item.AuthorizedSignatory   = model.AuthorizedSignatory;
            item.InsuranceStartDate    = model.InsuranceStartDate;
            item.InsuranceEndDate      = model.InsuranceEndDate;
            item.PrivatePremium        = model.PrivatePremium;
            item.PublicPremium         = model.PublicPremium;
            item.SupervisionFee        = model.SupervisionFee;
            item.PrepaidAmount         = model.PrepaidAmount;
            item.UnderCollectionAmount = model.UnderCollectionAmount;
            item.TreasuryAmount        = model.TreasuryAmount;
            item.Status                = model.Status;
            item.Notes                 = model.Notes;

            await _db.SaveChangesAsync();
            TempData["Success"] = "تم تحديث عقد التأمين";
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var item = await _db.InsuranceContracts
                .Include(x => x.SupplyOrders.Where(s => s.Del != true))
                .Include(x => x.Receipts.Where(r => r.Del != true))
                .FirstOrDefaultAsync(x => x.Id == id && x.Del != true);
            if (item == null) return NotFound();
            return View(item);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, string? reason)
        {
            var item = await _db.InsuranceContracts.FindAsync(id);
            if (item != null)
            {
                item.Del          = true;
                item.DeletedAt    = DateTime.Now;
                item.DeletedBy    = User.Identity?.Name;
                item.DeleteReason = reason;
                await _db.SaveChangesAsync();
            }
            TempData["Success"] = "تم حذف العقد";
            return RedirectToAction("Index");
        }
    }
}
