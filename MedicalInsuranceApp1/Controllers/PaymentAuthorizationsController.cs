using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MedicalInsuranceApp1.Data;
using MedicalInsuranceApp1.Models.Entities;
using MedicalInsuranceApp1.Infrastrcture;

namespace MedicalInsuranceApp1.Controllers
{
    [Authorize]
    [RequirePermission(AppModules.PaymentAuthorizations)]
    public class PaymentAuthorizationsController : Controller
    {
        private readonly AppDbContext _db;
        public PaymentAuthorizationsController(AppDbContext db) => _db = db;

        public async Task<IActionResult> Index(string? search, string? status, int? month, int? year)
        {
            int selMonth = month ?? DateTime.Now.Month;
            int selYear  = year  ?? DateTime.Now.Year;

            var query = _db.PaymentAuthorizations
                .Include(x => x.BankAccount)
                .Include(x => x.Commission)
                .Where(x => x.Del != true
                          && x.AuthorizationDate.Month == selMonth
                          && x.AuthorizationDate.Year  == selYear);

            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(x =>
                    (x.AuthorizationNumber != null && x.AuthorizationNumber.Contains(search)) ||
                    (x.BeneficiaryName     != null && x.BeneficiaryName.Contains(search))     ||
                    (x.Description         != null && x.Description.Contains(search)));

            if (!string.IsNullOrWhiteSpace(status))
                query = query.Where(x => x.Status == status);

            var list = await query.OrderByDescending(x => x.AuthorizationDate).ToListAsync();

            ViewBag.Month         = selMonth;
            ViewBag.Year          = selYear;
            ViewBag.Search        = search;
            ViewBag.Status        = status;
            ViewBag.TotalAmount   = list.Sum(x => x.Amount);
            ViewBag.CountPending  = list.Count(x => x.Status == "قيد التنفيذ");
            ViewBag.CountExecuted = list.Count(x => x.Status == "منفذ");

            return View(list);
        }

        [HttpGet]
        public async Task<IActionResult> Create(int? commissionId)
        {
            await LoadViewBags();
            var model = new PaymentAuthorization { AuthorizationDate = DateTime.Today };

            if (commissionId.HasValue)
            {
                model.CommissionId = commissionId;
                var c = await _db.Commissions.FindAsync(commissionId);
                if (c != null)
                {
                    model.BeneficiaryName = c.BeneficiaryName ?? c.SupplierName;
                    model.Amount          = c.CommissionAmount;
                    model.Description     = $"صرف عمولة — {c.SupplierName}";
                }
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PaymentAuthorization model)
        {
            model.UserId    = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            model.CreatedAt = DateTime.Now;
            model.Del       = false;

            _db.PaymentAuthorizations.Add(model);

            // إنشاء حركة بنكية تلقائية إن كان حساب بنكي محدداً وحالة "منفذ"
            if (model.BankAccountId.HasValue && model.Status == "منفذ")
            {
                var account = await _db.BankAccounts.FindAsync(model.BankAccountId);
                if (account != null)
                {
                    var tx = new BankTransaction
                    {
                        BankAccountId   = model.BankAccountId.Value,
                        TransactionDate = model.AuthorizationDate,
                        EntryNumber     = model.AuthorizationNumber,
                        DocumentNumber  = model.CheckNumber,
                        Description     = $"صرف — {model.BeneficiaryName}",
                        DebitAmount     = model.Amount,
                        CreditAmount    = 0,
                        Category        = TransactionCategory.صرف_عمولة,
                        CommissionId    = model.CommissionId,
                        UserId          = model.UserId,
                        CreatedAt       = DateTime.Now,
                        Del             = false
                    };
                    tx.RunningBalance      = account.CurrentBalance - model.Amount;
                    account.CurrentBalance = tx.RunningBalance;
                    _db.BankTransactions.Add(tx);
                    await _db.SaveChangesAsync();
                    model.LinkedTransactionId = tx.Id;
                }
            }

            // تحديث حالة العمولة
            if (model.CommissionId.HasValue && model.Status == "منفذ")
            {
                var comm = await _db.Commissions.FindAsync(model.CommissionId);
                if (comm != null) comm.Status = "مصروفة";
            }

            await _db.SaveChangesAsync();
            TempData["Success"] = "تم إضافة إذن الصرف";
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var item = await _db.PaymentAuthorizations.FindAsync(id);
            if (item == null) return NotFound();
            await LoadViewBags();
            return View(item);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, PaymentAuthorization model)
        {
            var item = await _db.PaymentAuthorizations.FindAsync(id);
            if (item == null) return NotFound();

            var prevStatus = item.Status;

            item.AuthorizationNumber = model.AuthorizationNumber;
            item.AuthorizationDate   = model.AuthorizationDate;
            item.BeneficiaryName     = model.BeneficiaryName;
            item.BeneficiaryType     = model.BeneficiaryType;
            item.Amount              = model.Amount;
            item.PaymentMethod       = model.PaymentMethod;
            item.CheckNumber         = model.CheckNumber;
            item.BankAccountId       = model.BankAccountId;
            item.Description         = model.Description;
            item.Status              = model.Status;
            item.Notes               = model.Notes;

            // إذا تغيرت الحالة من "قيد التنفيذ" إلى "منفذ" ولم تكن هناك حركة بنكية
            if (prevStatus != "منفذ" && model.Status == "منفذ" && item.LinkedTransactionId == null && item.BankAccountId.HasValue)
            {
                var account = await _db.BankAccounts.FindAsync(item.BankAccountId);
                if (account != null)
                {
                    var tx = new BankTransaction
                    {
                        BankAccountId   = item.BankAccountId.Value,
                        TransactionDate = item.AuthorizationDate,
                        EntryNumber     = item.AuthorizationNumber,
                        DocumentNumber  = item.CheckNumber,
                        Description     = $"صرف — {item.BeneficiaryName}",
                        DebitAmount     = item.Amount,
                        CreditAmount    = 0,
                        Category        = TransactionCategory.صرف_عمولة,
                        CommissionId    = item.CommissionId,
                        UserId          = item.UserId,
                        CreatedAt       = DateTime.Now,
                        Del             = false
                    };
                    tx.RunningBalance      = account.CurrentBalance - item.Amount;
                    account.CurrentBalance = tx.RunningBalance;
                    _db.BankTransactions.Add(tx);
                    await _db.SaveChangesAsync();
                    item.LinkedTransactionId = tx.Id;

                    if (item.CommissionId.HasValue)
                    {
                        var comm = await _db.Commissions.FindAsync(item.CommissionId);
                        if (comm != null) comm.Status = "مصروفة";
                    }
                }
            }

            await _db.SaveChangesAsync();
            TempData["Success"] = "تم تحديث إذن الصرف";
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, string? reason)
        {
            var item = await _db.PaymentAuthorizations.FindAsync(id);
            if (item != null)
            {
                item.Del          = true;
                item.DeletedAt    = DateTime.Now;
                item.DeletedBy    = User.Identity?.Name;
                item.DeleteReason = reason;
                await _db.SaveChangesAsync();
            }
            TempData["Success"] = "تم حذف إذن الصرف";
            return RedirectToAction("Index");
        }

        private async Task LoadViewBags()
        {
            ViewBag.BankAccounts = await _db.BankAccounts
                .Where(x => x.Del != true && x.IsActive)
                .OrderBy(x => x.BankName).ToListAsync();
            ViewBag.Commissions = await _db.Commissions
                .Where(x => x.Del != true && x.Status == "قيد الصرف")
                .OrderByDescending(x => x.CreatedAt).ToListAsync();
        }
    }
}
