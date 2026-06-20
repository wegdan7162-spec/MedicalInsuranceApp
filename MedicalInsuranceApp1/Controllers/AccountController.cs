using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MedicalInsuranceApp1.Data;
using MedicalInsuranceApp1.Models.Entities;
using MedicalInsuranceApp1.Models.ViewModels;
using Microsoft.EntityFrameworkCore;
using MedicalInsuranceApp1.Models.ViewModels.Identity;

namespace MedicalInsuranceApp1.Controllers
{
    [AllowAnonymous]
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<AccountController> _logger;
        private readonly AppDbContext _db;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ILogger<AccountController> logger,
            AppDbContext db)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _db = db;
        }

        // ===== GET: Login =====
        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            if (User.Identity?.IsAuthenticated == true)
                return RedirectToAction("Index", "Dashboard");

            return View(new LoginVM { ReturnUrl = returnUrl });
        }

        // ===== POST: Login =====
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginVM model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.FindByNameAsync(model.UserName);

            if (user == null)
            {
                ModelState.AddModelError("", "المستخدم غير موجود");
                return View(model);
            }

            if (!user.IsActive)
            {
                ModelState.AddModelError("", "حسابك معطل. تواصل مع المسؤول.");
                return View(model);
            }

            var passwordOk = await _userManager.CheckPasswordAsync(user, model.Password);
            if (!passwordOk)
            {
                ModelState.AddModelError("", "كلمة المرور غير صحيحة");
                return View(model);
            }

            var result = await _signInManager.PasswordSignInAsync(
                model.UserName,
                model.Password,
                model.RememberMe,
                lockoutOnFailure: true);

            if (result.Succeeded)
            {
                user.LastLoginAt = DateTime.UtcNow;
                await _userManager.UpdateAsync(user);

                // تسجيل الجلسة
                var session = new UserSession
                {
                    UserId    = user.Id,
                    UserName  = user.UserName ?? "",
                    FullName  = user.FullName,
                    LoginAt   = DateTime.Now,
                    IPAddress = HttpContext.Connection.RemoteIpAddress?.ToString()
                };
                _db.UserSessions.Add(session);
                await _db.SaveChangesAsync();
                HttpContext.Session.SetInt32("SessionId", session.Id);

                if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
                    return Redirect(model.ReturnUrl);

                return RedirectToAction("Index", "Dashboard");
            }

            if (result.IsLockedOut)
            {
                ModelState.AddModelError("", "تم قفل حسابك. حاولي بعد 15 دقيقة.");
                return View(model);
            }

            if (result.IsNotAllowed)
            {
                ModelState.AddModelError("", "غير مسموح بتسجيل الدخول.");
                return View(model);
            }

            ModelState.AddModelError("", "حدث خطأ. حاولي مرة أخرى.");
            return View(model);
        }

        // ===== POST: Logout =====
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            // تسجيل وقت الخروج ومدة الجلسة
            var sessionId = HttpContext.Session.GetInt32("SessionId");
            if (sessionId.HasValue)
            {
                var session = await _db.UserSessions.FindAsync(sessionId.Value);
                if (session != null)
                {
                    session.LogoutAt = DateTime.Now;
                    session.DurationMinutes = (int)(DateTime.Now - session.LoginAt).TotalMinutes;
                    await _db.SaveChangesAsync();
                }
            }
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login");
        }

        // ===== GET: AccessDenied =====
        [HttpGet]
        public IActionResult AccessDenied() => View();

        // ===== GET: ChangePassword =====
        [HttpGet]
        public IActionResult ChangePassword() => View(new ChangePasswordVM());

        // ===== POST: ChangePassword =====
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordVM model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("Login");

            var result = await _userManager.ChangePasswordAsync(
                user, model.CurrentPassword, model.NewPassword);

            if (result.Succeeded)
            {
                await _signInManager.RefreshSignInAsync(user);
                TempData["Success"] = "تم تغيير كلمة المرور بنجاح";
                return RedirectToAction("Index", "Dashboard");
            }

            foreach (var error in result.Errors)
                ModelState.AddModelError("", error.Description);

            return View(model);
        }

        // ===== GET: Profile =====
        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("Login");

            var roles = await _userManager.GetRolesAsync(user);

            var vm = new UserProfileVM
            {
                Id = user.Id,
                UserName = user.UserName ?? "",
                FullName = user.FullName,
                UserJob = user.UserJob,
                IsActive = user.IsActive,
                LastLoginAt = user.LastLoginAt,
                CreatedAt = user.CreatedAt,
                Roles = roles
            };

            return View(vm);
        }
    }
}