using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MedicalInsuranceApp1.Data;
using MedicalInsuranceApp1.Models.Entities;
using MedicalInsuranceApp1.Infrastrcture;

namespace MedicalInsuranceApp1.Controllers
{
    [Authorize]
    [RequirePermission(AppModules.BankDepositSlips)]
    public class BankDepositSlipsController : Controller
    {
        private readonly AppDbContext _db;
        public BankDepositSlipsController(AppDbContext db) => _db = db;

        public async Task<IActionResult> Index(DateTime? date, int? bankAccountId, int? month, int? year)
        {
            int selMonth = month ?? DateTime.Now.Month;
            int selYear  = year  ?? DateTime.Now.Year;

            var query = _db.BankDepositSlips
                .Include(x => x.BankAccount)
                .Include(x => x.ReceiptVoucher)
                .Where(x => x.Del != true
                          && x.DepositDate.Month == selMonth
                          && x.DepositDate.Year  == selYear);

            if (bankAccountId.HasValue)
                query = query.Where(x => x.BankAccountId == bankAccountId);

            var list = await query.OrderByDescending(x => x.DepositDate).ToListAsync();

            ViewBag.Month          = selMonth;
            ViewBag.Year           = selYear;
            ViewBag.BankAccountId  = bankAccountId;
            ViewBag.TotalAmount    = list.Sum(x => x.Amount);
            ViewBag.TotalChecks    = list.Sum(x => x.CheckCount);
            ViewBag.BankAccounts   = await _db.BankAccounts
                                              .Where(x => x.Del != true && x.IsActive)
                                              .OrderBy(x => x.BankName).ToListAsync();

            return View(list);
        }

        [HttpGet]
        public async Task<IActionResult> Create(int? receiptVoucherId)
        {
            await LoadViewBags();
            var model = new BankDepositSlip { DepositDate = DateTime.Today };

            if (receiptVoucherId.HasValue)
            {
                var rv = await _db.ReceiptVouchers
                    .Include(x => x.Contract)
                    .FirstOrDefaultAsync(x => x.Id == receiptVoucherId);
                if (rv != null)
                {
                    model.ReceiptVoucherId  = rv.Id;
                    model.ClientName        = rv.FacilityName ?? rv.Contract?.FacilityName ?? string.Empty;
                    model.Amount            = rv.TreasuryAmount;
                    model.BankAccountId     = rv.BankAccountId;
                    model.PaymentMethod     = rv.PaymentMethod == "نقدي" ? "نقدي" : "صك";
                }
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BankDepositSlip model)
        {
            model.UserId    = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            model.CreatedAt = DateTime.Now;
            model.Del       = false;

            _db.BankDepositSlips.Add(model);
            await _db.SaveChangesAsync();

            // ── المرحلة 2: إضافة بنود الإيداع المصرفي للقيد المحاسبي ──
            if (model.ReceiptVoucherId.HasValue)
            {
                var rv = await _db.ReceiptVouchers
                    .Include(x => x.SupplyOrder)
                    .FirstOrDefaultAsync(x => x.Id == model.ReceiptVoucherId);

                if (rv?.SupplyOrderId != null)
                {
                    var entry = await _db.AccountingEntries
                        .Include(x => x.Lines)
                        .FirstOrDefaultAsync(x => x.SupplyOrderId == rv.SupplyOrderId && x.Del != true);

                    if (entry != null)
                    {
                        var bankAccount = model.BankAccountId.HasValue
                            ? await _db.BankAccounts.FindAsync(model.BankAccountId)
                            : null;

                        var bankAccountName = bankAccount != null
                            ? $"{bankAccount.BankName} ح/{bankAccount.AccountNumber}"
                            : "حساب مصرفي";

                        // مدين: حساب المصرف (تحويل من الخزينة للمصرف)
                        entry.Lines.Add(new AccountingEntryLine
                        {
                            LineOrder       = 10,
                            AccountName     = bankAccountName,
                            LineDescription = $"إيداع — {model.ClientName}",
                            Debit           = model.Amount,
                            Credit          = 0
                        });

                        // دائن: خزانة التأمين (تقليص الخزينة بعد الإيداع)
                        entry.Lines.Add(new AccountingEntryLine
                        {
                            LineOrder       = 11,
                            AccountName     = "خزانة التأمين",
                            LineDescription = $"إيداع في {bankAccountName}",
                            Debit           = 0,
                            Credit          = model.Amount
                        });

                        entry.BankDepositSlipId = model.Id;
                        entry.BankDebitAmount   = model.Amount;
                        entry.BankDebitAccount  = bankAccountName;
                        entry.Status            = "مكتمل";
                    }
                }
            }

            await _db.SaveChangesAsync();
            TempData["Success"] = "تم تسجيل قسيمة الإيداع";
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var item = await _db.BankDepositSlips.FindAsync(id);
            if (item == null) return NotFound();
            await LoadViewBags();
            return View(item);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, BankDepositSlip model)
        {
            var item = await _db.BankDepositSlips.FindAsync(id);
            if (item == null) return NotFound();

            item.SlipNumber        = model.SlipNumber;
            item.DepositDate       = model.DepositDate;
            item.ClientName        = model.ClientName;
            item.BankAccountId     = model.BankAccountId;
            item.Amount            = model.Amount;
            item.CheckCount        = model.CheckCount;
            item.BeneficiaryBranch = model.BeneficiaryBranch;
            item.DueDate           = model.DueDate;
            item.PaymentMethod     = model.PaymentMethod;
            item.Notes             = model.Notes;

            await _db.SaveChangesAsync();
            TempData["Success"] = "تم تحديث قسيمة الإيداع";
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, string? reason)
        {
            var item = await _db.BankDepositSlips.FindAsync(id);
            if (item != null)
            {
                item.Del          = true;
                item.DeletedAt    = DateTime.Now;
                item.DeletedBy    = User.Identity?.Name;
                item.DeleteReason = reason;
                await _db.SaveChangesAsync();
            }
            TempData["Success"] = "تم حذف قسيمة الإيداع";
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Print(int id)
        {
            var item = await _db.BankDepositSlips
                .Include(x => x.BankAccount)
                .Include(x => x.ReceiptVoucher)
                .FirstOrDefaultAsync(x => x.Id == id && x.Del != true);
            if (item == null) return NotFound();
            return View(item);
        }

        private async Task LoadViewBags()
        {
            ViewBag.BankAccounts    = await _db.BankAccounts
                                              .Where(x => x.Del != true && x.IsActive)
                                              .OrderBy(x => x.BankName).ToListAsync();
            ViewBag.ReceiptVouchers = await _db.ReceiptVouchers
                                              .Where(x => x.Del != true)
                                              .OrderByDescending(x => x.ReceiptDate).ToListAsync();
        }
    }
}
