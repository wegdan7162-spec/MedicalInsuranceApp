using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MedicalInsuranceApp1.Data;
using MedicalInsuranceApp1.Models.Entities;
using MedicalInsuranceApp1.Infrastrcture;

namespace MedicalInsuranceApp1.Controllers
{
    [Authorize]
    [RequirePermission(AppModules.SupplyOrders)]
    public class SupplyOrdersController : Controller
    {
        private readonly AppDbContext _db;
        public SupplyOrdersController(AppDbContext db) => _db = db;

        public async Task<IActionResult> Index(int? contractId, string? status, int? year)
        {
            var query = _db.SupplyOrders
                .Include(x => x.Contract)
                .Include(x => x.Receipt)
                .Include(x => x.Supplier)
                .Where(x => x.Del != true);

            if (contractId.HasValue)
                query = query.Where(x => x.ContractId == contractId);

            if (!string.IsNullOrWhiteSpace(status))
                query = query.Where(x => x.Status == status);

            if (year.HasValue)
                query = query.Where(x => x.OrderDate.Year == year || x.CoverageFrom.Year == year);

            var list = await query.OrderByDescending(x => x.OrderDate).ToListAsync();

            ViewBag.ContractId = contractId;
            ViewBag.Status     = status;
            ViewBag.Year       = year;
            ViewBag.Contracts  = await _db.InsuranceContracts
                                          .Where(x => x.Del != true && x.Status == "نشط")
                                          .OrderBy(x => x.FacilityName).ToListAsync();
            ViewBag.TotalAmount    = list.Sum(x => x.Amount);
            ViewBag.CountPending   = list.Count(x => x.Status == "قيد التحصيل");
            ViewBag.CountCollected = list.Count(x => x.Status == "مُوَرَّد");

            return View(list);
        }

        [HttpGet]
        public async Task<IActionResult> Create(int? contractId)
        {
            await LoadViewBags();
            var model = new SupplyOrder
            {
                OrderDate    = DateTime.Today,
                CoverageFrom = DateTime.Today.AddMonths(-3),
                CoverageTo   = DateTime.Today,
                ContractId   = contractId
            };
            if (contractId.HasValue)
            {
                var c = await _db.InsuranceContracts.FindAsync(contractId);
                if (c != null)
                {
                    model.FacilityName = c.FacilityName;
                    model.Amount       = c.PrivatePremium + c.PublicPremium;
                }
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SupplyOrder model)
        {
            model.UserId    = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            model.CreatedAt = DateTime.Now;
            model.Del       = false;

            // ── توليد رقم مسلسل تلقائي دائماً ──
            int nextSeq = await _db.SupplyOrders.CountAsync() + 1;
            model.OrderNumber = nextSeq.ToString("D7"); // مثال: 0000085

            // ── تعبئة اسم المنشأة من المورد إن لم يكن محدداً ──
            if (string.IsNullOrWhiteSpace(model.FacilityName) && model.SupplierId.HasValue)
            {
                var supplier = await _db.Suppliers.FindAsync(model.SupplierId);
                if (supplier != null) model.FacilityName = supplier.Name;
            }

            _db.SupplyOrders.Add(model);
            await _db.SaveChangesAsync();

            // ── المرحلة 1: إنشاء القيد المحاسبي تلقائياً عند حفظ أمر التوريد ──
            var facilityName   = model.FacilityName ?? string.Empty;
            var coveragePeriod = $"{model.CoverageFrom:yyyy/MM/dd} — {model.CoverageTo:yyyy/MM/dd}";
            var description    = $"أقساط مباشرة — {facilityName} — الفترة: {coveragePeriod}";

            // ── احسب رسوم الإشراف (5% من المبلغ الصافي تقريباً) إن لم تكن محددة ──
            // سيتم استكمال البنود بالقيم الدقيقة عند ربط إيصال القبض
            var entry = new AccountingEntry
            {
                EntryNumber   = $"JV-{model.OrderDate:yyyy}-{model.OrderNumber}",
                EntryDate     = model.OrderDate,
                Description   = description,
                SupplyOrderId = model.Id,
                DebitAmount   = model.Amount,
                DebitAccount  = "أقساط تحت التحصيل",
                Status        = "مبدئي",
                UserId        = model.UserId,
                CreatedAt     = DateTime.Now,
                Del           = false
            };

            // ── البنود المبدئية للقيد (مدين — أقساط تحت التحصيل) ──
            // البند الدائن سيُستكمل عند ربط إيصال القبض وقسيمة الإيداع
            entry.Lines.Add(new AccountingEntryLine
            {
                LineOrder       = 1,
                AccountName     = "أقساط تحت التحصيل — قطاع خاص",
                LineDescription = description,
                Debit           = model.Amount,
                Credit          = 0
            });

            _db.AccountingEntries.Add(entry);
            await _db.SaveChangesAsync();

            TempData["Success"] = "تم إضافة أمر التوريد وإنشاء القيد المحاسبي تلقائياً";
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var item = await _db.SupplyOrders
                .Include(x => x.Receipt)
                .Include(x => x.Contract)
                .FirstOrDefaultAsync(x => x.Id == id && x.Del != true);
            if (item == null) return NotFound();
            await LoadViewBags();
            return View(item);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, SupplyOrder model)
        {
            var item = await _db.SupplyOrders.FindAsync(id);
            if (item == null) return NotFound();

            item.OrderNumber         = model.OrderNumber;
            item.OrderDate           = model.OrderDate;
            item.ContractId          = model.ContractId;
            item.FacilityName        = model.FacilityName;
            item.CoverageFrom        = model.CoverageFrom;
            item.CoverageTo          = model.CoverageTo;
            item.QuarterDescription  = model.QuarterDescription;
            item.Amount              = model.Amount;
            item.Status              = model.Status;
            item.Notes               = model.Notes;

            await _db.SaveChangesAsync();
            TempData["Success"] = "تم تحديث أمر التوريد";
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, string? reason)
        {
            var item = await _db.SupplyOrders.FindAsync(id);
            if (item != null)
            {
                item.Del          = true;
                item.DeletedAt    = DateTime.Now;
                item.DeletedBy    = User.Identity?.Name;
                item.DeleteReason = reason;
                await _db.SaveChangesAsync();
            }
            TempData["Success"] = "تم حذف أمر التوريد";
            return RedirectToAction("Index");
        }

        // ══════════════════════════════════════════════════════════════
        // ── مرحلة 1: اعتماد قسم الاكتتاب ──
        // يُستدعى بعد صدور إيصال القبض وعودة صورته إلى الاكتتاب
        // الأثر: WorkflowStatus → معتمد_اكتتاب + بند الدائن في القيد
        // ══════════════════════════════════════════════════════════════
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UnderwritingApprove(int id, string? receiptNumberConfirm)
        {
            var so = await _db.SupplyOrders
                .Include(x => x.Receipt)
                .FirstOrDefaultAsync(x => x.Id == id && x.Del != true);

            if (so == null)
                return NotFound();

            if (so.WorkflowStatus != "في_انتظار_اكتتاب")
            {
                TempData["Error"] = "لا يمكن الاعتماد — الحالة الحالية: " + so.WorkflowStatus;
                return RedirectToAction("Index");
            }

            var userName = User.Identity?.Name
                        ?? User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
                        ?? "—";

            // ── 1. تحديث حالة سير العمل ──
            so.WorkflowStatus         = "معتمد_اكتتاب";
            so.UnderwritingApprovedAt = DateTime.Now;
            so.UnderwritingApprovedBy = userName;

            // إن كان رقم الإيصال مُدخلاً يدوياً وليس مربوطاً بعد → حفظه في ملاحظات
            if (!string.IsNullOrWhiteSpace(receiptNumberConfirm) && so.Receipt == null)
            {
                so.Notes = (so.Notes ?? "") + $" | رقم الإيصال المدخل: {receiptNumberConfirm}";
            }

            // ── 2. إضافة بند الدائن في القيد المحاسبي المرتبط ──
            var entry = await _db.AccountingEntries
                .Include(x => x.Lines)
                .FirstOrDefaultAsync(x => x.SupplyOrderId == id && x.Del != true);

            if (entry != null)
            {
                var receipt      = so.Receipt;
                var facilityName = so.FacilityName ?? string.Empty;
                var period       = $"{so.CoverageFrom:yyyy/MM/dd} — {so.CoverageTo:yyyy/MM/dd}";
                var creditAmount = receipt?.TotalAmount ?? so.Amount;

                bool creditExists = entry.Lines.Any(l => l.Credit > 0);
                if (!creditExists)
                {
                    entry.Lines.Add(new AccountingEntryLine
                    {
                        LineOrder       = entry.Lines.Count + 1,
                        AccountName     = "إيرادات أقساط التأمين",
                        LineDescription = $"قسط {facilityName} — {period} — اعتماد الاكتتاب",
                        Debit           = 0,
                        Credit          = creditAmount
                    });

                    var totalDebit  = entry.Lines.Sum(l => l.Debit);
                    var totalCredit = entry.Lines.Sum(l => l.Credit);
                    entry.Status          = (totalDebit == totalCredit && totalDebit > 0) ? "مكتمل" : "قيد المراجعة";
                    entry.BankDebitAmount = creditAmount;
                }
            }

            await _db.SaveChangesAsync();
            TempData["Success"] = $"تم اعتماد أمر التوريد من قسم الاكتتاب بواسطة {userName}";
            return RedirectToAction("Index");
        }

        // ── رقم أمر التوريد التالي (AJAX) ──
        [HttpGet]
        public async Task<IActionResult> NextOrderNumber()
        {
            int nextSeq = await _db.SupplyOrders.CountAsync() + 1;
            return Json(nextSeq.ToString("D7"));
        }

        public async Task<IActionResult> Print(int id)
        {
            var item = await _db.SupplyOrders
                .Include(x => x.Contract)
                .Include(x => x.Receipt)
                .FirstOrDefaultAsync(x => x.Id == id && x.Del != true);
            if (item == null) return NotFound();
            return View(item);
        }

        private async Task LoadViewBags()
        {
            ViewBag.Suppliers = await _db.Suppliers
                .Where(x => x.Del != true && x.IsActive)
                .OrderBy(x => x.Name)
                .ToListAsync();

            ViewBag.Contracts = await _db.InsuranceContracts
                .Where(x => x.Del != true)
                .OrderBy(x => x.FacilityName)
                .ToListAsync();
        }
    }
}
