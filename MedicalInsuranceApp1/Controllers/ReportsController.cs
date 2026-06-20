using System.Text;
using MedicalInsuranceApp1.Data;
using MedicalInsuranceApp1.Infrastrcture;
using MedicalInsuranceApp1.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace MedicalInsuranceApp1.Controllers
{
    [Authorize]
    public class ReportsController : Controller
    {
        private readonly AppDbContext _db;

        public ReportsController(AppDbContext db)
        {
            _db = db;
        }

        // ===== GET: عرض فورم الفلاتر =====
        public async Task<IActionResult> Index()
        {
            var vm = new ReportFilterVM();
            await FillDropdowns(vm);
            return View(vm);
        }

        // ===== POST: تطبيق الفلاتر وعرض النتائج =====
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(ReportFilterVM filter)
        {
            await FillDropdowns(filter);

            // جلب أنواع المصروفات للأعمدة الديناميكية
            var expenseTypes = await _db.ExpenseTypes
                .OrderBy(e => e.ExpenseName)
                .ToListAsync();
            filter.ExpenseTypeNames = expenseTypes.Select(e => e.ExpenseName).ToList();

            var rows = new List<ReportRowVM>();

            // ===== مطالبات القضايا (OutterClaims) =====
            if (filter.ReportType == "OutterClaims" || filter.ReportType == "Both")
            {
                // لا نستخدم Include للمحكمة مباشرة لأن CourtId غير nullable
                // يجعل EF Core يستخدم INNER JOIN فتختفي السجلات بدون محكمة صالحة
                var query = _db.OutterClaims
                    .Include(c => c.Plaintiff)
                    .Include(c => c.Branch)
                    .Where(c => c.Del == null || c.Del == false)
                    .AsQueryable();

                query = ApplyOutterFilters(query, filter);

                var claims = await query.OrderBy(c => c.Year).ThenBy(c => c.Num).ToListAsync();

                // جلب أسماء المحاكم بشكل منفصل (LEFT JOIN آمن)
                var courtIds   = claims.Select(c => c.CourtId).Distinct().ToList();
                var courtsDict = await _db.Courts
                    .Where(c => courtIds.Contains(c.Id))
                    .ToDictionaryAsync(c => c.Id, c => c.CourtName);

                // جلب المصروفات
                var claimNums  = claims.Select(c => (int)c.Num).Distinct().ToList();
                var claimYears = claims.Select(c => c.Year).Distinct().ToList();
                var expenses   = await _db.Expenses
                    .Include(e => e.ExpenseType)
                    .Where(e => claimNums.Contains(e.ClaimNumber)
                             && claimYears.Contains(e.Year))
                    .ToListAsync();

                foreach (var c in claims)
                {
                    var claimExpenses = expenses
                        .Where(e => e.ClaimNumber == (int)c.Num && e.Year == c.Year)
                        .ToList();

                    var byType = new Dictionary<string, decimal>();
                    foreach (var et in expenseTypes)
                        byType[et.ExpenseName] = claimExpenses
                            .Where(e => e.ExpenseType?.ExpenseName == et.ExpenseName)
                            .Sum(e => e.Amount);

                    rows.Add(new ReportRowVM
                    {
                        ClaimSource   = "قضية",
                        Num           = c.Num,
                        Year          = c.Year,
                        PlaintiffName = c.Plaintiff?.PlainitiffName ?? "-",
                        AgentName     = "",
                        BranchName    = c.Branch?.BranchName ?? "-",
                        Place         = c.Place ?? "-",
                        CourtName     = courtsDict.TryGetValue(c.CourtId, out var cname) ? cname : "-",
                        IncidentDate  = c.IncidentDate,
                        RegDate       = c.RegDate,
                        Reserve       = c.Reserve,
                        Settled       = c.Setteld,
                        TotalExpenses = claimExpenses.Sum(e => e.Amount),
                        ClaimStatus   = c.ClaimStatus ?? "",
                      //  FileStatus     = "",
                        ExpensesByType = byType
                    });
                }
            }

            // ===== الصلح الودي =====
            if (filter.ReportType == "FriendlyClaims" || filter.ReportType == "Both")
            {
                var query = _db.FriendlyClaims
                    .Include(c => c.Plaintiff)
                    .Include(c => c.Branch)
                    .Where(c => c.Del != true)
                    .AsQueryable();

                query = ApplyFriendlyFilters(query, filter);

                var claims = await query.OrderBy(c => c.Year).ThenBy(c => c.Num).ToListAsync();

                var claimNums  = claims.Select(c => (int)c.Num).Distinct().ToList();
                var claimYears = claims.Select(c => c.Year).Distinct().ToList();

                var expenses = await _db.Expenses
                    .Include(e => e.ExpenseType)
                    .Where(e => claimNums.Contains(e.ClaimNumber)
                             && claimYears.Contains(e.Year))
                    .ToListAsync();

                foreach (var c in claims)
                {
                    var claimExpenses = expenses
                        .Where(e => e.ClaimNumber == (int)c.Num && e.Year == c.Year)
                        .ToList();

                    var byType = new Dictionary<string, decimal>();
                    foreach (var et in expenseTypes)
                        byType[et.ExpenseName] = claimExpenses
                            .Where(e => e.ExpenseType?.ExpenseName == et.ExpenseName)
                            .Sum(e => e.Amount);

                    rows.Add(new ReportRowVM
                    {
                        ClaimSource   = "صلح ودي",
                        Num           = c.Num,
                        Year          = c.Year,
                        PlaintiffName = c.Plaintiff?.PlainitiffName ?? "-",
                        BranchName    = c.Branch?.BranchName ?? "-",
                        Place         = c.Place ?? "-",
                        CourtName     = "-",
                        IncidentDate  = c.IncidentDate,
                        RegDate       = c.RegDate,
                        Reserve       = c.Reserve,
                        Settled       = c.Setteld,
                        TotalExpenses = claimExpenses.Sum(e => e.Amount),
                        ClaimStatus   = c.ClaimStatus ?? "",
                       // FileStatus     = "",
                        ExpensesByType = byType
                    });
                }
            }

            filter.Rows       = rows;
            filter.HasResults = true;
            return View(filter);
        }

        // ===== طباعة التقرير =====
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PrintReport(ReportFilterVM filter)
        {
            var expenseTypes = await _db.ExpenseTypes.OrderBy(e => e.ExpenseName).ToListAsync();
            filter.ExpenseTypeNames = expenseTypes.Select(e => e.ExpenseName).ToList();

            var rows = new List<ReportRowVM>();

            if (filter.ReportType == "OutterClaims" || filter.ReportType == "Both")
            {
                var query = _db.OutterClaims
                    .Include(c => c.Plaintiff).Include(c => c.Branch)
                    .Where(c => c.Del == null || c.Del == false).AsQueryable();
                query = ApplyOutterFilters(query, filter);
                var claims = await query.OrderBy(c => c.Year).ThenBy(c => c.Num).ToListAsync();
                var cIds   = claims.Select(c => c.CourtId).Distinct().ToList();
                var cDict  = await _db.Courts.Where(c => cIds.Contains(c.Id))
                    .ToDictionaryAsync(c => c.Id, c => c.CourtName);
                var nums  = claims.Select(c => (int)c.Num).Distinct().ToList();
                var years = claims.Select(c => c.Year).Distinct().ToList();
                var exps  = await _db.Expenses.Include(e => e.ExpenseType)
                    .Where(e => nums.Contains(e.ClaimNumber) && years.Contains(e.Year))
                    .ToListAsync();
                foreach (var c in claims)
                {
                    var ce = exps.Where(e => e.ClaimNumber == (int)c.Num && e.Year == c.Year).ToList();
                    var byType = expenseTypes.ToDictionary(et => et.ExpenseName,
                        et => ce.Where(e => e.ExpenseType?.ExpenseName == et.ExpenseName).Sum(e => e.Amount));
                    rows.Add(new ReportRowVM {
                        ClaimSource = "قضية", Num = c.Num, Year = c.Year,
                        PlaintiffName = c.Plaintiff?.PlainitiffName ?? "-",
                        AgentName = "",
                        BranchName = c.Branch?.BranchName ?? "-",
                        Place = c.Place ?? "-",
                        CourtName = cDict.TryGetValue(c.CourtId, out var cn) ? cn : "-",
                        IncidentDate = c.IncidentDate, RegDate = c.RegDate,
                        Reserve = c.Reserve, Settled = c.Setteld,
                        TotalExpenses = ce.Sum(e => e.Amount),
                        ClaimStatus = c.ClaimStatus ?? "",
                       // FileStatus = "",
                        ExpensesByType = byType
                    });
                }
            }

            if (filter.ReportType == "FriendlyClaims" || filter.ReportType == "Both")
            {
                var query = _db.FriendlyClaims
                    .Include(c => c.Plaintiff).Include(c => c.Branch)
                    .Where(c => c.Del != true).AsQueryable();
                query = ApplyFriendlyFilters(query, filter);
                var claims = await query.OrderBy(c => c.Year).ThenBy(c => c.Num).ToListAsync();
                var nums  = claims.Select(c => (int)c.Num).Distinct().ToList();
                var years = claims.Select(c => c.Year).Distinct().ToList();
                var exps  = await _db.Expenses.Include(e => e.ExpenseType)
                    .Where(e => nums.Contains(e.ClaimNumber) && years.Contains(e.Year))
                    .ToListAsync();
                foreach (var c in claims)
                {
                    var ce = exps.Where(e => e.ClaimNumber == (int)c.Num && e.Year == c.Year).ToList();
                    var byType = expenseTypes.ToDictionary(et => et.ExpenseName,
                        et => ce.Where(e => e.ExpenseType?.ExpenseName == et.ExpenseName).Sum(e => e.Amount));
                    rows.Add(new ReportRowVM {
                        ClaimSource = "صلح ودي", Num = c.Num, Year = c.Year,
                        PlaintiffName = c.Plaintiff?.PlainitiffName ?? "-",
                        AgentName = c.AgentName ?? "",
                        BranchName = c.Branch?.BranchName ?? "-",
                        Place = c.Place ?? "-", CourtName = "-",
                        IncidentDate = c.IncidentDate, RegDate = c.RegDate,
                        Reserve = c.Reserve, Settled = c.Setteld,
                        TotalExpenses = ce.Sum(e => e.Amount),
                        ClaimStatus = c.ClaimStatus ?? "",
                       // FileStatus = "",
                        ExpensesByType = byType
                    });
                }
            }

            filter.Rows      = rows;
            filter.HasResults = true;

            // اسم المستخدم الكامل
            var userName = User.Identity?.Name;
            var appUser  = await _db.Users.FirstOrDefaultAsync(u => u.UserName == userName);
            filter.UserFullName = appUser?.FullName ?? userName ?? "";

            // وصف الفلتر المختار (يظهر يمين التقرير)
            var descParts = new List<string>();

            if (filter.BranchId.HasValue)
            {
                var branch = await _db.Branches.FindAsync(filter.BranchId);
                descParts.Add($"فرع: {branch?.BranchName}");
            }
            if (filter.DateFrom.HasValue || filter.DateTo.HasValue)
            {
                var from = filter.DateFrom.HasValue ? filter.DateFrom.Value.ToString("yyyy/MM/dd") : "---";
                var to   = filter.DateTo.HasValue   ? filter.DateTo.Value.ToString("yyyy/MM/dd")   : "---";
                descParts.Add($"الفترة: {from} – {to}");
            }
            if (!string.IsNullOrWhiteSpace(filter.ClaimStatus))
                descParts.Add($"الحالة: {filter.ClaimStatus}");
            if (!string.IsNullOrWhiteSpace(filter.FileStatus))
                descParts.Add($"نوع الملف: {filter.FileStatus}");
            if (!string.IsNullOrWhiteSpace(filter.Place))
                descParts.Add($"المكان: {filter.Place}");

            filter.FilterDescription = descParts.Any()
                ? string.Join(" | ", descParts)
                : "كل المطالبات";

            return View(filter);
        }

        // ===== تصدير Excel =====
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ExportExcel(ReportFilterVM filter)
        {
            // نفس منطق الـ POST بالأعلى
            var expenseTypes = await _db.ExpenseTypes.OrderBy(e => e.ExpenseName).ToListAsync();
            filter.ExpenseTypeNames = expenseTypes.Select(e => e.ExpenseName).ToList();

            var rows = new List<ReportRowVM>();

            if (filter.ReportType == "OutterClaims" || filter.ReportType == "Both")
            {
                var query = _db.OutterClaims.Include(c => c.Plaintiff).Include(c => c.Branch)
                    .Where(c => c.Del == null || c.Del == false).AsQueryable();
                query = ApplyOutterFilters(query, filter);
                var claims = await query.OrderBy(c => c.Year).ThenBy(c => c.Num).ToListAsync();
                var xIds   = claims.Select(c => c.CourtId).Distinct().ToList();
                var xDict  = await _db.Courts.Where(c => xIds.Contains(c.Id))
                    .ToDictionaryAsync(c => c.Id, c => c.CourtName);
                var nums  = claims.Select(c => (int)c.Num).Distinct().ToList();
                var years = claims.Select(c => c.Year).Distinct().ToList();
                var exps  = await _db.Expenses.Include(e => e.ExpenseType)
                    .Where(e => nums.Contains(e.ClaimNumber) && years.Contains(e.Year))
                    .ToListAsync();

                foreach (var c in claims)
                {
                    var ce    = exps.Where(e => e.ClaimNumber == (int)c.Num && e.Year == c.Year).ToList();
                    var byType = expenseTypes.ToDictionary(et => et.ExpenseName,
                        et => ce.Where(e => e.ExpenseType?.ExpenseName == et.ExpenseName).Sum(e => e.Amount));
                    rows.Add(new ReportRowVM
                    {
                        ClaimSource = "قضية", Num = c.Num, Year = c.Year,
                        PlaintiffName = c.Plaintiff?.PlainitiffName ?? "-",
                        BranchName = c.Branch?.BranchName ?? "-", Place = c.Place ?? "-",
                        CourtName = xDict.TryGetValue(c.CourtId, out var xn) ? xn : "-",
                        IncidentDate = c.IncidentDate,
                        RegDate = c.RegDate, Reserve = c.Reserve, Settled = c.Setteld,
                        TotalExpenses = ce.Sum(e => e.Amount),
                        ClaimStatus = c.ClaimStatus ?? "",
                      //  FileStatus = "",
                        ExpensesByType = byType
                    });
                }
            }

            if (filter.ReportType == "FriendlyClaims" || filter.ReportType == "Both")
            {
                var query = _db.FriendlyClaims.Include(c => c.Plaintiff).Include(c => c.Branch)
                    .Where(c => c.Del == null || c.Del == false).AsQueryable();
                query = ApplyFriendlyFilters(query, filter);
                var claims = await query.OrderBy(c => c.Year).ThenBy(c => c.Num).ToListAsync();
                var nums  = claims.Select(c => (int)c.Num).Distinct().ToList();
                var years = claims.Select(c => c.Year).Distinct().ToList();
                var exps  = await _db.Expenses.Include(e => e.ExpenseType)
                    .Where(e => nums.Contains(e.ClaimNumber) && years.Contains(e.Year))
                    .ToListAsync();

                foreach (var c in claims)
                {
                    var ce    = exps.Where(e => e.ClaimNumber == (int)c.Num && e.Year == c.Year).ToList();
                    var byType = expenseTypes.ToDictionary(et => et.ExpenseName,
                        et => ce.Where(e => e.ExpenseType?.ExpenseName == et.ExpenseName).Sum(e => e.Amount));
                    rows.Add(new ReportRowVM
                    {
                        ClaimSource = "صلح ودي", Num = c.Num, Year = c.Year,
                        PlaintiffName = c.Plaintiff?.PlainitiffName ?? "-",
                        BranchName = c.Branch?.BranchName ?? "-", Place = c.Place ?? "-",
                        CourtName = "-", IncidentDate = c.IncidentDate,
                        RegDate = c.RegDate, Reserve = c.Reserve, Settled = c.Setteld,
                        TotalExpenses = ce.Sum(e => e.Amount),
                        ClaimStatus = c.ClaimStatus ?? "",
                       // FileStatus = "",
                        ExpensesByType = byType
                    });
                }
            }

            // ===== بناء ملف Excel (HTML format - يفتح في Excel مباشرة) =====
            var sb = new StringBuilder();
            sb.AppendLine("<html xmlns:o='urn:schemas-microsoft-com:office:office' xmlns:x='urn:schemas-microsoft-com:office:excel'>");
            sb.AppendLine("<head><meta charset='UTF-8'>");
            sb.AppendLine("<style>td,th{border:1px solid #ccc;padding:6px 10px;font-family:Arial,sans-serif;font-size:12px}");
            sb.AppendLine("th{background:#1B4332;color:#fff;font-weight:bold}");
            sb.AppendLine(".total-row{background:#e8f5e9;font-weight:bold}</style></head><body>");
            sb.AppendLine("<table dir='rtl'><thead><tr>");

            // العناوين حسب الأعمدة المختارة
            sb.AppendLine("<th>#</th>");
            if (filter.ColClaimSource)   sb.AppendLine("<th>النوع</th>");
            if (filter.ColNum)           sb.AppendLine("<th>رقم المطالبة</th><th>السنة</th>");
            if (filter.ColPlaintiff)     sb.AppendLine("<th>المدعي</th>");
            if (filter.ColBranch)        sb.AppendLine("<th>الفرع</th>");
            if (filter.ColPlace)         sb.AppendLine("<th>مكان الواقعة</th>");
            if (filter.ColCourt)         sb.AppendLine("<th>المحكمة</th>");
            if (filter.ColIncidentDate)  sb.AppendLine("<th>تاريخ الواقعة</th>");
            if (filter.ColRegDate)       sb.AppendLine("<th>تاريخ التسجيل</th>");            if (filter.ColClaimStatus)     sb.AppendLine("<th>نوع الملف</th>");
            if (filter.ColClaimStatus)   sb.AppendLine("<th>حالة المطالبة</th>");
            if (filter.ColFileStatus)   sb.AppendLine("<th>نوع المطالبة</th>");
            if (filter.ColReserve)       sb.AppendLine("<th>الاحتياطي</th>");
            if (filter.ColSettled)       sb.AppendLine("<th>المسدد</th>");
            if (filter.ColTotalExpenses) sb.AppendLine("<th>إجمالي المصروفات</th>");
            if (filter.ColExpenseDetail)
                foreach (var et in expenseTypes)
                    sb.AppendLine($"<th>{et.ExpenseName}</th>");

            sb.AppendLine("</tr></thead><tbody>");

            int rowNum2 = 1;
            foreach (var r in rows)
            {
                sb.AppendLine("<tr>");
                sb.AppendLine($"<td>{rowNum2++}</td>");
                if (filter.ColClaimSource)   sb.AppendLine($"<td>{r.ClaimSource}</td>");
                if (filter.ColNum)           sb.AppendLine($"<td>{r.Num}</td><td>{r.Year}</td>");
                if (filter.ColPlaintiff)     sb.AppendLine($"<td>{r.PlaintiffName}</td>");
                if (filter.ColBranch)        sb.AppendLine($"<td>{r.BranchName}</td>");
                if (filter.ColPlace)         sb.AppendLine($"<td>{r.Place}</td>");
                if (filter.ColCourt)         sb.AppendLine($"<td>{r.CourtName}</td>");
                if (filter.ColIncidentDate)  sb.AppendLine($"<td>{(r.IncidentDate.HasValue ? r.IncidentDate.Value.ToString("yyyy/MM/dd") : "-")}</td>");
                if (filter.ColRegDate)       sb.AppendLine($"<td>{r.RegDate.ToString("yyyy/MM/dd")}</td>");                if (filter.ColClaimStatus)     sb.AppendLine($"<td>{r.ClaimStatus}</td>");
                if (filter.ColClaimStatus)   sb.AppendLine($"<td>{r.ClaimStatus}</td>");
               // if (filter.ColFileStatus)   sb.AppendLine($"<td>{r.FileStatus}</td>");
                if (filter.ColReserve)       sb.AppendLine($"<td>{r.Reserve}</td>");
                if (filter.ColSettled)       sb.AppendLine($"<td>{r.Settled}</td>");
                if (filter.ColTotalExpenses) sb.AppendLine($"<td>{r.TotalExpenses}</td>");
                if (filter.ColExpenseDetail)
                    foreach (var et in expenseTypes)
                    {
                        var amt = r.ExpensesByType.TryGetValue(et.ExpenseName, out var v) ? v : 0;
                        sb.AppendLine($"<td>{amt}</td>");
                    }
                sb.AppendLine("</tr>");
            }

            // صف الإجمالي
            if (rows.Any())
            {
                int totalCols = 1
                    + (filter.ColClaimSource  ? 1 : 0)
                    + (filter.ColNum          ? 2 : 0)
                    + (filter.ColPlaintiff    ? 1 : 0)
                    + (filter.ColBranch       ? 1 : 0)
                    + (filter.ColPlace        ? 1 : 0)
                    + (filter.ColCourt        ? 1 : 0)
                    + (filter.ColIncidentDate ? 1 : 0)
                    + (filter.ColRegDate      ? 1 : 0)
                    + (filter.ColClaimStatus ? 1 : 0)
                    + (filter.ColFileStatus ? 1 : 0);

                sb.AppendLine($"<tr class='total-row'><td colspan='{totalCols}'>الإجمالي ({rows.Count})</td>");
                if (filter.ColReserve)       sb.AppendLine($"<td>{rows.Sum(r => r.Reserve)}</td>");
                if (filter.ColSettled)       sb.AppendLine($"<td>{rows.Sum(r => r.Settled)}</td>");
                if (filter.ColTotalExpenses) sb.AppendLine($"<td>{rows.Sum(r => r.TotalExpenses)}</td>");
                if (filter.ColExpenseDetail)
                    foreach (var et in expenseTypes)
                    {
                        var tot = rows.Sum(r => r.ExpensesByType.TryGetValue(et.ExpenseName, out var v) ? v : 0);
                        sb.AppendLine($"<td>{tot}</td>");
                    }
                sb.AppendLine("</tr>");
            }

            sb.AppendLine("</tbody></table></body></html>");

            var bytes = Encoding.UTF8.GetBytes(sb.ToString());
            var fileName = $"تقرير_المطالبات_{DateTime.Now:yyyy-MM-dd}.xls";
            return File(bytes, "application/vnd.ms-excel", fileName);
        }

        // ===== مساعدات =====
        private IQueryable<Models.Entities.OutterClaim> ApplyOutterFilters(
            IQueryable<Models.Entities.OutterClaim> q, ReportFilterVM f)
        {
            if (f.DateFrom.HasValue)
                q = q.Where(c => c.RegDate >= f.DateFrom.Value);
            if (f.DateTo.HasValue)
                q = q.Where(c => c.RegDate <= f.DateTo.Value.Date.AddDays(1).AddSeconds(-1));
            if (f.BranchId.HasValue)
                q = q.Where(c => c.BranchId == f.BranchId);
            if (!string.IsNullOrWhiteSpace(f.Place))
                q = q.Where(c => c.Place != null && c.Place.Contains(f.Place));
            if (!string.IsNullOrWhiteSpace(f.FileStatus))
                q = q.Where(c => c.FileStatus == f.FileStatus);
            if (!string.IsNullOrWhiteSpace(f.ClaimStatus))
            {
                if (f.ClaimStatus == "م.مغطاء")
                    // القيم المخزونة فعلاً: "م مغطاء" (بمسافة) + صيغ أخرى
                    q = q.Where(c => c.ClaimStatus == "م مغطاء"
                                  || c.ClaimStatus == "م.مغطاء"
                                  || c.ClaimStatus == " م.مغطاء"
                                  || c.ClaimStatus == "مغطى"
                                  || c.ClaimStatus == null
                                  || c.ClaimStatus == "");
                else if (f.ClaimStatus == "غير مغطاء")
                    // القيم المخزونة فعلاً: "غيرمغطاء" (بدون مسافة) + صيغ أخرى
                    q = q.Where(c => c.ClaimStatus == "غيرمغطاء"
                                  || c.ClaimStatus == "غير مغطاء"
                                  || c.ClaimStatus == "غير مغطى"
                                  || c.ClaimStatus == "غيرمغطى");
                else
                    q = q.Where(c => c.ClaimStatus == f.ClaimStatus);
            }
            return q;
        }

        private IQueryable<Models.Entities.FriendlyClaim> ApplyFriendlyFilters(
            IQueryable<Models.Entities.FriendlyClaim> q, ReportFilterVM f)
        {
            if (f.DateFrom.HasValue)
                q = q.Where(c => c.RegDate >= f.DateFrom.Value);
            if (f.DateTo.HasValue)
                q = q.Where(c => c.RegDate <= f.DateTo.Value.Date.AddDays(1).AddSeconds(-1));
            if (f.BranchId.HasValue)
                q = q.Where(c => c.BranchId == f.BranchId);
            if (!string.IsNullOrWhiteSpace(f.Place))
                q = q.Where(c => c.Place != null && c.Place.Contains(f.Place));
            if (!string.IsNullOrWhiteSpace(f.FileStatus))
                q = q.Where(c => c.FileStatus == f.FileStatus);
            if (!string.IsNullOrWhiteSpace(f.ClaimStatus))
            {
                if (f.ClaimStatus == "م.مغطاء")
                    q = q.Where(c => c.ClaimStatus == "م مغطاء"
                                  || c.ClaimStatus == "م.مغطاء"
                                  || c.ClaimStatus == " م.مغطاء"
                                  || c.ClaimStatus == "مغطى"
                                  || c.ClaimStatus == null
                                  || c.ClaimStatus == "");
                else if (f.ClaimStatus == "غير مغطاء")
                    q = q.Where(c => c.ClaimStatus == "غيرمغطاء"
                                  || c.ClaimStatus == "غير مغطاء"
                                  || c.ClaimStatus == "غير مغطى"
                                  || c.ClaimStatus == "غيرمغطى");
                else
                    q = q.Where(c => c.ClaimStatus == f.ClaimStatus);
            }
            return q;
        }

        private async Task FillDropdowns(ReportFilterVM vm)
        {
            vm.Branches = await _db.Branches
                .Where(b => b.Del == null || b.Del == false)
                .OrderBy(b => b.BranchName)
                .Select(b => new SelectListItem { Value = b.Id.ToString(), Text = b.BranchName })
                .ToListAsync();
        }
    }
}
