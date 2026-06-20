using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MedicalInsuranceApp1.Data;
using MedicalInsuranceApp1.Models.Entities;
using MedicalInsuranceApp1.Infrastrcture;

namespace MedicalInsuranceApp1.Controllers
{
    [Authorize]
    [RequirePermission(AppModules.Marketers)]
    public class MarketersController : Controller
    {
        private readonly AppDbContext _db;
        public MarketersController(AppDbContext db) => _db = db;

        // ===================== قائمة المسوقين =====================
        public async Task<IActionResult> Index(string? search)
        {
            var query = _db.Marketers.Where(x => x.Del != true);

            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(x => x.Name.Contains(search) ||
                                         (x.Phone != null && x.Phone.Contains(search)));

            var list = await query.OrderBy(x => x.Name).ToListAsync();

            // إجماليات
            var ids = list.Select(m => m.Id).ToList();
            var totals = await _db.MarketerCommissionItems
                .Where(i => _db.MarketerCommissionRecords
                    .Where(r => r.Del != true && ids.Contains(r.MarketerId))
                    .Select(r => r.Id)
                    .Contains(i.RecordId))
                .GroupBy(i => _db.MarketerCommissionRecords
                    .Where(r => r.Id == i.RecordId)
                    .Select(r => r.MarketerId)
                    .FirstOrDefault())
                .Select(g => new { MarketerId = g.Key, Total = g.Sum(i => i.CommissionAmount) })
                .ToListAsync();

            ViewBag.Search   = search;
            ViewBag.TotalsDict = totals.ToDictionary(t => t.MarketerId, t => t.Total);

            return View(list);
        }

        // ===================== تفاصيل مسوق (حركته) =====================
        public async Task<IActionResult> Details(int id)
        {
            var marketer = await _db.Marketers.FindAsync(id);
            if (marketer == null || marketer.Del == true) return NotFound();

            var records = await _db.MarketerCommissionRecords
                .Include(r => r.Items)
                .Where(r => r.MarketerId == id && r.Del != true)
                .OrderByDescending(r => r.SettlementDate)
                .ToListAsync();

            ViewBag.TotalCommission = records.SelectMany(r => r.Items).Sum(i => i.CommissionAmount);
            ViewBag.TotalPremiums   = records.SelectMany(r => r.Items).Sum(i => i.TotalPremiums);
            ViewBag.RecordsCount    = records.Count;

            return View((marketer, records));
        }

        // ===================== إضافة مسوق =====================
        [HttpGet]
        public IActionResult Create() => View(new Marketer());

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Marketer model)
        {
            if (!ModelState.IsValid) return View(model);

            model.UserId    = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            model.CreatedAt = DateTime.Now;
            model.Del       = false;

            _db.Marketers.Add(model);
            await _db.SaveChangesAsync();
            TempData["Success"] = "تم إضافة المسوق بنجاح";
            return RedirectToAction("Index");
        }

        // ===================== تعديل مسوق =====================
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var item = await _db.Marketers.FindAsync(id);
            if (item == null || item.Del == true) return NotFound();
            return View(item);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Marketer model)
        {
            var item = await _db.Marketers.FindAsync(id);
            if (item == null) return NotFound();
            if (!ModelState.IsValid) return View(model);

            item.Name     = model.Name;
            item.Phone    = model.Phone;
            item.Notes    = model.Notes;
            item.IsActive = model.IsActive;

            await _db.SaveChangesAsync();
            TempData["Success"] = "تم تحديث بيانات المسوق";
            return RedirectToAction("Index");
        }

        // ===================== حذف مسوق =====================
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, string? reason)
        {
            var item = await _db.Marketers.FindAsync(id);
            if (item == null) return NotFound();

            item.Del          = true;
            item.DeletedAt    = DateTime.Now;
            item.DeletedBy    = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            item.DeleteReason = reason;

            await _db.SaveChangesAsync();
            TempData["Success"] = "تم حذف المسوق";
            return RedirectToAction("Index");
        }

        // ===================== إضافة كشف عمولة =====================
        [HttpGet]
        public async Task<IActionResult> AddRecord(int marketerId)
        {
            var marketer = await _db.Marketers.FindAsync(marketerId);
            if (marketer == null || marketer.Del == true) return NotFound();

            ViewBag.Marketer = marketer;
            return View(new MarketerCommissionRecord
            {
                MarketerId     = marketerId,
                SettlementDate = DateTime.Today
            });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> AddRecord(int marketerId,
            MarketerCommissionRecord model,
            List<string>  facilityNames,
            List<string>  periods,
            List<decimal> totalPremiums,
            List<decimal> returnedPremiums,
            List<decimal> netPremiums,
            List<decimal> commissionRates,
            List<decimal> commissionAmounts)
        {
            var marketer = await _db.Marketers.FindAsync(marketerId);
            if (marketer == null) return NotFound();

            model.MarketerId = marketerId;
            model.UserId     = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            model.CreatedAt  = DateTime.Now;
            model.Del        = false;
            model.Items      = new List<MarketerCommissionItem>();

            for (int i = 0; i < facilityNames.Count; i++)
            {
                if (string.IsNullOrWhiteSpace(facilityNames[i])) continue;
                model.Items.Add(new MarketerCommissionItem
                {
                    FacilityName     = facilityNames[i],
                    Period           = i < periods.Count         ? periods[i]          : null,
                    TotalPremiums    = i < totalPremiums.Count   ? totalPremiums[i]    : 0,
                    ReturnedPremiums = i < returnedPremiums.Count? returnedPremiums[i] : 0,
                    NetPremiums      = i < netPremiums.Count     ? netPremiums[i]      : 0,
                    CommissionRate   = i < commissionRates.Count ? commissionRates[i]  : 0,
                    CommissionAmount = i < commissionAmounts.Count? commissionAmounts[i]: 0,
                });
            }

            _db.MarketerCommissionRecords.Add(model);
            await _db.SaveChangesAsync();
            TempData["Success"] = "تم إضافة الكشف بنجاح";
            return RedirectToAction("Details", new { id = marketerId });
        }

        // ===================== كشف عمولات المسوقين (التقرير الشامل) =====================
        [RequirePermission(AppModules.MarketersReport)]
        public async Task<IActionResult> CommissionsReport(int? marketerId, int? year, int? month, string? search)
        {
            // جلب كل المسوقين النشطين
            var marketers = await _db.Marketers
                .Where(m => m.Del != true)
                .OrderBy(m => m.Name)
                .ToListAsync();

            // سنوات متاحة للفلتر: من 2020 حتى السنة الحالية + أي سنوات موجودة في السجلات
            var dbYears = await _db.MarketerCommissionRecords
                .Where(r => r.Del != true)
                .Select(r => r.SettlementDate.Year)
                .Distinct()
                .ToListAsync();

            int currentYear = DateTime.Now.Year;
            var years = Enumerable.Range(2020, currentYear - 2020 + 1)
                .Union(dbYears)
                .Distinct()
                .OrderByDescending(y => y)
                .ToList();

            // استعلام الكشوف مع البنود
            var recordsQuery = _db.MarketerCommissionRecords
                .Include(r => r.Items)
                .Where(r => r.Del != true);

            if (marketerId.HasValue && marketerId > 0)
                recordsQuery = recordsQuery.Where(r => r.MarketerId == marketerId.Value);

            if (year.HasValue && year > 0)
                recordsQuery = recordsQuery.Where(r => r.SettlementDate.Year == year.Value);

            if (month.HasValue && month > 0)
                recordsQuery = recordsQuery.Where(r => r.SettlementDate.Month == month.Value);

            var allRecords = await recordsQuery
                .OrderBy(r => r.MarketerId)
                .ThenByDescending(r => r.SettlementDate)
                .ToListAsync();

            // فلتر البحث النصي (في Period + FacilityName + اسم المسوق)
            var marketerDict = marketers.ToDictionary(m => m.Id);
            if (!string.IsNullOrWhiteSpace(search))
            {
                var s = search.Trim();
                allRecords = allRecords.Where(r =>
                {
                    var name = marketerDict.ContainsKey(r.MarketerId) ? marketerDict[r.MarketerId].Name : "";
                    return name.Contains(s) ||
                           r.Items.Any(i =>
                               (i.Period        != null && i.Period.Contains(s)) ||
                               (i.FacilityName  != null && i.FacilityName.Contains(s)));
                }).ToList();

                // أبقِ في كل كشف فقط البنود المطابقة
                foreach (var rec in allRecords)
                {
                    var name = marketerDict.ContainsKey(rec.MarketerId) ? marketerDict[rec.MarketerId].Name : "";
                    if (!name.Contains(s))
                    {
                        rec.Items = rec.Items.Where(i =>
                            (i.Period       != null && i.Period.Contains(s)) ||
                            (i.FacilityName != null && i.FacilityName.Contains(s))
                        ).ToList();
                    }
                }
            }

            // تجميع حسب المسوق
            var grouped = allRecords
                .GroupBy(r => r.MarketerId)
                .Select(g => new
                {
                    Marketer = marketerDict.ContainsKey(g.Key) ? marketerDict[g.Key] : null,
                    Records  = g.ToList()
                })
                .Where(x => x.Marketer != null)
                .ToList();

            var allItems = allRecords.SelectMany(r => r.Items).ToList();

            ViewBag.Marketers        = marketers;
            ViewBag.Years            = years;
            ViewBag.SelectedMarketer = marketerId ?? 0;
            ViewBag.SelectedYear     = year ?? 0;
            ViewBag.SelectedMonth    = month ?? 0;
            ViewBag.Search           = search ?? "";
            ViewBag.TotalCommission  = allItems.Sum(i => i.CommissionAmount);
            ViewBag.TotalPremiums    = allItems.Sum(i => i.TotalPremiums);
            ViewBag.TotalNet         = allItems.Sum(i => i.NetPremiums);
            ViewBag.TotalRecords     = allRecords.Count;
            ViewBag.Grouped          = grouped;

            return View();
        }

        // ===================== حذف كشف عمولة =====================
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteRecord(int id)
        {
            var record = await _db.MarketerCommissionRecords.FindAsync(id);
            if (record == null) return NotFound();

            int mid = record.MarketerId;
            record.Del       = true;
            record.DeletedAt = DateTime.Now;
            record.DeletedBy = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            await _db.SaveChangesAsync();
            TempData["Success"] = "تم حذف الكشف";
            return RedirectToAction("Details", new { id = mid });
        }

        // ===================== البحث عن كشف عمولة =====================
        public async Task<IActionResult> Search(string? checkNumber, string? supplyOrderNum, string? receiptNum)
        {
            List<MarketerCommissionRecord> results = new();
            bool searched = !string.IsNullOrWhiteSpace(checkNumber)
                         || !string.IsNullOrWhiteSpace(supplyOrderNum)
                         || !string.IsNullOrWhiteSpace(receiptNum);

            if (searched)
            {
                var query = _db.MarketerCommissionRecords
                    .Include(r => r.Items)
                    .Include(r => r.Marketer)
                    .Where(r => r.Del != true);

                if (!string.IsNullOrWhiteSpace(checkNumber))
                    query = query.Where(r => r.CheckNumber != null && r.CheckNumber.Contains(checkNumber.Trim()));

                if (!string.IsNullOrWhiteSpace(supplyOrderNum))
                    query = query.Where(r => r.SupplyOrderNumbers != null && r.SupplyOrderNumbers.Contains(supplyOrderNum.Trim()));

                if (!string.IsNullOrWhiteSpace(receiptNum))
                    query = query.Where(r => r.ReceiptNumbers != null && r.ReceiptNumbers.Contains(receiptNum.Trim()));

                results = await query.OrderByDescending(r => r.SettlementDate).ToListAsync();
            }

            // قائمة الجهات للقائمة المنسدلة
            var facilities = await _db.InsuranceContracts
                .Where(c => c.Del != true)
                .Select(c => c.FacilityName)
                .Distinct()
                .OrderBy(n => n)
                .ToListAsync();

            // أضف أي جهات موجودة في بنود الكشوف وغير موجودة في العقود
            var existingFacilities = await _db.MarketerCommissionItems
                .Select(i => i.FacilityName)
                .Distinct()
                .ToListAsync();

            // أضف الموردين المسجّلين في جدول الموردين
            var supplierNames = await _db.Suppliers
                .Where(s => s.Del != true && s.IsActive)
                .Select(s => s.Name)
                .ToListAsync();

            var allFacilities = facilities
                .Union(existingFacilities)
                .Union(supplierNames)
                .Where(f => !string.IsNullOrWhiteSpace(f))
                .OrderBy(f => f)
                .ToList();

            // قائمة المسوقين للقائمة المنسدلة في الفورم
            var marketers = await _db.Marketers
                .Where(m => m.Del != true && m.IsActive)
                .OrderBy(m => m.Name)
                .ToListAsync();

            ViewBag.CheckNumber    = checkNumber;
            ViewBag.SupplyOrderNum = supplyOrderNum;
            ViewBag.ReceiptNum     = receiptNum;
            ViewBag.Searched       = searched;
            ViewBag.Facilities     = allFacilities;
            ViewBag.Marketers      = marketers;

            return View(results);
        }

        // ===================== حفظ كشف جديد من صفحة البحث =====================
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> SearchCreate(
            int           marketerId,
            DateTime      settlementDate,
            string?       checkNumber,
            string?       supplyOrderNumbers,
            string?       receiptNumbers,
            string?       preparedBy,
            List<string>  facilityNames,
            List<string>  periods,
            List<decimal> totalPremiums,
            List<decimal> returnedPremiums,
            List<decimal> netPremiums,
            List<decimal> commissionRates,
            List<decimal> commissionAmounts)
        {
            var record = new MarketerCommissionRecord
            {
                MarketerId         = marketerId,
                SettlementDate     = settlementDate,
                CheckNumber        = checkNumber,
                SupplyOrderNumbers = supplyOrderNumbers,
                ReceiptNumbers     = receiptNumbers,
                PreparedBy         = preparedBy,
                UserId             = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value,
                CreatedAt          = DateTime.Now,
                Del                = false,
                Items              = new List<MarketerCommissionItem>()
            };

            for (int i = 0; i < facilityNames.Count; i++)
            {
                if (string.IsNullOrWhiteSpace(facilityNames[i])) continue;
                var rate = i < commissionRates.Count ? commissionRates[i] : 0;
                record.Items.Add(new MarketerCommissionItem
                {
                    FacilityName     = facilityNames[i],
                    Period           = i < periods.Count          ? periods[i]          : null,
                    TotalPremiums    = i < totalPremiums.Count    ? totalPremiums[i]    : 0,
                    ReturnedPremiums = i < returnedPremiums.Count ? returnedPremiums[i] : 0,
                    NetPremiums      = i < netPremiums.Count      ? netPremiums[i]      : 0,
                    CommissionRate   = rate / 100,   // تخزين كنسبة عشرية
                    CommissionAmount = i < commissionAmounts.Count ? commissionAmounts[i] : 0,
                });
            }

            _db.MarketerCommissionRecords.Add(record);
            await _db.SaveChangesAsync();
            TempData["Success"] = "تم حفظ الكشف بنجاح";
            return RedirectToAction("Details", new { id = marketerId });
        }
    }
}
