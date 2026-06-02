using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MedicalInsuranceApp1.Data;

namespace MedicalInsuranceApp1.Controllers
{
    [Authorize]
    public class ActivityController : Controller
    {
        private readonly AppDbContext _db;
        public ActivityController(AppDbContext db) => _db = db;

        public async Task<IActionResult> Index(string? user, DateTime? from, DateTime? to)
        {
            var sessions = _db.UserSessions.AsQueryable();
            var activities = _db.UserActivities.AsQueryable();

            if (!string.IsNullOrEmpty(user))
            {
                sessions    = sessions.Where(x => x.UserName == user || x.FullName.Contains(user));
                activities  = activities.Where(x => x.UserName == user || x.FullName.Contains(user));
            }
            if (from.HasValue)
            {
                sessions   = sessions.Where(x => x.LoginAt >= from.Value);
                activities = activities.Where(x => x.VisitedAt >= from.Value);
            }
            if (to.HasValue)
            {
                var toEnd = to.Value.AddDays(1);
                sessions   = sessions.Where(x => x.LoginAt < toEnd);
                activities = activities.Where(x => x.VisitedAt < toEnd);
            }

            ViewBag.Sessions   = await sessions.OrderByDescending(x => x.LoginAt).Take(100).ToListAsync();
            ViewBag.Activities = await activities.OrderByDescending(x => x.VisitedAt).Take(200).ToListAsync();
            ViewBag.User = user;
            ViewBag.From = from?.ToString("yyyy-MM-dd");
            ViewBag.To   = to?.ToString("yyyy-MM-dd");
            ViewBag.Users = await _db.UserSessions.Select(x => x.UserName).Distinct().ToListAsync();

            return View();
        }
    }
}
