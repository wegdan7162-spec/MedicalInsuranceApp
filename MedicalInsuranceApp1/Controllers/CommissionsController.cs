using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MedicalInsuranceApp1.Data;
using MedicalInsuranceApp1.Models.Entities;
using MedicalInsuranceApp1.Infrastrcture;

namespace MedicalInsuranceApp1.Controllers
{
    [Authorize]
    [RequirePermission(AppModules.Commissions)]
    public class CommissionsController : Controller
    {
        private readonly AppDbContext _db;
        public CommissionsController(AppDbContext db) => _db = db;

        public async Task<IActionResult> Index(string? search, string? status, int? year)
        {
            var query = _db.Commissions.Where(x => x.Del != true);

            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(x =>
                    x.SupplierName.Contains(search) ||
                    (x.BeneficiaryName != null && x.BeneficiaryName.Contains(search)) ||
                    (x.AuthorizationNumber != null && x.AuthorizationNumber.Contains(search)));

            if (!string.IsNullOrWhiteSpace(status))
                query = query.Where(x => x.Status == status);

            if (year.HasValue)
                query = query.Where(x => x.CreatedAt.Year == year || (x.PaymentDate.HasValue && x.PaymentDate.Value.Year == year));

            var list = await query.OrderByDescending(x => x.CreatedAt).ToListAsync();

            ViewBag.Search          = search;
            ViewBag.Status          = status;
            ViewBag.Year            = year;
            ViewBag.TotalPremium    = list.Sum(x => x.PremiumAmount);
            ViewBag.TotalCommission = list.Sum(x => x.CommissionAmount);
            ViewBag.TotalPaid       = list.Where(x => x.Status == "مصروفة").Sum(x => x.CommissionAmount);
            ViewBag.TotalPending    = list.Where(x => x.Status == "قيد الصرف").Sum(x => x.CommissionAmount);
            ViewBag.BankAccounts    = await _db.BankAccounts.Where(x => x.Del != true && x.IsActive).OrderBy(x => x.BankName).ToListAsync();

            return View(list);
        }

        [HttpGet]
        public IActionResult Create() => View(new Commission { CommissionRate = 5 });

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Commission model)
        {
            model.UserId    = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            model.CreatedAt = DateTime.Now;
            model.Del       = false;

            // احسب قيمة العمولة إذا لم تُدخل
            if (model.CommissionAmount == 0 && model.PremiumAmount > 0)
                model.CommissionAmount = model.PremiumAmount * model.CommissionRate / 100;

            _db.Commissions.Add(model);
            await _db.SaveChangesAsync();
            TempData["Success"] = "تم إضافة العمولة بنجاح";
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var item = await _db.Commissions.FindAsync(id);
            if (item == null) return NotFound();
            return View(item);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Commission model)
        {
            var item = await _db.Commissions.FindAsync(id);
            if (item == null) return NotFound();

            item.SupplierName        = model.SupplierName;
            item.PremiumAmount       = model.PremiumAmount;
            item.CommissionRate      = model.CommissionRate;
            item.CommissionAmount    = model.CommissionAmount > 0
                ? model.CommissionAmount
                : model.PremiumAmount * model.CommissionRate / 100;
            item.BeneficiaryName     = model.BeneficiaryName;
            item.AuthorizationNumber = model.AuthorizationNumber;
            item.EntryNumber         = model.EntryNumber;
            item.PaymentDate         = model.PaymentDate;
            item.Status              = model.Status;
            item.Notes               = model.Notes;

            await _db.SaveChangesAsync();
            TempData["Success"] = "تم تحديث العمولة بنجاح";
            return RedirectToAction("Index");
        }

        /// <summary>تحويل العمولة من «قيد الصرف» إلى «مصروفة» وإنشاء حركة بنكية</summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkPaid(int id, int bankAccountId, string? checkNumber)
        {
            var item = await _db.Commissions.FindAsync(id);
            if (item == null) return NotFound();

            item.Status      = "مصروفة";
            item.PaymentDate = DateTime.Today;

            // أنشئ حركة بنكية مدين
            var account = await _db.BankAccounts.FindAsync(bankAccountId);
            if (account != null)
            {
                var tx = new BankTransaction
                {
                    BankAccountId   = bankAccountId,
                    TransactionDate = DateTime.Today,
                    EntryNumber     = item.EntryNumber,
                    DocumentNumber  = checkNumber,
                    DocType         = DocumentType.صك,
                    Description     = $"عمولة {item.BeneficiaryName} — {item.SupplierName}",
                    DebitAmount     = item.CommissionAmount,
                    CreditAmount    = 0,
                    Category        = TransactionCategory.صرف_عمولة,
                    CommissionId    = item.Id,
                    UserId          = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value,
                    CreatedAt       = DateTime.Now,
                    Del             = false
                };
                tx.RunningBalance   = account.CurrentBalance - item.CommissionAmount;
                account.CurrentBalance = tx.RunningBalance;
                _db.BankTransactions.Add(tx);
            }

            await _db.SaveChangesAsync();
            TempData["Success"] = "تم تحديد العمولة كمصروفة وإنشاء القيد البنكي";
            return RedirectToAction("Index");
        }

        // ====== كشف بيان العمولات ======
        [RequirePermission(AppModules.CommissionsReport)]
        public async Task<IActionResult> Report(DateTime? from, DateTime? to, string? status)
        {
            // الفترة الافتراضية = ربع السنة الحالي
            var now = DateTime.Now;
            int q = (now.Month - 1) / 3;
            from ??= new DateTime(now.Year, q * 3 + 1, 1);
            to   ??= from.Value.AddMonths(3).AddDays(-1);

            var query = _db.Commissions
                .Where(x => x.Del != true
                         && x.CreatedAt.Date >= from.Value.Date
                         && x.CreatedAt.Date <= to.Value.Date);

            if (!string.IsNullOrWhiteSpace(status))
                query = query.Where(x => x.Status == status);

            var list = await query.OrderBy(x => x.CreatedAt).ToListAsync();

            ViewBag.From            = from.Value.ToString("yyyy-MM-dd");
            ViewBag.To              = to.Value.ToString("yyyy-MM-dd");
            ViewBag.FromDisplay     = from.Value.ToString("yyyy/MM/dd");
            ViewBag.ToDisplay       = to.Value.ToString("yyyy/MM/dd");
            ViewBag.Status          = status;
            ViewBag.TotalPremium    = list.Sum(x => x.PremiumAmount);
            ViewBag.TotalCommission = list.Sum(x => x.CommissionAmount);

            return View(list);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, string? reason)
        {
            var item = await _db.Commissions.FindAsync(id);
            if (item != null)
            {
                item.Del          = true;
                item.DeletedAt    = DateTime.Now;
                item.DeletedBy    = User.Identity?.Name;
                item.DeleteReason = reason;
                await _db.SaveChangesAsync();
            }
            TempData["Success"] = "تم حذف العمولة";
            return RedirectToAction("Index");
        }
    }
}
