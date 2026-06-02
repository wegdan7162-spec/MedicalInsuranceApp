using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MedicalInsuranceApp1.Data;
using MedicalInsuranceApp1.Models.Identity;
using MedicalInsuranceApp1.Models.ViewModels;

namespace MedicalInsuranceApp1.Controllers
{
    [Authorize]
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
                TotalCourtCases = await _db.OutterClaims.CountAsync(),
                TotalFriendly   = await _db.FriendlyClaims.CountAsync(),

                TotalReserve =
                    (await _db.OutterClaims.SumAsync(x => (double?)x.Reserve)    ?? 0) +
                    (await _db.FriendlyClaims.SumAsync(x => (double?)x.Reserve)  ?? 0),

                TotalSettled =
                    (await _db.OutterClaims.SumAsync(x => (double?)x.Setteld)    ?? 0) +
                    (await _db.FriendlyClaims.SumAsync(x => (double?)x.Setteld)  ?? 0),

                TotalUsers  = _userManager.Users.Count(),
                ActiveUsers = _userManager.Users.Count(x => x.IsActive),

                RecentClaims = await _db.OutterClaims
                    .Include(x => x.Branch)
                    .Include(x => x.Plaintiff)
                    .OrderByDescending(x => x.RegDate)
                    .Take(5)
                    .ToListAsync()
                    .ContinueWith(t => t.Result.Select(x => new RecentClaimVM
                    {
                        Id          = x.Id,
                        ClaimNumber = $"{x.Year}-{x.Num:D4}",
                        ClaimType   = "قضية",
                        Plaintiff   = x.Plaintiff?.PlainitiffName ?? "-",
                        Branch      = x.Branch?.BranchName ?? "-",
                        Reserve     = x.Reserve,
                        Status      = x.ClaimStatus ?? "تحت التسوية",
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
                    Courts   = _db.OutterClaims.Count(x => x.RegDate.Month == month && x.RegDate.Year == yr),
                    Friendly = _db.FriendlyClaims.Count(x => x.RegDate.Month == month && x.RegDate.Year == yr),
                    Outter   = 0
                });
            }
            return stats;
        }
    }
}
