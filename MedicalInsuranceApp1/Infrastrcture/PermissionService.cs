using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MedicalInsuranceApp1.Data;
using MedicalInsuranceApp1.Models.ViewModels.Identity;

namespace MedicalInsuranceApp1.Infrastrcture
{
    /// <summary>
    /// الوحدات المتاحة في النظام
    /// </summary>
    public static class AppModules
    {
        public const string Dashboard      = "Dashboard";
        public const string OutterClaims   = "OutterClaims";
        public const string FriendlyClaims = "FriendlyClaims";
        public const string Users          = "Users";
        public const string Roles          = "Roles";
        public const string Branches       = "Branches";
        public const string Courts         = "Courts";
        public const string Plaintiffs     = "Plaintiffs";
        public const string Trash          = "Trash";
        public const string Reports                = "Reports";
        public const string ReservationSettlement  = "ReservationSettlement";
        public const string CaseFeeSettlement      = "CaseFeeSettlement";
        public const string PrivateLawyerSettlement = "PrivateLawyerSettlement";

        // ===== المصارف =====
        public const string Banks             = "Banks";

        // ===== الجزء المالي =====
        public const string BankAccounts      = "BankAccounts";
        public const string BankTransactions  = "BankTransactions";
        public const string BankStatement     = "BankStatement";      // كشف حركة المصارف
        public const string Commissions       = "Commissions";
        public const string CommissionsReport = "CommissionsReport";  // كشف بيان العمولات
        public const string DataImport        = "DataImport";         // استيراد بيانات Excel

        // ===== وحدة الاكتتاب =====
        public const string InsuranceContracts    = "InsuranceContracts";
        public const string SupplyOrders          = "SupplyOrders";
        public const string ReceiptVouchers       = "ReceiptVouchers";
        public const string PaymentAuthorizations = "PaymentAuthorizations";
        public const string IssuanceStats         = "IssuanceStats";

        // ===== وحدة المسوقين =====
        public const string Marketers        = "Marketers";
        public const string MarketersReport  = "MarketersReport"; // كشف عمولات المسوقين

        // ===== وحدة الموردين =====
        public const string Suppliers = "Suppliers";

        // ===== كشف المؤمن عليهم =====
        public const string InsuredEmployees = "InsuredEmployees";

        // ===== قسم الخزينة =====
        public const string BankDepositSlips  = "BankDepositSlips";   // قسائم الإيداع المصرفي
        public const string TreasuryDailyReport = "TreasuryDailyReport"; // الحركة اليومية للخزينة

        // ===== القيود المحاسبية =====
        public const string AccountingEntries = "AccountingEntries";

        // ===== اليومية والمراجعة =====
        public const string DailyJournal   = "DailyJournal";
        public const string InternalReview = "InternalReview";

        public static readonly List<(string Key, string Label)> All = new()
        {
            (Dashboard,               "لوحة التحكم"),
            (OutterClaims,            "القضايا "),
            (FriendlyClaims,          "الصلح الودي"),
            (Reports,                 "التقارير"),
            (Users,                   "المستخدمين"),
            (Roles,                   "الأدوار والصلاحيات"),
            (Branches,                "الفروع"),
            (Courts,                  "المحاكم"),
            (Plaintiffs,              "المدعيين"),
            (Trash,                   "سلة المحذوفات"),
            (ReservationSettlement,   "تسوية حجوزات"),
            (CaseFeeSettlement,       "تسوية أتعاب القضايا"),
            (PrivateLawyerSettlement, "تسوية أتعاب المحاماة الخواص"),
            (Banks,                   "المصارف وفروعها"),
            (BankAccounts,            "الحسابات المصرفية"),
            (BankTransactions,        "حركات الحسابات"),
            (BankStatement,           "كشف حركة المصارف"),
            (Commissions,             "العمولات"),
            (CommissionsReport,       "كشف بيان العمولات"),
            (DataImport,             "استيراد بيانات Excel"),
            (InsuranceContracts,      "عقود التأمين الطبي"),
            (SupplyOrders,            "أوامر التوريد"),
            (ReceiptVouchers,         "إيصالات القبض"),
            (PaymentAuthorizations,   "إذون الصرف"),
            (IssuanceStats,           "إحصائيات الإصدار"),
            (Marketers,               "المسوقون"),
            (MarketersReport,         "كشف عمولات المسوقين"),
            (Suppliers,               "الموردون"),
            (InsuredEmployees,        "كشف المؤمن عليهم"),
            (BankDepositSlips,        "قسائم الإيداع المصرفي"),
            (TreasuryDailyReport,     "الحركة اليومية للخزينة"),
            (AccountingEntries,       "القيود المحاسبية"),
            (DailyJournal,            "اليومية"),
            (InternalReview,          "المراجعة الداخلية"),
        };
    }

    public interface IPermissionService
    {
        Task<bool> HasPermissionAsync(string userId, string module, string action);
        Task<Dictionary<string, bool[]>> GetUserPermissionsAsync(string userId);
    }

    /// <summary>
    /// action: "View" | "Add" | "Edit" | "Delete"
    /// </summary>
    public class PermissionService : IPermissionService
    {
        private readonly AppDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;

        public PermissionService(AppDbContext db, UserManager<ApplicationUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        public async Task<bool> HasPermissionAsync(string userId, string module, string action)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null || !user.IsActive) return false;

            // SuperAdmin و Admin يملكان كل الصلاحيات
            if (await _userManager.IsInRoleAsync(user, "SuperAdmin") ||
                await _userManager.IsInRoleAsync(user, "Admin")) return true;

            var roles = await _userManager.GetRolesAsync(user);
            if (!roles.Any()) return false;

            // جلب معرفات الأدوار
            var roleIds = await _db.Roles
                .Where(r => roles.Contains(r.Name!))
                .Select(r => r.Id)
                .ToListAsync();

            var perms = await _db.RolePermissions
                .Where(p => roleIds.Contains(p.RoleId) && p.Module == module)
                .ToListAsync();

            return action switch
            {
                "View"   => perms.Any(p => p.CanView),
                "Add"    => perms.Any(p => p.CanAdd),
                "Edit"   => perms.Any(p => p.CanEdit),
                "Delete" => perms.Any(p => p.CanDelete),
                _        => false
            };
        }

        public async Task<Dictionary<string, bool[]>> GetUserPermissionsAsync(string userId)
        {
            var result = new Dictionary<string, bool[]>();
            foreach (var (key, _) in AppModules.All)
            {
                result[key] = new bool[]
                {
                    await HasPermissionAsync(userId, key, "View"),
                    await HasPermissionAsync(userId, key, "Add"),
                    await HasPermissionAsync(userId, key, "Edit"),
                    await HasPermissionAsync(userId, key, "Delete"),
                };
            }
            return result;
        }
    }
}
