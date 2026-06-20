using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MedicalInsuranceApp1.Data;
using MedicalInsuranceApp1.Models.Entities;
using MedicalInsuranceApp1.Infrastrcture;

namespace MedicalInsuranceApp1.Controllers
{
    [Authorize]
    [RequirePermission(AppModules.IssuanceStats)]
    public class IssuanceStatsController : Controller
    {
        private readonly AppDbContext _db;
        public IssuanceStatsController(AppDbContext db) => _db = db;

        /// <summary>إحصائيات الإصدار — ملخص جميع العقود والمبالغ</summary>
        public async Task<IActionResult> Index(int? year, string? facilityType, string? status)
        {
            int selYear = year ?? DateTime.Now.Year;

            var contractsQ = _db.InsuranceContracts
                .Include(x => x.SupplyOrders.Where(s => s.Del != true))
                .Include(x => x.Receipts.Where(r => r.Del != true))
                .Where(x => x.Del != true);

            if (year.HasValue)
                contractsQ = contractsQ.Where(x =>
                    x.InsuranceStartDate.Year == year || x.InsuranceEndDate.Year == year);

            if (!string.IsNullOrEmpty(facilityType) && Enum.TryParse<FacilityType>(facilityType, out var ft))
                contractsQ = contractsQ.Where(x => x.FacilityType == ft);

            if (!string.IsNullOrEmpty(status))
                contractsQ = contractsQ.Where(x => x.Status == status);

            var contracts = await contractsQ.OrderByDescending(x => x.InsuranceStartDate).ToListAsync();

            // --- إحصائيات العقود ---
            ViewBag.TotalContracts      = contracts.Count;
            ViewBag.ActiveContracts     = contracts.Count(x => x.Status == "نشط");
            ViewBag.ExpiredContracts    = contracts.Count(x => x.Status == "منتهي");
            ViewBag.CancelledContracts  = contracts.Count(x => x.Status == "ملغي");
            ViewBag.PrivateCount        = contracts.Count(x => x.FacilityType == FacilityType.Private);
            ViewBag.PublicCount         = contracts.Count(x => x.FacilityType == FacilityType.Public);

            // --- إجماليات الأقساط ---
            ViewBag.TotalPrivatePremium  = contracts.Sum(x => x.PrivatePremium);
            ViewBag.TotalPublicPremium   = contracts.Sum(x => x.PublicPremium);
            ViewBag.TotalPremium         = contracts.Sum(x => x.PrivatePremium + x.PublicPremium);
            ViewBag.TotalSupervision     = contracts.Sum(x => x.SupervisionFee);
            ViewBag.TotalUnderCollection = contracts.Sum(x => x.UnderCollectionAmount);
            ViewBag.TotalTreasury        = contracts.Sum(x => x.TreasuryAmount);

            // --- إحصائيات أوامر التوريد ---
            var allOrders = contracts.SelectMany(x => x.SupplyOrders).ToList();
            ViewBag.TotalOrders     = allOrders.Count;
            ViewBag.PendingOrders   = allOrders.Count(x => x.Status == "قيد التحصيل");
            ViewBag.CompletedOrders = allOrders.Count(x => x.Status == "مُوَرَّد");
            ViewBag.TotalOrderAmount = allOrders.Sum(x => x.Amount);

            // --- إحصائيات إيصالات القبض ---
            var allReceipts = contracts.SelectMany(x => x.Receipts).ToList();
            ViewBag.TotalReceipts       = allReceipts.Count;
            ViewBag.TotalReceiptAmount  = allReceipts.Sum(x => x.TotalAmount);
            ViewBag.TotalReceiptTreasury = allReceipts.Sum(x => x.TreasuryAmount);

            // --- فلاتر ---
            ViewBag.Year         = year;
            ViewBag.FacilityType = facilityType;
            ViewBag.Status       = status;

            return View(contracts);
        }
    }
}
