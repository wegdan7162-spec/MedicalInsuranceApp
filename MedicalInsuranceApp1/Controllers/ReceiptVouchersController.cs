using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MedicalInsuranceApp1.Data;
using MedicalInsuranceApp1.Models.Entities;
using MedicalInsuranceApp1.Infrastrcture;

namespace MedicalInsuranceApp1.Controllers
{
    [Authorize]
    [RequirePermission(AppModules.ReceiptVouchers)]
    public class ReceiptVouchersController : Controller
    {
        private readonly AppDbContext _db;
        public ReceiptVouchersController(AppDbContext db) => _db = db;

        public async Task<IActionResult> Index(int? contractId, string? search, int? month, int? year,
                                               string? collectionType, string? paymentMethod)
        {
            int selMonth = month ?? DateTime.Now.Month;
            int selYear  = year  ?? DateTime.Now.Year;

            var query = _db.ReceiptVouchers
                .Include(x => x.Contract)
                .Include(x => x.SupplyOrder)
                .Where(x => x.Del != true
                          && x.ReceiptDate.Month == selMonth
                          && x.ReceiptDate.Year  == selYear);

            if (contractId.HasValue)
                query = query.Where(x => x.ContractId == contractId);

            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(x =>
                    (x.ReceiptNumber != null && x.ReceiptNumber.Contains(search)) ||
                    (x.FacilityName  != null && x.FacilityName.Contains(search))  ||
                    (x.CheckNumber   != null && x.CheckNumber.Contains(search)));

            if (!string.IsNullOrWhiteSpace(collectionType) && Enum.TryParse<CollectionType>(collectionType, out var ct))
                query = query.Where(x => x.CollectionType == ct);

            if (!string.IsNullOrWhiteSpace(paymentMethod))
                query = query.Where(x => x.PaymentMethod == paymentMethod);

            var list = await query.OrderByDescending(x => x.ReceiptDate).ToListAsync();

            ViewBag.Month          = selMonth;
            ViewBag.Year           = selYear;
            ViewBag.ContractId     = contractId;
            ViewBag.Search         = search;
            ViewBag.CollectionType = collectionType;
            ViewBag.PaymentMethod  = paymentMethod;
            ViewBag.TotalAmount    = list.Sum(x => x.TotalAmount);
            ViewBag.TotalTreasury  = list.Sum(x => x.TreasuryAmount);
            ViewBag.TotalSupervision = list.Sum(x => x.SupervisionFee);
            ViewBag.Contracts       = await _db.InsuranceContracts
                                              .Where(x => x.Del != true).OrderBy(x => x.FacilityName).ToListAsync();
            ViewBag.CollectionTypes = Enum.GetValues<CollectionType>();

            return View(list);
        }

        [HttpGet]
        public async Task<IActionResult> Create(int? contractId, int? supplyOrderId)
        {
            await LoadViewBags();
            var model = new ReceiptVoucher { ReceiptDate = DateTime.Today };
            if (contractId.HasValue)
            {
                model.ContractId = contractId;
                var c = await _db.InsuranceContracts.FindAsync(contractId);
                if (c != null) model.FacilityName = c.FacilityName;
            }
            if (supplyOrderId.HasValue)
            {
                model.SupplyOrderId = supplyOrderId;
                var s = await _db.SupplyOrders.Include(x => x.Contract).FirstOrDefaultAsync(x => x.Id == supplyOrderId);
                if (s != null)
                {
                    model.ContractId   = s.ContractId;
                    model.FacilityName = s.FacilityName ?? s.Contract?.FacilityName;
                    model.DinarAmount  = s.Amount;
                    model.TotalAmount  = s.Amount;
                }
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ReceiptVoucher model)
        {
            // توليد رقم الإيصال تلقائياً إن لم يُرسَل
            if (string.IsNullOrWhiteSpace(model.ReceiptNumber))
            {
                int count = await _db.ReceiptVouchers.CountAsync() + 1;
                model.ReceiptNumber = $"RV-{DateTime.Now:yyyy}-{count:D5}";
            }

            // الإجمالي = المبلغ الكامل بالدينار (لا درهم ولا رسوم إشراف)
            model.TotalAmount    = model.DinarAmount;
            model.TreasuryAmount = model.DinarAmount;

            model.UserId    = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            model.CreatedAt = DateTime.Now;
            model.Del       = false;

            _db.ReceiptVouchers.Add(model);

            // تحديث رصيد العقد إن كان مرتبطاً
            if (model.ContractId.HasValue)
            {
                var c = await _db.InsuranceContracts.FindAsync(model.ContractId);
                if (c != null)
                {
                    c.TreasuryAmount        += model.TreasuryAmount;
                    c.UnderCollectionAmount  = Math.Max(0, c.UnderCollectionAmount - model.TotalAmount);
                }
            }

            // تحديث حالة أمر التوريد — يدخل مرحلة انتظار اعتماد الاكتتاب
            if (model.SupplyOrderId.HasValue)
            {
                var so = await _db.SupplyOrders.FindAsync(model.SupplyOrderId);
                if (so != null)
                {
                    so.WorkflowStatus = "في_انتظار_اكتتاب";
                    // Status تبقى "قيد التحصيل" حتى يعتمد قسم الاكتتاب
                }
            }

            // ── حركة الخزينة تُنشأ يدوياً من قِبَل موظف الخزينة (TreasuryConfirm) ──
            // لا توليد تلقائي هنا

            await _db.SaveChangesAsync();

            // ══════════════════════════════════════════════════════════════
            // ── التوازن الآلي للقيد المحاسبي ──
            // مدين  : المصرف الذي استقبل الإيداع (أو الخزينة إن كان نقداً)
            // دائن  : أقساط مباشرة — اسم الجهة — الفترة التأمينية
            // النتيجة: مدين = دائن تلقائياً ← القيد "مكتمل" فوراً
            // ══════════════════════════════════════════════════════════════

            // ── 1. تحديد اسم حساب الجانب المدين ──
            string debitAcctName;
            string debitDesc;
            if (model.BankAccountId.HasValue)
            {
                var bankAcc = await _db.BankAccounts.FindAsync(model.BankAccountId);
                debitAcctName = bankAcc != null
                    ? $"{bankAcc.BankName} ح/ {bankAcc.AccountNumber}"
                    : "حساب مصرفي";
                debitDesc = bankAcc != null
                    ? $"{bankAcc.BankName} — حساب رقم {bankAcc.AccountNumber}"
                    : $"إيداع قسط — {model.FacilityName} — {model.ReceiptNumber}";
            }
            else
            {
                debitAcctName = "خزينة التأمين";
                debitDesc     = $"استلام نقدي — {model.FacilityName} — {model.ReceiptNumber}";
            }

            // ── 2. تحديد بيانات الجانب الدائن من أمر التوريد ──
            SupplyOrder? linkedSo = model.SupplyOrderId.HasValue
                ? await _db.SupplyOrders.FindAsync(model.SupplyOrderId)
                : null;

            string periodStr     = linkedSo != null
                ? $"{linkedSo.CoverageFrom:yyyy/MM/dd} — {linkedSo.CoverageTo:yyyy/MM/dd}"
                : model.ReceiptDate.ToString("yyyy/MM/dd");
            string creditAcctName = $"أقساط مباشرة — {model.FacilityName}";
            string creditDesc     = $"أقساط الفترة: {periodStr} — {model.FacilityName}";

            // ── 3أ. قيد مرتبط بأمر توريد — تحديث القيد الموجود ──
            if (model.SupplyOrderId.HasValue)
            {
                var entry = await _db.AccountingEntries
                    .Include(x => x.Lines)
                    .FirstOrDefaultAsync(x => x.SupplyOrderId == model.SupplyOrderId
                                           && x.Del != true);
                if (entry != null)
                {
                    _db.AccountingEntryLines.RemoveRange(entry.Lines);
                    entry.Lines.Clear();

                    // مدين: المصرف / الخزينة
                    entry.Lines.Add(new AccountingEntryLine
                    {
                        LineOrder       = 1,
                        AccountName     = debitAcctName,
                        LineDescription = debitDesc,
                        Debit           = model.TotalAmount,
                        Credit          = 0
                    });

                    // دائن: الأقساط للجهة الموردة
                    entry.Lines.Add(new AccountingEntryLine
                    {
                        LineOrder       = 2,
                        AccountName     = creditAcctName,
                        LineDescription = creditDesc,
                        Debit           = 0,
                        Credit          = model.TotalAmount   // مدين = دائن ✓
                    });

                    entry.DebitAmount  = model.TotalAmount;
                    entry.DebitAccount = debitAcctName;
                    entry.EntryDate    = model.ReceiptDate;
                    entry.Status       = "مكتمل";   // متوازن تلقائياً
                    await _db.SaveChangesAsync();
                }
            }
            else
            {
                // ── 3ب. لا يوجد أمر توريد — إنشاء قيد مستقل متوازن ──
                int entrySeq = await _db.AccountingEntries.CountAsync() + 1;
                var newEntry = new AccountingEntry
                {
                    EntryNumber  = $"JV-{model.ReceiptDate:yyyy}-{entrySeq:D5}",
                    EntryDate    = model.ReceiptDate,
                    Description  = $"إيصال قبض — {model.FacilityName}",
                    DebitAmount  = model.TotalAmount,
                    DebitAccount = debitAcctName,
                    Status       = "مكتمل",
                    UserId       = model.UserId,
                    CreatedAt    = DateTime.Now,
                    Del          = false
                };
                newEntry.Lines.Add(new AccountingEntryLine
                {
                    LineOrder       = 1,
                    AccountName     = debitAcctName,
                    LineDescription = debitDesc,
                    Debit           = model.TotalAmount,
                    Credit          = 0
                });
                newEntry.Lines.Add(new AccountingEntryLine
                {
                    LineOrder       = 2,
                    AccountName     = creditAcctName,
                    LineDescription = creditDesc,
                    Debit           = 0,
                    Credit          = model.TotalAmount   // مدين = دائن ✓
                });
                _db.AccountingEntries.Add(newEntry);
                await _db.SaveChangesAsync();
            }

            TempData["Success"] = "تم تسجيل إيصال القبض";
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var item = await _db.ReceiptVouchers.FindAsync(id);
            if (item == null) return NotFound();
            await LoadViewBags();
            return View(item);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ReceiptVoucher model)
        {
            var item = await _db.ReceiptVouchers.FindAsync(id);
            if (item == null) return NotFound();

            item.ReceiptNumber    = model.ReceiptNumber;
            item.ReceiptDate      = model.ReceiptDate;
            item.FacilityName     = model.FacilityName;
            item.DinarAmount      = model.DinarAmount;
            item.DirhamAmount     = 0;
            item.TotalAmount      = model.DinarAmount;
            item.SupervisionFee   = 0;
            item.TreasuryAmount   = model.DinarAmount;
            item.PaymentMethod    = model.PaymentMethod;
            item.CheckNumber      = model.CheckNumber;
            item.BankName         = model.BankName;
            item.CollectionType   = model.CollectionType;
            item.Notes            = model.Notes;

            await _db.SaveChangesAsync();
            TempData["Success"] = "تم تحديث إيصال القبض";
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, string? reason)
        {
            var item = await _db.ReceiptVouchers.FindAsync(id);
            if (item != null)
            {
                item.Del          = true;
                item.DeletedAt    = DateTime.Now;
                item.DeletedBy    = User.Identity?.Name;
                item.DeleteReason = reason;
                await _db.SaveChangesAsync();
            }
            TempData["Success"] = "تم حذف الإيصال";
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Print(int id)
        {
            var item = await _db.ReceiptVouchers
                .Include(x => x.Contract)
                .Include(x => x.SupplyOrder)
                .Include(x => x.BankAccount)
                .FirstOrDefaultAsync(x => x.Id == id && x.Del != true);
            if (item == null) return NotFound();
            return View(item);
        }

        // ─── الحركة اليومية للخزينة ────────────────────────────────────────
        [RequirePermission(AppModules.ReceiptVouchers)]
        public async Task<IActionResult> DailyReport(DateTime? date)
        {
            var selDate = date?.Date ?? DateTime.Today;

            var list = await _db.ReceiptVouchers
                .Include(x => x.Contract)
                .Include(x => x.SupplyOrder)
                .Where(x => x.Del != true && x.ReceiptDate.Date == selDate)
                .OrderBy(x => x.ReceiptNumber)
                .ToListAsync();

            // أوامر التوريد المعلقة (معتمدة اكتتاب ولم تؤكد خزينة بعد)
            var pendingOrders = await _db.SupplyOrders
                .Include(x => x.Supplier)
                .Include(x => x.Receipt)
                .Where(x => x.Del != true && x.WorkflowStatus == "معتمد_اكتتاب")
                .OrderByDescending(x => x.Id)
                .ToListAsync();

            ViewBag.Date             = selDate;
            ViewBag.DateDisplay      = selDate.ToString("yyyy/MM/dd");
            ViewBag.TotalTreasury    = list.Sum(x => x.TreasuryAmount);
            ViewBag.TotalBank        = list.Sum(x => x.BankDepositAmount
                                                     ?? (x.PaymentMethod == "صك" || x.PaymentMethod == "تحويل"
                                                         ? x.TotalAmount - x.SupervisionFee : 0));
            ViewBag.TotalDebtors     = 0m;
            ViewBag.TotalAmount      = list.Sum(x => x.TotalAmount);
            ViewBag.TotalSupervision = list.Sum(x => x.SupervisionFee);
            ViewBag.PendingOrders    = pendingOrders;

            return View(list);
        }

        [RequirePermission(AppModules.ReceiptVouchers)]
        public async Task<IActionResult> DailyMovement(DateTime? date)
        {
            var selDate = date?.Date ?? DateTime.Today;

            var list = await _db.ReceiptVouchers
                .Include(x => x.Contract)
                .Include(x => x.SupplyOrder)
                .Where(x => x.Del != true && x.ReceiptDate.Date == selDate)
                .OrderBy(x => x.ReceiptNumber)
                .ToListAsync();

            ViewBag.Date             = selDate;
            ViewBag.DateDisplay      = selDate.ToString("yyyy/MM/dd");
            ViewBag.TotalTreasury    = list.Sum(x => x.TreasuryAmount);
            ViewBag.TotalAmount      = list.Sum(x => x.TotalAmount);
            ViewBag.TotalSupervision = list.Sum(x => x.SupervisionFee);

            return View(list);
        }

        // ── تحديث مبلغ الإيداع المصرفي من الحركة اليومية (AJAX) ──
        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequirePermission(AppModules.ReceiptVouchers)]
        public async Task<IActionResult> UpdateBankDeposit(int id, decimal amount)
        {
            var rv = await _db.ReceiptVouchers
                .Include(x => x.SupplyOrder)
                .FirstOrDefaultAsync(x => x.Id == id && x.Del != true);

            if (rv == null)
                return Json(new { success = false, message = "إيصال غير موجود" });

            rv.BankDepositAmount = amount;

            // ── تحديث القيد المحاسبي المرتبط بأمر التوريد ──
            if (rv.SupplyOrderId.HasValue)
            {
                var entry = await _db.AccountingEntries
                    .Include(x => x.Lines)
                    .FirstOrDefaultAsync(x => x.SupplyOrderId == rv.SupplyOrderId
                                           && x.Del != true);
                if (entry != null && amount > 0)
                {
                    // تحديد اسم حساب المدين (المصرف أو الخزينة)
                    string debitAcctName;
                    if (rv.BankAccountId.HasValue)
                    {
                        var bankAcc = await _db.BankAccounts.FindAsync(rv.BankAccountId);
                        debitAcctName = bankAcc != null
                            ? $"{bankAcc.BankName} ح/ {bankAcc.AccountNumber}"
                            : (rv.BankName ?? "حساب مصرفي");
                    }
                    else
                    {
                        debitAcctName = string.IsNullOrEmpty(rv.BankName)
                            ? "خزينة التأمين"
                            : rv.BankName;
                    }

                    string debitDesc  = $"إيداع قسط — {rv.FacilityName} — {rv.ReceiptNumber}";
                    string creditAcctName = $"أقساط مباشرة — {rv.FacilityName}";
                    string periodStr  = rv.SupplyOrder != null
                        ? $"{rv.SupplyOrder.CoverageFrom:yyyy/MM/dd} — {rv.SupplyOrder.CoverageTo:yyyy/MM/dd}"
                        : rv.ReceiptDate.ToString("yyyy/MM/dd");
                    string creditDesc = $"أقساط الفترة: {periodStr} — {rv.FacilityName}";

                    _db.AccountingEntryLines.RemoveRange(entry.Lines);
                    entry.Lines.Clear();

                    entry.Lines.Add(new AccountingEntryLine
                    {
                        LineOrder       = 1,
                        AccountName     = debitAcctName,
                        LineDescription = debitDesc,
                        Debit           = amount,
                        Credit          = 0
                    });
                    entry.Lines.Add(new AccountingEntryLine
                    {
                        LineOrder       = 2,
                        AccountName     = creditAcctName,
                        LineDescription = creditDesc,
                        Debit           = 0,
                        Credit          = amount
                    });

                    entry.DebitAmount  = amount;
                    entry.DebitAccount = debitAcctName;
                    entry.Status       = "مكتمل";
                }
            }

            await _db.SaveChangesAsync();
            return Json(new { success = true, amount = amount });
        }

        // ── رقم الإيصال التالي (AJAX) ──
        [HttpGet]
        public async Task<IActionResult> NextReceiptNumber()
        {
            int count  = await _db.ReceiptVouchers.CountAsync() + 1;
            string num = $"RV-{DateTime.Now:yyyy}-{count:D5}";
            return Json(num);
        }

        // ── أوامر التوريد للمورد المحدد (AJAX) ──
        [HttpGet]
        public async Task<IActionResult> SupplyOrdersBySupplier(int supplierId)
        {
            var supplier = await _db.Suppliers.FindAsync(supplierId);
            if (supplier == null) return Json(new List<object>());

            var orders = await _db.SupplyOrders
                .Where(x => x.Del != true
                         && x.Status == "قيد التحصيل"
                         && x.FacilityName == supplier.Name)
                .OrderByDescending(x => x.OrderDate)
                .Select(x => new {
                    x.Id,
                    x.OrderNumber,
                    x.FacilityName,
                    Amount     = x.Amount.ToString("N3"),
                    OrderDate  = x.OrderDate.ToString("yyyy/MM/dd")
                })
                .ToListAsync();

            return Json(orders);
        }

        // ══════════════════════════════════════════════════════════════
        // ── مرحلة الخزينة: تأكيد استلام المبلغ وإثبات حركة الخزينة ──
        // يعبّيها موظف الخزينة يدوياً بعد اعتماد قسم الاكتتاب
        // ══════════════════════════════════════════════════════════════
        [HttpGet]
        public async Task<IActionResult> TreasuryConfirm(int supplyOrderId)
        {
            var so = await _db.SupplyOrders
                .Include(x => x.Receipt)
                .Include(x => x.Contract)
                .FirstOrDefaultAsync(x => x.Id == supplyOrderId && x.Del != true);

            if (so == null) return NotFound();

            var receipt        = so.Receipt;
            var contract       = so.Contract;
            var totalAmount    = receipt?.TotalAmount ?? so.Amount;
            var supervisionFee = Math.Round(totalAmount * 0.005m, 3);
            var netTreasury    = totalAmount - supervisionFee;

            // تسلسل = عدد سجلات الخزينة المكتملة + 1
            int seqNum = await _db.ReceiptVouchers
                .CountAsync(x => x.Del != true && x.TreasuryAmount > 0) + 1;

            ViewBag.SupplyOrder    = so;
            ViewBag.BankAccounts   = await _db.BankAccounts
                                              .Where(x => x.Del != true && x.IsActive)
                                              .OrderBy(x => x.BankName).ToListAsync();
            ViewBag.TotalAmount    = totalAmount;
            ViewBag.SupervisionFee = supervisionFee;
            ViewBag.NetTreasury    = netTreasury;
            ViewBag.ReceiptNumber  = receipt?.ReceiptNumber ?? "";
            ViewBag.FacilityName   = so.FacilityName ?? contract?.FacilityName ?? "";
            ViewBag.CoverageFrom   = so.CoverageFrom;
            ViewBag.CoverageTo     = so.CoverageTo;
            ViewBag.PrivatePremium = contract?.PrivatePremium ?? 0m;
            ViewBag.PublicPremium  = contract?.PublicPremium  ?? 0m;
            ViewBag.SeqNum         = seqNum;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TreasuryConfirm(
            int      supplyOrderId,
            int?     bankAccountId,
            DateTime depositDate,
            decimal  totalAmount,
            decimal  privatePremium,
            decimal  publicPremium,
            decimal  advancePremium,
            decimal  pendingPremium,
            decimal  supervisionFee,
            decimal  netTreasury,
            string?  depositSlipNumber,
            string?  notes)
        {
            var so = await _db.SupplyOrders
                .Include(x => x.Receipt)
                .FirstOrDefaultAsync(x => x.Id == supplyOrderId && x.Del != true);

            if (so == null) return NotFound();

            var userName = User.Identity?.Name
                        ?? User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
                        ?? "—";

            // ── 1. تحديث إيصال القبض بالقيم الفعلية ──
            var receipt = so.Receipt;
            if (receipt != null)
            {
                receipt.PrivatePremium    = privatePremium;
                receipt.PublicPremium     = publicPremium;
                receipt.AdvancePremium    = advancePremium;
                receipt.PendingPremium    = pendingPremium;
                receipt.SupervisionFee    = supervisionFee;
                receipt.TreasuryAmount    = netTreasury;
                receipt.BankDepositAmount = totalAmount;
                receipt.BankAccountId     = bankAccountId;
                if (!string.IsNullOrWhiteSpace(notes))
                    receipt.Notes = notes;
            }

            // ── 2. إنشاء حركة الخزينة ──
            if (bankAccountId.HasValue)
            {
                var account = await _db.BankAccounts.FindAsync(bankAccountId);
                if (account != null)
                {
                    var tx = new BankTransaction
                    {
                        BankAccountId   = bankAccountId.Value,
                        TransactionDate = depositDate,
                        EntryNumber     = receipt?.ReceiptNumber,
                        DocumentNumber  = depositSlipNumber,
                        Description     = $"إيداع قسط — {so.FacilityName} — {receipt?.ReceiptNumber}",
                        CreditAmount    = totalAmount,
                        DebitAmount     = 0,
                        Category        = TransactionCategory.إيداع_قسط,
                        UserId          = userName,
                        CreatedAt       = DateTime.Now,
                        Del             = false
                    };
                    tx.RunningBalance      = account.CurrentBalance + totalAmount;
                    account.CurrentBalance = tx.RunningBalance;
                    _db.BankTransactions.Add(tx);
                    await _db.SaveChangesAsync();

                    if (receipt != null)
                        receipt.LinkedTransactionId = tx.Id;
                }
            }

            // ── 3. تحديث حالة سير العمل ──
            so.WorkflowStatus           = "معتمد_خزينة";
            so.UnderwritingVerifiedAt   = DateTime.Now;
            so.UnderwritingVerifiedBy   = userName;
            so.Status                   = "مُوَرَّد";

            // ── 4. تحديث الجانب المدين في القيد المحاسبي ──
            var entry = await _db.AccountingEntries
                .Include(x => x.Lines)
                .FirstOrDefaultAsync(x => x.SupplyOrderId == supplyOrderId && x.Del != true);

            if (entry != null && bankAccountId.HasValue)
            {
                var bankAcc = await _db.BankAccounts.FindAsync(bankAccountId);
                string debitAcctName = bankAcc != null
                    ? $"{bankAcc.BankName} ح/ {bankAcc.AccountNumber}"
                    : "خزينة التأمين";

                // تحديث البند المدين الموجود أو إضافته
                var debitLine = entry.Lines.FirstOrDefault(l => l.Debit > 0);
                if (debitLine != null)
                {
                    debitLine.AccountName     = debitAcctName;
                    debitLine.Debit           = totalAmount;
                    debitLine.LineDescription = bankAcc != null
                        ? $"{bankAcc.BankName} — حساب رقم {bankAcc.AccountNumber}"
                        : $"إيداع — {so.FacilityName} — {depositDate:yyyy/MM/dd}";
                }
                else
                {
                    entry.Lines.Add(new AccountingEntryLine
                    {
                        LineOrder       = 1,
                        AccountName     = debitAcctName,
                        LineDescription = bankAcc != null
                            ? $"{bankAcc.BankName} — حساب رقم {bankAcc.AccountNumber}"
                            : $"إيداع — {so.FacilityName} — {depositDate:yyyy/MM/dd}",
                        Debit           = totalAmount,
                        Credit          = 0
                    });
                }

                entry.DebitAmount  = totalAmount;
                entry.DebitAccount = debitAcctName;
                var td = entry.Lines.Sum(l => l.Debit);
                var tc = entry.Lines.Sum(l => l.Credit);
                entry.Status = (td == tc && td > 0) ? "مكتمل" : "قيد المراجعة";
            }

            await _db.SaveChangesAsync();
            TempData["Success"] = $"تم تأكيد حركة الخزينة بواسطة {userName}";
            return RedirectToAction("Index", "SupplyOrders");
        }

        // ─── إدخال حركة الخزينة المستقل ──────────────────────────────────────
        [RequirePermission(AppModules.ReceiptVouchers)]
        public async Task<IActionResult> TreasuryMovement()
        {
            int seqNum = await _db.ReceiptVouchers
                .CountAsync(x => x.Del != true && x.TreasuryAmount > 0) + 1;

            ViewBag.SeqNum       = seqNum;
            ViewBag.BankAccounts = await _db.BankAccounts
                                            .Where(x => x.Del != true && x.IsActive)
                                            .OrderBy(x => x.BankName).ToListAsync();
            ViewBag.Suppliers    = await _db.Suppliers
                                            .Where(x => x.Del != true && x.IsActive)
                                            .OrderBy(x => x.Name).ToListAsync();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TreasuryMovement(
            string?  receiptNumber,
            string?  facilityName,
            DateTime receiptDate,
            DateTime? coverageFrom,
            DateTime? coverageTo,
            decimal  totalAmount,
            decimal  privatePremium,
            decimal  publicPremium,
            decimal  advancePremium,
            decimal  pendingPremium,
            decimal  supervisionFee,
            decimal  netTreasury,
            int?     bankAccountId,
            DateTime depositDate,
            string?  depositSlipNumber,
            string?  paymentMethod,
            string?  notes)
        {
            var userName = User.Identity?.Name
                        ?? User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
                        ?? "—";

            // ── 1. إنشاء إيصال القبض ──
            var receipt = new ReceiptVoucher
            {
                ReceiptNumber     = receiptNumber,
                FacilityName      = facilityName,
                ReceiptDate       = receiptDate,
                DinarAmount       = totalAmount,
                TotalAmount       = totalAmount,
                PrivatePremium    = privatePremium,
                PublicPremium     = publicPremium,
                AdvancePremium    = advancePremium,
                PendingPremium    = pendingPremium,
                SupervisionFee    = supervisionFee,
                TreasuryAmount    = netTreasury,
                BankDepositAmount = totalAmount,
                BankAccountId     = bankAccountId,
                CoverageFrom      = coverageFrom,
                CoverageTo        = coverageTo,
                PaymentMethod     = paymentMethod ?? "صك",
                CheckNumber       = depositSlipNumber,
                Notes             = notes,
                CreatedAt         = DateTime.Now,
                Del               = false
            };
            _db.ReceiptVouchers.Add(receipt);
            await _db.SaveChangesAsync();

            // ── 2. إنشاء حركة مصرفية ──
            if (bankAccountId.HasValue)
            {
                var account = await _db.BankAccounts.FindAsync(bankAccountId);
                if (account != null)
                {
                    var tx = new BankTransaction
                    {
                        BankAccountId   = bankAccountId.Value,
                        TransactionDate = depositDate,
                        EntryNumber     = receiptNumber,
                        DocumentNumber  = depositSlipNumber,
                        Description     = $"إيداع قسط — {facilityName} — {receiptNumber}",
                        CreditAmount    = totalAmount,
                        DebitAmount     = 0,
                        Category        = TransactionCategory.إيداع_قسط,
                        UserId          = userName,
                        CreatedAt       = DateTime.Now,
                        Del             = false
                    };
                    tx.RunningBalance      = account.CurrentBalance + totalAmount;
                    account.CurrentBalance = tx.RunningBalance;
                    _db.BankTransactions.Add(tx);
                    await _db.SaveChangesAsync();

                    receipt.LinkedTransactionId = tx.Id;
                    await _db.SaveChangesAsync();
                }
            }

            TempData["Success"] = "تم حفظ حركة الخزينة بنجاح";
            return RedirectToAction("DailyReport");
        }

        private async Task LoadViewBags()
        {
            ViewBag.Suppliers       = await _db.Suppliers
                                              .Where(x => x.Del != true && x.IsActive)
                                              .OrderBy(x => x.Name).ToListAsync();
            ViewBag.Contracts       = await _db.InsuranceContracts
                                              .Where(x => x.Del != true).OrderBy(x => x.FacilityName).ToListAsync();
            ViewBag.SupplyOrders    = await _db.SupplyOrders
                                              .Where(x => x.Del != true && x.Status == "قيد التحصيل")
                                              .OrderByDescending(x => x.OrderDate).ToListAsync();
            ViewBag.BankAccounts    = await _db.BankAccounts
                                              .Where(x => x.Del != true && x.IsActive).OrderBy(x => x.BankName).ToListAsync();
            ViewBag.CollectionTypes = Enum.GetValues<CollectionType>();
        }
    }
}
