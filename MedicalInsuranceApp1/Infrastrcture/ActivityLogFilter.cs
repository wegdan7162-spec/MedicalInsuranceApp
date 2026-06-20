using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Identity;
using MedicalInsuranceApp1.Data;
using MedicalInsuranceApp1.Models.Entities;
using MedicalInsuranceApp1.Models.ViewModels.Identity;

namespace MedicalInsuranceApp1.Infrastrcture
{
    public class ActivityLogFilter : IAsyncActionFilter
    {
        private readonly AppDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;

        public ActivityLogFilter(AppDbContext db, UserManager<ApplicationUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            await next();

            try
            {
                if (!context.HttpContext.User.Identity?.IsAuthenticated ?? true) return;

                var user = await _userManager.GetUserAsync(context.HttpContext.User);
                if (user == null) return;

                var routeData = context.RouteData;
                var controller = routeData.Values["controller"]?.ToString() ?? "";
                var action     = routeData.Values["action"]?.ToString() ?? "";

                // تجاهل الأكشنز الثانوية (POST, Delete, إلخ)
                if (context.HttpContext.Request.Method != "GET") return;

                // تجاهل صفحات معينة
                var skipControllers = new[] { "Account" };
                if (skipControllers.Contains(controller)) return;

                var pageTitle = controller switch
                {
                    "Dashboard"     => "لوحة التحكم",
                    "OutterClaims"  => action == "Inventory" ? "" : "مطالبات القضايا",
                    "FriendlyClaims"=> "الصلح الودي",
                    "Users"         => "المستخدمين",
                    "Roles"         => "الأدوار والصلاحيات",
                    "Branches"      => "الفروع",
                    "Courts"        => "المحاكم",
                    "Plaintiffs"    => "المدعين",
                    "Trash"         => $"سلة المحذوفات - {action}",
                    "Activity"      => "حركة المستخدمين",
                    _               => controller
                };

                _db.UserActivities.Add(new UserActivity
                {
                    UserId     = user.Id,
                    UserName   = user.UserName ?? "",
                    FullName   = user.FullName,
                    Controller = controller,
                    Action     = action,
                    PageTitle  = pageTitle,
                    IPAddress  = context.HttpContext.Connection.RemoteIpAddress?.ToString(),
                    VisitedAt  = DateTime.Now
                });
                await _db.SaveChangesAsync();
            }
            catch { /* لا توقف التطبيق بسبب خطأ في اللوج */ }
        }
    }
}
