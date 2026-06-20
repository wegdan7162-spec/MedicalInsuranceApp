using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MedicalInsuranceApp1.Data;
using MedicalInsuranceApp1.Models.ViewModels;
using MedicalInsuranceApp1.Infrastrcture;
using MedicalInsuranceApp1.Models.ViewModels.Identity;

namespace MedicalInsuranceApp1.Controllers
{
    [Authorize]
    [RequirePermission(AppModules.Dashboard)]
    public class DashboardController : Controller
    {
        private readonly AppDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;

        public DashboardController(
            AppDbContext db,
            UserManager<ApplicationUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var vm = new DashboardVM
            {
                // القضايا = TblOutters (البيانات القديمة)
                TotalOutter   = await _db.OutterClaims.CountAsync(x => x.Del != true),
                TotalFriendly = await _db.FriendlyClaims.CountAsync(x => x.Del != true),

                TotalReserve =
                    (await _db.OutterClaims.Where(x => x.Del != true).SumAsync(x => (double?)x.Reserve)   ?? 0) +
                    (await _db.FriendlyClaims.Where(x => x.Del != true).SumAsync(x => (double?)x.Reserve) ?? 0),

                TotalSettled =
                    (await _db.OutterClaims.Where(x => x.Del != true).SumAsync(x => (double?)x.Setteld)   ?? 0) +
                    (await _db.FriendlyClaims.Where(x => x.Del != true).SumAsync(x => (double?)x.Setteld) ?? 0),

                TotalUsers  = _userManager.Users.Count(),
                ActiveUsers = _userManager.Users.Count(x => x.IsActive),

                // ===== الجزء المالي =====
                TotalBankAccounts = await _db.BankAccounts.CountAsync(x => x.Del != true && x.IsActive),
                TotalBankBalance  = await _db.BankAccounts.Where(x => x.Del != true && x.IsActive)
                                        .SumAsync(x => (decimal?)x.CurrentBalance) ?? 0,

                TotalCreditThisMonth = await _db.BankTransactions
                    .Where(x => x.Del != true
                             && x.TransactionDate.Month == DateTime.Now.Month
                             && x.TransactionDate.Year  == DateTime.Now.Year)
                    .SumAsync(x => (decimal?)x.CreditAmount) ?? 0,

                TotalDebitThisMonth = await _db.BankTransactions
                    .Where(x => x.Del != true
                             && x.TransactionDate.Month == DateTime.Now.Month
                             && x.TransactionDate.Year  == DateTime.Now.Year)
                    .SumAsync(x => (decimal?)x.DebitAmount) ?? 0,

                TotalCommissions   = await _db.Commissions
                    .Where(x => x.Del != true && x.Status == "مصروفة")
                    .SumAsync(x => (decimal?)x.CommissionAmount) ?? 0,

                PendingCommissions = await _db.Commissions
                    .CountAsync(x => x.Del != true && x.Status == "قيد الصرف"),

                RecentClaims = await _db.OutterClaims
                    .Include(x => x.Branch)
                    .Include(x => x.Plaintiff)
                    .Where(x => x.Del != true)
                    .OrderByDescending(x => x.RegDate)
                    .Take(5)
                    .ToListAsync()
                    .ContinueWith(t => t.Result.Select(x => new RecentClaimVM
                    {
                        Id          = x.Id,
                        ClaimNumber = $"{x.Year}-{x.Num}",
                        Plaintiff   = x.Plaintiff?.PlainitiffName ?? "-",
                        Branch      = x.Branch?.BranchName ?? "-",
                        Reserve     = x.Reserve,
                        Status      = x.ClaimStatus ?? "-",
                        RegDate     = x.RegDate
                    }).ToList()),

                MonthlyStats = GetMonthlyStats()
            };

            return View(vm);
        }

        private List<MonthlyStatVM> GetMonthlyStats()
        {
            var months = new[]
            {
                "يناير","فبراير","مارس","أبريل","مايو","يونيو",
                "يوليو","أغسطس","سبتمبر","أكتوبر","نوفمبر","ديسمبر"
            };

            var stats = new List<MonthlyStatVM>();
            for (int i = 5; i >= 0; i--)
            {
                var date  = DateTime.Now.AddMonths(-i);
                var month = date.Month;
                var yr    = date.Year;

                stats.Add(new MonthlyStatVM
                {
                    Month    = months[month - 1],
                    Courts   = _db.OutterClaims.Count(x => x.Del != true && x.RegDate.Month == month && x.RegDate.Year == yr),
                    Friendly = _db.FriendlyClaims.Count(x => x.Del != true && x.RegDate.Month == month && x.RegDate.Year == yr),
                    Outter   = 0
                });
            }
            return stats;
        }
    }
}
