using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MedicalInsuranceApp1.Data;
using MedicalInsuranceApp1.Models.Entities;
using MedicalInsuranceApp1.Infrastrcture;

namespace MedicalInsuranceApp1.Controllers
{
    [Authorize]
    [RequirePermission(AppModules.BankTransactions)]
    public class BankTransactionsController : Controller
    {
        private readonly AppDbContext _db;
        public BankTransactionsController(AppDbContext db) => _db = db;

        public async Task<IActionResult> Index(
            int? accountId, int? month, int? year,
            string? category, string? search)
        {
            var now = DateTime.Now;
            month ??= now.Month;
            year  ??= now.Year;

            var query = _db.BankTransactions
                .Include(x => x.BankAccount)
                .Include(x => x.FriendlyClaim)
                .Include(x => x.OutterClaim)
                .Where(x => x.Del != true
                         && x.TransactionDate.Month == month
                         && x.TransactionDate.Year  == year);

            if (accountId.HasValue)
                query = query.Where(x => x.BankAccountId == accountId);

            if (!string.IsNullOrWhiteSpace(category) && Enum.TryParse<TransactionCategory>(category, out var cat))
                query = query.Where(x => x.Category == cat);

            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(x =>
                    (x.Description != null && x.Description.Contains(search)) ||
                    (x.DocumentNumber != null && x.DocumentNumber.Contains(search)) ||
                    (x.EntryNumber != null && x.EntryNumber.Contains(search)));

            var list = await query.OrderBy(x => x.TransactionDate).ToListAsync();

            ViewBag.Accounts   = await _db.BankAccounts.Where(x => x.Del != true && x.IsActive).OrderBy(x => x.BankName).ToListAsync();
            ViewBag.AccountId  = accountId;
            ViewBag.Month      = month;
            ViewBag.Year       = year;
            ViewBag.Category   = category;
            ViewBag.Search     = search;
            ViewBag.TotalDebit  = list.Sum(x => x.DebitAmount);
            ViewBag.TotalCredit = list.Sum(x => x.CreditAmount);
            ViewBag.NetBalance  = list.Sum(x => x.CreditAmount - x.DebitAmount);
            ViewBag.Categories  = Enum.GetValues<TransactionCategory>();

            return View(list);
        }

        [HttpGet]
        public async Task<IActionResult> Create(int? accountId)
        {
            await LoadViewBags(accountId);
            var model = new BankTransaction
            {
                TransactionDate = DateTime.Today,
                BankAccountId   = accountId ?? 0
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BankTransaction model)
        {
            model.UserId    = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            model.CreatedAt = DateTime.Now;
            model.Del       = false;

            // احسب الرصيد الجاري
            var lastTx = await _db.BankTransactions
                .Where(x => x.BankAccountId == model.BankAccountId && x.Del != true && x.TransactionDate <= model.TransactionDate)
                .OrderByDescending(x => x.TransactionDate)
                .ThenByDescending(x => x.Id)
                .FirstOrDefaultAsync();

            var prevBalance = lastTx?.RunningBalance
                ?? (await _db.BankAccounts.FindAsync(model.BankAccountId))?.OpeningBalance
                ?? 0;

            model.RunningBalance = prevBalance + model.CreditAmount - model.DebitAmount;

            _db.BankTransactions.Add(model);

            // حدّث رصيد الحساب
            var account = await _db.BankAccounts.FindAsync(model.BankAccountId);
            if (account != null)
                account.CurrentBalance = model.RunningBalance;

            await _db.SaveChangesAsync();
            TempData["Success"] = "تم إضافة الحركة بنجاح";
            return RedirectToAction("Index", new { accountId = model.BankAccountId,
                month = model.TransactionDate.Month, year = model.TransactionDate.Year });
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var item = await _db.BankTransactions.FindAsync(id);
            if (item == null) return NotFound();
            await LoadViewBags(item.BankAccountId);
            return View(item);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, BankTransaction model)
        {
            var item = await _db.BankTransactions.FindAsync(id);
            if (item == null) return NotFound();

            item.TransactionDate = model.TransactionDate;
            item.EntryNumber     = model.EntryNumber;
            item.DocType         = model.DocType;
            item.DocumentNumber  = model.DocumentNumber;
            item.Description     = model.Description;
            item.DebitAmount     = model.DebitAmount;
            item.CreditAmount    = model.CreditAmount;
            item.Category        = model.Category;
            item.FriendlyClaimId = model.FriendlyClaimId;
            item.OutterClaimId   = model.OutterClaimId;

            await _db.SaveChangesAsync();
            TempData["Success"] = "تم تحديث الحركة — يُنصح بإعادة حساب الرصيد";
            return RedirectToAction("Index", new { accountId = item.BankAccountId,
                month = item.TransactionDate.Month, year = item.TransactionDate.Year });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, string? reason)
        {
            var item = await _db.BankTransactions.FindAsync(id);
            if (item != null)
            {
                item.Del          = true;
                item.DeletedAt    = DateTime.Now;
                item.DeletedBy    = User.Identity?.Name;
                item.DeleteReason = reason;
                await _db.SaveChangesAsync();
            }
            TempData["Success"] = "تم حذف الحركة";
            return RedirectToAction("Index");
        }

        // ====== كشف حركة المصرف الشهري ======
        [RequirePermission(AppModules.BankStatement)]
        public async Task<IActionResult> MonthlyStatement(int? bankAccountId, int? month, int? year)
        {
            var now = DateTime.Now;
            month ??= now.Month;
            year  ??= now.Year;

            var accounts = await _db.BankAccounts
                .Where(x => x.Del != true && x.IsActive)
                .OrderBy(x => x.BankName)
                .ToListAsync();

            ViewBag.Accounts      = accounts;
            ViewBag.BankAccountId = bankAccountId;
            ViewBag.Month         = month;
            ViewBag.Year          = year;

            if (!bankAccountId.HasValue)
                return View(new List<BankTransaction>());

            // حركات الشهر المختار
            var txList = await _db.BankTransactions
                .Where(x => x.Del != true
                         && x.BankAccountId == bankAccountId
                         && x.TransactionDate.Month == month
                         && x.TransactionDate.Year  == year)
                .OrderBy(x => x.TransactionDate)
                .ThenBy(x => x.Id)
                .ToListAsync();

            // رصيد الشهر السابق = آخر رصيد جارٍ قبل بداية الشهر
            var firstDay = new DateTime(year.Value, month.Value, 1);
            var prevTx = await _db.BankTransactions
                .Where(x => x.Del != true
                         && x.BankAccountId == bankAccountId
                         && x.TransactionDate < firstDay)
                .OrderByDescending(x => x.TransactionDate)
                .ThenByDescending(x => x.Id)
                .FirstOrDefaultAsync();

            var account = accounts.FirstOrDefault(x => x.Id == bankAccountId);
            decimal prevBalance = prevTx?.RunningBalance
                ?? account?.OpeningBalance ?? 0;

            ViewBag.Account         = account;
            ViewBag.PrevBalance     = prevBalance;
            ViewBag.TotalDebit      = txList.Sum(x => x.DebitAmount);
            ViewBag.TotalCredit     = txList.Sum(x => x.CreditAmount);
            ViewBag.CurrentBalance  = prevBalance + txList.Sum(x => x.CreditAmount - x.DebitAmount);
            ViewBag.MonthName       = new System.Globalization.DateTimeFormatInfo().GetMonthName(month.Value);

            return View(txList);
        }

        private async Task LoadViewBags(int? accountId = null)
        {
            ViewBag.Accounts       = await _db.BankAccounts.Where(x => x.Del != true && x.IsActive).OrderBy(x => x.BankName).ToListAsync();
            ViewBag.FriendlyClaims = await _db.FriendlyClaims.Where(x => x.Del != true)
                .Select(x => new { x.Id, Label = $"{x.Year}-{x.Num} | {x.Plaintiff!.PlainitiffName}" })
                .ToListAsync();
            ViewBag.OutterClaims   = await _db.OutterClaims.Where(x => x.Del != true)
                .Select(x => new { x.Id, Label = $"{x.Year}-{x.Num} | {x.Plaintiff!.PlainitiffName}" })
                .ToListAsync();
            ViewBag.Categories     = Enum.GetValues<TransactionCategory>();
            ViewBag.DocTypes       = Enum.GetValues<DocumentType>();
            ViewBag.SelectedAccount = accountId;
        }
    }
}
