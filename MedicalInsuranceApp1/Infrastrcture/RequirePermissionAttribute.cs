using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Identity;
using MedicalInsuranceApp1.Models.ViewModels.Identity;

namespace MedicalInsuranceApp1.Infrastrcture
{
    /// <summary>
    /// يحمي الـ Action أو الـ Controller — يتحقق أن المستخدم يملك الصلاحية المطلوبة.
    /// المثال: [RequirePermission("OutterClaims", "View")]
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public class RequirePermissionAttribute : Attribute, IAsyncAuthorizationFilter
    {
        private readonly string _module;
        private readonly string _action;

        public RequirePermissionAttribute(string module, string action = "View")
        {
            _module = module;
            _action = action;
        }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            var user = context.HttpContext.User;
            if (user?.Identity?.IsAuthenticated != true)
            {
                context.Result = new RedirectToActionResult("Login", "Account", null);
                return;
            }

            var permService = context.HttpContext.RequestServices
                .GetRequiredService<IPermissionService>();
            var userManager = context.HttpContext.RequestServices
                .GetRequiredService<UserManager<ApplicationUser>>();

            var appUser = await userManager.GetUserAsync(user);
            if (appUser == null || !appUser.IsActive)
            {
                context.Result = new RedirectToActionResult("Login", "Account", null);
                return;
            }

            var hasPermission = await permService.HasPermissionAsync(appUser.Id, _module, _action);
            if (!hasPermission)
            {
                context.Result = new RedirectToActionResult("AccessDenied", "Account", null);
            }
        }
    }
}
