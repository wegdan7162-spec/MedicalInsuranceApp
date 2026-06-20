using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MedicalInsuranceApp1.Data;
using MedicalInsuranceApp1.Models.Entities;

namespace MedicalInsuranceApp1.Controllers
{
    [Authorize]
    public class ActivityController : Controller
    {
        private readonly AppDbContext _db;
        public ActivityController(AppDbContext db) => _db = db;

        public async Task<IActionResult> Index(string? user, DateTime? from, DateTime? to)
        {
            var sessionsQ  = _db.UserSessions.AsQueryable();
            var activitiesQ = _db.UserActivities.AsQueryable();

            if (!string.IsNullOrEmpty(user))
            {
                sessionsQ   = sessionsQ.Where(x => x.UserName == user || x.FullName.Contains(user));
                activitiesQ = activitiesQ.Where(x => x.UserName == user || x.FullName.Contains(user));
            }
            if (from.HasValue)
            {
                sessionsQ   = sessionsQ.Where(x => x.LoginAt >= from.Value);
                activitiesQ = activitiesQ.Where(x => x.VisitedAt >= from.Value);
            }
            if (to.HasValue)
            {
                var toEnd = to.Value.AddDays(1);
                sessionsQ   = sessionsQ.Where(x => x.LoginAt < toEnd);
                activitiesQ = activitiesQ.Where(x => x.VisitedAt < toEnd);
            }

            var rawActivities = await activitiesQ
                .OrderBy(x => x.UserId)
                .ThenBy(x => x.VisitedAt)
                .Take(500)
                .ToListAsync();

            // احسب المدة لكل زيارة بالفرق مع الزيارة التالية لنفس المستخدم
            var activitiesWithDuration = rawActivities
                .Select((a, idx) =>
                {
                    // الزيارة التالية لنفس المستخدم
                    var next = rawActivities
                        .Skip(idx + 1)
                        .FirstOrDefault(n => n.UserId == a.UserId);

                    int? durationSec = null;
                    if (next != null)
                    {
                        var diff = (next.VisitedAt - a.VisitedAt).TotalSeconds;
                        // نتجاهل فترات طويلة جداً (أكثر من 30 دقيقة = خرج أو غير نشط)
                        if (diff > 0 && diff <= 1800)
                            durationSec = (int)diff;
                    }

                    return new ActivityWithDuration
                    {
                        Activity    = a,
                        DurationSec = durationSec
                    };
                })
                .OrderByDescending(x => x.Activity.VisitedAt)
                .Take(200)
                .ToList();

            ViewBag.Sessions   = await sessionsQ.OrderByDescending(x => x.LoginAt).Take(100).ToListAsync();
            ViewBag.Activities = activitiesWithDuration;
            ViewBag.User = user;
            ViewBag.From = from?.ToString("yyyy-MM-dd");
            ViewBag.To   = to?.ToString("yyyy-MM-dd");
            ViewBag.Users = await _db.UserSessions.Select(x => x.UserName).Distinct().ToListAsync();

            return View();
        }
    }

    public class ActivityWithDuration
    {
        public UserActivity Activity    { get; set; } = null!;
        public int?         DurationSec { get; set; }
    }
}
