using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MedicalInsuranceApp1.Data;
using MedicalInsuranceApp1.Models.Entities;
using MedicalInsuranceApp1.Infrastrcture;

namespace MedicalInsuranceApp1.Controllers
{
    [Authorize]
    [RequirePermission(AppModules.DailyJournal)]
    public class DailyJournalController : Controller
    {
        private readonly AppDbContext _db;
        public DailyJournalController(AppDbContext db) => _db = db;

        public async Task<IActionResult> Index(int? month, int? year, string? status)
        {
            int selMonth = month ?? DateTime.Now.Month;
            int selYear  = year  ?? DateTime.Now.Year;

            var query = _db.DailyJournals
                .Include(x => x.Items)
                .Where(x => x.Del != true
                          && x.JournalDate.Month == selMonth
                          && x.JournalDate.Year  == selYear);

            if (!string.IsNullOrWhiteSpace(status))
                query = query.Where(x => x.Status == status);

            var list = await query.OrderByDescending(x => x.JournalDate).ToListAsync();

            ViewBag.Month         = selMonth;
            ViewBag.Year          = selYear;
            ViewBag.Status        = status;
            ViewBag.TotalAmount   = list.Sum(x => x.TotalAmount);
            ViewBag.CountPending  = list.Count(x => x.Status == "قيد المراجعة");
            ViewBag.CountApproved = list.Count(x => x.Status == "معتمدة");

            return View(list);
        }

        // ─── إنشاء يومية جديدة ────────────────────────────────────────
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            // نجمع إيصالات اليوم التي لم تُضاف لأي يومية بعد
            var today = DateTime.Today;
            var receipts = await _db.ReceiptVouchers
                .Include(x => x.SupplyOrder)
                .Where(x => x.Del != true && x.ReceiptDate.Date == today)
                .ToListAsync();

            ViewBag.Receipts = receipts;
            ViewBag.Total    = receipts.Sum(x => x.TotalAmount);
            return View(new DailyJournal { JournalDate = today, TotalAmount = receipts.Sum(x => x.TotalAmount) });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DailyJournal model, int[]? receiptIds)
        {
            model.UserId    = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            model.CreatedAt = DateTime.Now;
            model.Del       = false;
            model.Status    = "قيد المراجعة";

            _db.DailyJournals.Add(model);
            await _db.SaveChangesAsync();

            // بناء البنود من الإيصالات المختارة
            if (receiptIds != null && receiptIds.Length > 0)
            {
                var receipts = await _db.ReceiptVouchers
                    .Where(x => receiptIds.Contains(x.Id))
                    .ToListAsync();

                foreach (var rv in receipts)
                {
                    // ابحث عن قسيمة إيداع وقيد محاسبي مرتبطَين
                    var slip = await _db.BankDepositSlips
                        .FirstOrDefaultAsync(x => x.ReceiptVoucherId == rv.Id && x.Del != true);
                    var entry = rv.SupplyOrderId.HasValue
                        ? await _db.AccountingEntries
                            .FirstOrDefaultAsync(x => x.SupplyOrderId == rv.SupplyOrderId && x.Del != true)
                        : null;

                    _db.DailyJournalItems.Add(new DailyJournalItem
                    {
                        JournalId          = model.Id,
                        SupplyOrderId      = rv.SupplyOrderId,
                        ReceiptVoucherId   = rv.Id,
                        BankDepositSlipId  = slip?.Id,
                        AccountingEntryId  = entry?.Id,
                        Amount             = rv.TotalAmount
                    });
                }

                model.TotalAmount = receipts.Sum(x => x.TotalAmount);
                await _db.SaveChangesAsync();
            }

            TempData["Success"] = "تم إنشاء اليومية وتحويلها لقسم المراجعة";
            return RedirectToAction("Index");
        }

        // ─── تفاصيل اليومية ───────────────────────────────────────────
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var journal = await _db.DailyJournals
                .Include(x => x.Items)
                    .ThenInclude(i => i.ReceiptVoucher)
                .Include(x => x.Items)
                    .ThenInclude(i => i.SupplyOrder)
                .Include(x => x.Items)
                    .ThenInclude(i => i.BankDepositSlip)
                .Include(x => x.Items)
                    .ThenInclude(i => i.AccountingEntry)
                .FirstOrDefaultAsync(x => x.Id == id && x.Del != true);

            if (journal == null) return NotFound();
            return View(journal);
        }

        // ─── اعتماد اليومية (قسم المراجعة الداخلية) ──────────────────
        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequirePermission(AppModules.InternalReview)]
        public async Task<IActionResult> Approve(int id, string? notes)
        {
            var journal = await _db.DailyJournals.FindAsync(id);
            if (journal == null) return NotFound();

            journal.Status      = "معتمدة";
            journal.ApprovedAt  = DateTime.Now;
            journal.ApprovedBy  = User.Identity?.Name;
            journal.ReviewNotes = notes;

            // اعتماد القيود المحاسبية المرتبطة
            var entryIds = await _db.DailyJournalItems
                .Where(x => x.JournalId == id && x.AccountingEntryId != null)
                .Select(x => x.AccountingEntryId!.Value)
                .ToListAsync();

            var entries = await _db.AccountingEntries
                .Where(x => entryIds.Contains(x.Id) && x.Status == "مكتمل")
                .ToListAsync();

            foreach (var e in entries)
                e.Status = "معتمد";

            await _db.SaveChangesAsync();
            TempData["Success"] = "تم اعتماد اليومية وجميع قيودها المحاسبية";
            return RedirectToAction("Details", new { id });
        }

        // ─── رد اليومية ───────────────────────────────────────────────
        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequirePermission(AppModules.InternalReview)]
        public async Task<IActionResult> Reject(int id, string? notes)
        {
            var journal = await _db.DailyJournals.FindAsync(id);
            if (journal == null) return NotFound();

            journal.Status      = "مردودة";
            journal.ReviewNotes = notes;
            await _db.SaveChangesAsync();

            TempData["Warning"] = "تم رد اليومية للتصحيح";
            return RedirectToAction("Details", new { id });
        }

        // ─── حذف ناعم ─────────────────────────────────────────────────
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, string? reason)
        {
            var item = await _db.DailyJournals.FindAsync(id);
            if (item != null)
            {
                item.Del          = true;
                item.DeletedAt    = DateTime.Now;
                item.DeletedBy    = User.Identity?.Name;
                item.DeleteReason = reason;
                await _db.SaveChangesAsync();
            }
            TempData["Success"] = "تم حذف اليومية";
            return RedirectToAction("Index");
        }
    }
}
