using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ClosedXML.Excel;
using MedicalInsuranceApp1.Data;
using MedicalInsuranceApp1.Models.Entities;
using MedicalInsuranceApp1.Infrastrcture;
using System.Text.RegularExpressions;

namespace MedicalInsuranceApp1.Controllers
{
    [Authorize]
    [RequirePermission(AppModules.DataImport)]
    public class ImportController : Controller
    {
        private readonly AppDbContext _db;
        public ImportController(AppDbContext db) => _db = db;

        // ─── صفحة الاستيراد الرئيسية ───────────────────────────────────────
        [HttpGet]
        public IActionResult Index() => View();

        // ─── استيراد كشف حركة المصارف ──────────────────────────────────────
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ImportBank(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                TempData["Error"] = "الرجاء اختيار ملف";
                return RedirectToAction("Index");
            }

            string ext = Path.GetExtension(file.FileName).ToLower();
            if (ext == ".xls")
            {
                TempData["Error"] = "الملف بصيغة .xls القديمة — افتح الملف في Excel ثم File → Save As → Excel Workbook (.xlsx) ثم ارفع الملف الجديد";
                return RedirectToAction("Index");
            }
            if (ext != ".xlsx")
            {
                TempData["Error"] = "يجب أن يكون الملف بصيغة .xlsx فقط";
                return RedirectToAction("Index");
            }

            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            int totalInserted = 0, totalSkipped = 0, totalSheets = 0;
            var messages = new List<string>();

            try
            {
                using var stream = file.OpenReadStream();
                using var wb = new XLWorkbook(stream);

                foreach (var ws in wb.Worksheets)
                {
                    totalSheets++;
                    var (ins, skip, msg) = await ProcessBankSheet(ws, userId);
                    totalInserted += ins;
                    totalSkipped  += skip;
                    if (!string.IsNullOrEmpty(msg))
                        messages.Add($"[{ws.Name.Trim()}]: {msg}");
                }

                TempData["Success"] = $"تم معالجة {totalSheets} ورقة — " +
                                      $"✓ {totalInserted} حركة جديدة — " +
                                      $"⊘ {totalSkipped} موجودة مسبقاً";
                if (messages.Any())
                    TempData["Info"] = string.Join(" | ", messages);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"خطأ في قراءة الملف: {ex.Message}";
            }

            return RedirectToAction("Index");
        }

        // ─── استيراد كشف بيان العمولات ─────────────────────────────────────
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ImportCommissions(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                TempData["Error"] = "الرجاء اختيار ملف";
                return RedirectToAction("Index");
            }

            string commExt = Path.GetExtension(file.FileName).ToLower();
            if (commExt == ".xls")
            {
                TempData["Error"] = "الملف بصيغة .xls القديمة — افتح الملف في Excel ثم File → Save As → Excel Workbook (.xlsx) ثم ارفع الملف الجديد";
                return RedirectToAction("Index");
            }
            if (commExt != ".xlsx")
            {
                TempData["Error"] = "يجب أن يكون الملف بصيغة .xlsx فقط";
                return RedirectToAction("Index");
            }

            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            int inserted = 0, skipped = 0;

            try
            {
                using var stream = file.OpenReadStream();
                using var wb = new XLWorkbook(stream);
                var ws = wb.Worksheets.First();

                // ابحث عن صف الترويسة (يحتوي "ت" أو "أسم الجهة")
                int headerRow = -1;
                int lastRow = ws.LastRowUsed()?.RowNumber() ?? 0;
                for (int r = 1; r <= lastRow; r++)
                {
                    var c0 = ws.Cell(r, 1).GetString().Trim();
                    var c1 = ws.Cell(r, 2).GetString().Trim();
                    if (c0 == "ت" || c1.Contains("أسم الجهه") || c1.Contains("أسم الجهة"))
                    {
                        headerRow = r;
                        break;
                    }
                }

                if (headerRow < 0)
                {
                    TempData["Error"] = "لم يتم العثور على ترويسة الجدول في ملف العمولات";
                    return RedirectToAction("Index");
                }

                // البيانات تبدأ من الصف التالي للترويسة
                // الأعمدة: ت | اسم الجهة | قيمة القسط | قيمة العمولة | رقم القيد | رقم الاشعار | بيان المستفيد
                for (int r = headerRow + 1; r <= lastRow; r++)
                {
                    var col0 = ws.Cell(r, 1).GetString().Trim();
                    var col1 = ws.Cell(r, 2).GetString().Trim();

                    // توقف عند صف الإجماليات أو عند نهاية البيانات
                    if (col0.Contains("الاجمال") || col0.Contains("إجمالي")) break;
                    if (!int.TryParse(col0, out _) || string.IsNullOrWhiteSpace(col1)) continue;

                    var supplierName    = col1;
                    var premiumStr      = ws.Cell(r, 3).GetString().Trim();
                    var commissionStr   = ws.Cell(r, 4).GetString().Trim();
                    var entryNumber     = ws.Cell(r, 5).GetString().Trim();
                    var authNumber      = ws.Cell(r, 6).GetString().Trim();
                    var beneficiaryInfo = ws.Cell(r, 7).GetString().Trim();

                    if (!decimal.TryParse(premiumStr,    out decimal premium))    premium    = 0;
                    if (!decimal.TryParse(commissionStr, out decimal commission)) commission = 0;

                    // تجنب التكرار: فحص رقم الاشعار + اسم الجهة
                    bool exists = await _db.Commissions.AnyAsync(x =>
                        x.Del != true &&
                        x.SupplierName == supplierName &&
                        x.AuthorizationNumber == authNumber &&
                        x.CommissionAmount == commission);

                    if (exists) { skipped++; continue; }

                    // استخراج اسم المستفيد من النص
                    string? beneficiaryName = ExtractBeneficiaryName(beneficiaryInfo);

                    var comm = new Commission
                    {
                        SupplierName        = supplierName,
                        PremiumAmount       = premium,
                        CommissionRate      = premium > 0 ? Math.Round(commission / premium * 100, 2) : 5,
                        CommissionAmount    = commission,
                        BeneficiaryName     = beneficiaryName,
                        AuthorizationNumber = string.IsNullOrWhiteSpace(authNumber)  ? null : authNumber,
                        EntryNumber         = string.IsNullOrWhiteSpace(entryNumber) ? null : entryNumber,
                        Notes               = beneficiaryInfo,
                        Status              = "مصروفة",   // البيانات الموجودة في الكشف مصروفة بالفعل
                        UserId              = userId,
                        CreatedAt           = DateTime.Now,
                        Del                 = false
                    };

                    _db.Commissions.Add(comm);
                    inserted++;
                }

                await _db.SaveChangesAsync();
                TempData["Success"] = $"✓ {inserted} عمولة جديدة — ⊘ {skipped} موجودة مسبقاً";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"خطأ في قراءة الملف: {ex.Message}";
            }

            return RedirectToAction("Index");
        }

        // ═══════════════════════════════════════════════════════════════════
        //  منطق معالجة ورقة كشف حركة مصرف
        // ═══════════════════════════════════════════════════════════════════
        private async Task<(int inserted, int skipped, string message)> ProcessBankSheet(
            IXLWorksheet ws, string? userId)
        {
            int lastRow = ws.LastRowUsed()?.RowNumber() ?? 0;
            if (lastRow < 5) return (0, 0, "ورقة فارغة");

            // ── 1. ابحث عن صف الترويسة (يحتوي على "ر.م") ──
            int headerRow = -1;
            for (int r = 1; r <= lastRow; r++)
            {
                if (ws.Cell(r, 1).GetString().Trim() == "ر.م")
                { headerRow = r; break; }
            }
            if (headerRow < 0) return (0, 0, "لا يوجد صف ترويسة");

            // ── 2. استخرج معلومات المصرف والشهر من الصفوف السابقة ──
            string bankTitle = "", accountNumber = "", bankDisplayName = "";
            int year = DateTime.Now.Year, month = DateTime.Now.Month;

            for (int r = 1; r < headerRow; r++)
            {
                var cellText = ws.Cell(r, 1).GetString().Trim();
                if (string.IsNullOrWhiteSpace(cellText)) continue;

                if (cellText.Contains("حركة مصرف") || cellText.Contains("حركة مصرف"))
                {
                    bankTitle = cellText;
                    accountNumber   = ExtractAccountNumber(cellText);
                    bankDisplayName = ExtractBankName(cellText);
                }
                else if (cellText.Contains("خلال شهر"))
                {
                    (month, year) = ParseMonthYear(cellText);
                }
            }

            if (string.IsNullOrWhiteSpace(accountNumber))
                return (0, 0, "لم يتم العثور على رقم الحساب");

            // ── 3. أوجد أو أنشئ الحساب البنكي ──
            var account = await _db.BankAccounts
                .FirstOrDefaultAsync(x => x.AccountNumber == accountNumber && x.Del != true);

            if (account == null)
            {
                account = new BankAccount
                {
                    BankName       = bankDisplayName,
                    AccountNumber  = accountNumber,
                    OpeningBalance = 0,
                    CurrentBalance = 0,
                    IsActive       = true,
                    Notes          = $"مُستورد تلقائياً من Excel — {bankTitle}",
                    Del            = false
                };
                _db.BankAccounts.Add(account);
                await _db.SaveChangesAsync();
            }

            // ── 4. حدد أعمدة مدين/دائن من الصف بعد الترويسة ──
            int creditCol = -1, debitCol = -1;  // creditCol = مدين (IN), debitCol = دائن (OUT)
            int subHeaderRow = headerRow + 1;
            int lastCol = ws.LastColumnUsed()?.ColumnNumber() ?? 9;

            for (int c = 1; c <= lastCol; c++)
            {
                var val = ws.Cell(subHeaderRow, c).GetString().Trim();
                if (val == "مدين") creditCol = c;
                if (val == "دائن") debitCol  = c;
            }

            if (creditCol < 0 || debitCol < 0)
                return (0, 0, $"لم يتم تحديد أعمدة مدين/دائن (creditCol={creditCol}, debitCol={debitCol})");

            // حدد موضع الأعمدة الأخرى ديناميكياً من صف الترويسة
            int colEntryNum = -1, colDocNum = -1, colDate = -1,
                colDesc = -1, colDesc2 = -1, colDocType = -1;

            for (int c = 1; c <= lastCol; c++)
            {
                var h = ws.Cell(headerRow, c).GetString().Trim();
                if (h.Contains("رقم القيد"))                      colEntryNum = c;
                if (h.Contains("رقم الصك") || h.Contains("قسيمة الإيداع")) colDocNum = c;
                if (h.Contains("تاريخ"))                           colDate    = c;
                if (h.Contains("البيان") && colDesc < 0)          colDesc    = c;
                else if (h.Contains("البيان"))                     colDesc2   = c;
                if (h.Contains("جهة الإص"))                        colDesc2   = c; // جهة الإصدار
                if (h.Contains("نوع المستند"))                     colDocType = c;
                if (h.Contains("أسم المصرف"))                      colDocType = c;
            }

            // ── 5. احسب الرصيد السابق من صف "رصيد الشهر السابق" إن وجد ──
            decimal prevBalance = 0;
            for (int r = headerRow + 2; r <= lastRow; r++)
            {
                var c0 = ws.Cell(r, 1).GetString().Trim();
                if (c0.Contains("رصيد الشهر السابق") || c0.Contains("قيد الفتح"))
                {
                    // ابحث عن أول قيمة رقمية في هذا الصف
                    for (int c = 2; c <= lastCol; c++)
                    {
                        var v = ws.Cell(r, c).GetString().Trim();
                        if (decimal.TryParse(v, out decimal pv) && pv != 0)
                        { prevBalance = pv; break; }
                    }
                    break;
                }
            }

            // ── 6. اقرأ الحركات ──
            int inserted = 0, skipped = 0;
            decimal runningBalance = prevBalance;

            // جلب الحركات الموجودة لهذا الحساب في هذا الشهر لتجنب التكرار
            var existingKeysList = await _db.BankTransactions
                .Where(x => x.Del != true
                         && x.BankAccountId == account.Id
                         && x.TransactionDate.Month == month
                         && x.TransactionDate.Year  == year)
                .Select(x => x.EntryNumber + "|" + x.DocumentNumber + "|" + x.CreditAmount + "|" + x.DebitAmount)
                .ToListAsync();
            var existingKeys = new HashSet<string>(existingKeysList);

            var newTransactions = new List<BankTransaction>();

            for (int r = headerRow + 2; r <= lastRow; r++)
            {
                var col0Text = ws.Cell(r, 1).GetString().Trim();

                // توقف عند صف الإجماليات
                if (col0Text.Contains("إجمالي") || col0Text.Contains("رصيد الشهر") || col0Text.Contains("الرصيد الحالي"))
                    break;

                // تجاهل الصفوف التي لا تبدأ برقم تسلسلي
                if (!int.TryParse(col0Text, out _)) continue;

                // المبالغ
                decimal credit = 0, debit = 0;
                if (creditCol > 0) TryParseDecimal(ws.Cell(r, creditCol).GetString(), out credit);
                if (debitCol  > 0) TryParseDecimal(ws.Cell(r, debitCol).GetString(),  out debit);

                if (credit == 0 && debit == 0) continue;

                // التفاصيل
                string entryNum = colEntryNum > 0 ? ws.Cell(r, colEntryNum).GetString().Trim() : "";
                string docNum   = colDocNum   > 0 ? ws.Cell(r, colDocNum  ).GetString().Trim() : "";
                string dateStr  = colDate     > 0 ? ws.Cell(r, colDate    ).GetString().Trim() : "";
                string desc     = colDesc     > 0 ? ws.Cell(r, colDesc    ).GetString().Trim() : "";
                string desc2    = colDesc2    > 0 ? ws.Cell(r, colDesc2   ).GetString().Trim() : "";
                string docTyp   = colDocType  > 0 ? ws.Cell(r, colDocType ).GetString().Trim() : "";

                // دمج الوصف
                string fullDesc = string.IsNullOrWhiteSpace(desc2)
                    ? desc
                    : $"{desc2} — {desc}";
                if (!string.IsNullOrWhiteSpace(docTyp) && !fullDesc.Contains(docTyp))
                    fullDesc = string.IsNullOrWhiteSpace(fullDesc) ? docTyp : $"{fullDesc} — {docTyp}";

                // تفسير التاريخ: "1/7" → month=1, day=7, year من الترويسة
                var txDate = ParseCellDate(dateStr, month, year);

                // مفتاح التكرار
                string dupKey = $"{entryNum}|{docNum}|{credit}|{debit}";
                if (existingKeys.Contains(dupKey)) { skipped++; continue; }

                runningBalance += credit - debit;

                var tx = new BankTransaction
                {
                    BankAccountId   = account.Id,
                    TransactionDate = txDate,
                    EntryNumber     = string.IsNullOrWhiteSpace(entryNum) ? null : entryNum,
                    DocumentNumber  = string.IsNullOrWhiteSpace(docNum)   ? null : docNum,
                    Description     = string.IsNullOrWhiteSpace(fullDesc) ? null : fullDesc,
                    CreditAmount    = credit,
                    DebitAmount     = debit,
                    RunningBalance  = runningBalance,
                    DocType         = DetectDocType(desc, docTyp, docNum),
                    Category        = DetectCategory(desc, docTyp, credit, debit),
                    UserId          = userId,
                    CreatedAt       = DateTime.Now,
                    Del             = false
                };

                newTransactions.Add(tx);
                existingKeys.Add(dupKey);
                inserted++;
            }

            if (newTransactions.Any())
            {
                _db.BankTransactions.AddRange(newTransactions);
                // حدّث رصيد الحساب
                account.CurrentBalance = runningBalance;
                await _db.SaveChangesAsync();
            }

            return (inserted, skipped, "");
        }

        // ═══════════════════════════════════════════════════════════════════
        //  دوال مساعدة
        // ═══════════════════════════════════════════════════════════════════

        /// <summary>استخرج رقم الحساب من نص مثل "حساب رقـ( 2000002375 )ـم" أو "((2000002375))"</summary>
        private static string ExtractAccountNumber(string text)
        {
            var m = Regex.Match(text, @"\(+\s*0*(\d{6,})\s*\)+");
            return m.Success ? m.Groups[1].Value : "";
        }

        /// <summary>استخرج اسم المصرف من "حركة مصرف الصحارى الرئيسي حساب رقم ..."</summary>
        private static string ExtractBankName(string text)
        {
            text = text.Trim();
            // أزل "حركة " من البداية
            if (text.StartsWith("حركة ")) text = text[5..].Trim();
            // خذ ما قبل "حساب"
            int idx = text.IndexOf("حساب", StringComparison.Ordinal);
            if (idx > 0) text = text[..idx].Trim();
            return text;
        }

        /// <summary>استخرج الشهر والسنة من "خلال شهر يناير (( 2024 ))"</summary>
        private static (int month, int year) ParseMonthYear(string text)
        {
            // السنة
            var yearMatch = Regex.Match(text, @"\d{4}");
            int year = yearMatch.Success ? int.Parse(yearMatch.Value) : DateTime.Now.Year;

            // الشهر بالعربي
            var months = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
            {
                ["يناير"]=1,["فبراير"]=2,["مارس"]=3,["أبريل"]=4,["ابريل"]=4,
                ["مايو"]=5,["يونيو"]=6,["يوليو"]=7,["أغسطس"]=8,["اغسطس"]=8,
                ["سبتمبر"]=9,["أكتوبر"]=10,["اكتوبر"]=10,["نوفمبر"]=11,["نوفنبر"]=11,["ديسمبر"]=12
            };
            int month = DateTime.Now.Month;
            foreach (var kv in months)
            {
                if (text.Contains(kv.Key)) { month = kv.Value; break; }
            }
            return (month, year);
        }

        /// <summary>حلل تاريخ من الخلية مثل "1/7" أو "2/23"</summary>
        private static DateTime ParseCellDate(string dateStr, int headerMonth, int headerYear)
        {
            if (string.IsNullOrWhiteSpace(dateStr)) return new DateTime(headerYear, headerMonth, 1);

            var parts = dateStr.Split('/');
            if (parts.Length >= 2
                && int.TryParse(parts[0].Trim(), out int p0)
                && int.TryParse(parts[1].Trim(), out int p1))
            {
                // تنسيق شهر/يوم
                int m = p0 >= 1 && p0 <= 12 ? p0 : headerMonth;
                int d = p1 >= 1 && p1 <= 31 ? p1 : 1;
                try { return new DateTime(headerYear, m, d); }
                catch { return new DateTime(headerYear, headerMonth, 1); }
            }
            return new DateTime(headerYear, headerMonth, 1);
        }

        private static bool TryParseDecimal(string s, out decimal result)
        {
            s = s.Replace(",", "").Trim();
            return decimal.TryParse(s, out result);
        }

        /// <summary>حدد نوع المستند من الوصف</summary>
        private static DocumentType? DetectDocType(string desc, string docType, string docNum)
        {
            var combined = $"{desc} {docType} {docNum}".ToLower();
            if (combined.Contains("نقدي"))     return DocumentType.نقدي;
            if (combined.Contains("حوالة") || combined.Contains("تحويل")) return DocumentType.حوالة;
            if (combined.Contains("خصم"))      return DocumentType.خصم_تلقائي;
            if (combined.Contains("قسيمة"))    return DocumentType.قسيمة_إيداع;
            if (combined.Contains("صك") || combined.Contains("شيك")) return DocumentType.صك;
            // إذا رقم المستند رقمي فقط → صك
            if (!string.IsNullOrWhiteSpace(docNum) && Regex.IsMatch(docNum, @"^\d+$"))
                return DocumentType.صك;
            return DocumentType.أخرى;
        }

        /// <summary>حدد تصنيف الحركة من الوصف</summary>
        private static TransactionCategory DetectCategory(string desc, string docType, decimal credit, decimal debit)
        {
            var combined = $"{desc} {docType}".ToLower();

            if (combined.Contains("عمولة"))                     return TransactionCategory.صرف_عمولة;
            if (combined.Contains("مطالبة") || combined.Contains("تعويض")) return TransactionCategory.دفع_مطالبة;
            if (combined.Contains("حجز") || combined.Contains("قضائي"))    return TransactionCategory.حجز_قضائي;
            if (combined.Contains("محاماة") || combined.Contains("أتعاب")) return TransactionCategory.أتعاب_محاماة;
            if (combined.Contains("مرتب") || combined.Contains("راتب") ||
                combined.Contains("ضرائب") || combined.Contains("ضمان") ||
                combined.Contains("إداري") || combined.Contains("اداري"))  return TransactionCategory.مصاريف_إدارية;

            // إيداع (ايداع صكوك / ايداع نقدي / قيد فتح)
            if (credit > 0 && (combined.Contains("ايداع") || combined.Contains("إيداع") ||
                combined.Contains("قسط") || combined.Contains("قيد")))
                return TransactionCategory.إيداع_قسط;

            return TransactionCategory.أخرى;
        }

        /// <summary>استخرج اسم المستفيد من نص مثل "عمولة السيد /حافظ مفتاح المبروك /مسوق..."</summary>
        private static string? ExtractBeneficiaryName(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return null;
            var m = Regex.Match(text, @"/([؀-ۿ\s]+)/");
            if (m.Success) return m.Groups[1].Value.Trim();
            // بديل: خذ الجزء بعد "عمولة " حتى "/"
            var m2 = Regex.Match(text, @"عمولة\s+(?:السيد|المسوق|الوكيل)?\s*/?\s*([؀-ۿ\s]+?)(?:\s*/|$)");
            if (m2.Success) return m2.Groups[1].Value.Trim();
            return null;
        }
    }
}
