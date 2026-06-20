using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MedicalInsuranceApp1.Models.ViewModels;
using MedicalInsuranceApp1.Infrastrcture;
using MedicalInsuranceApp1.Models.ViewModels.Identity;

namespace MedicalInsuranceApp1.Controllers
{
    [Authorize]
    [RequirePermission(AppModules.Users)]
    public class UsersController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;

        public UsersController(
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // ===== قائمة المستخدمين =====
        public async Task<IActionResult> Index()
        {
            var users = _userManager.Users
                .OrderByDescending(u => u.CreatedAt)
                .ToList();

            var vm = new List<UserListVM>();
            foreach (var u in users)
            {
                var roles = await _userManager.GetRolesAsync(u);
                vm.Add(new UserListVM
                {
                    Id = u.Id,
                    UserName = u.UserName ?? "",
                    FullName = u.FullName,
                    UserJob = u.UserJob,
                    IsActive = u.IsActive,
                    LastLoginAt = u.LastLoginAt,
                    CreatedAt = u.CreatedAt,
                    Roles = roles
                });
            }

            return View(vm);
        }

        // ===== إضافة مستخدم =====
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var vm = new CreateUserVM
            {
                AvailableRoles = _roleManager.Roles.Select(r => new RoleCheckVM
                {
                    RoleId = r.Id,
                    RoleName = r.Name ?? ""
                }).ToList()
            };
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateUserVM model)
        {
            if (!ModelState.IsValid)
            {
                model.AvailableRoles = _roleManager.Roles.Select(r => new RoleCheckVM
                {
                    RoleId = r.Id,
                    RoleName = r.Name ?? ""
                }).ToList();
                return View(model);
            }

            if (await _userManager.FindByNameAsync(model.UserName) != null)
            {
                ModelState.AddModelError("UserName", "اسم المستخدم موجود مسبقاً");
                model.AvailableRoles = _roleManager.Roles.Select(r => new RoleCheckVM
                {
                    RoleId = r.Id,
                    RoleName = r.Name ?? ""
                }).ToList();
                return View(model);
            }

            var user = new ApplicationUser
            {
                UserName = model.UserName,
                FullName = model.FullName,
                UserJob = model.UserJob,
                IsActive = model.IsActive,
                InsertP = model.InsertP,
                UpdateP = model.UpdateP,
                PrintP = model.PrintP,
                UsersP = model.UsersP,
                SettingsP = model.SettingsP,
                CreatedAt = DateTime.UtcNow,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                foreach (var e in result.Errors)
                    ModelState.AddModelError("", e.Description);
                model.AvailableRoles = _roleManager.Roles.Select(r => new RoleCheckVM
                {
                    RoleId = r.Id,
                    RoleName = r.Name ?? ""
                }).ToList();
                return View(model);
            }

            if (model.SelectedRoles != null && model.SelectedRoles.Any())
                await _userManager.AddToRolesAsync(user, model.SelectedRoles);

            TempData["Success"] = $"تم إنشاء المستخدم {model.UserName} بنجاح";
            return RedirectToAction("Index");
        }

        // ===== تعديل مستخدم =====
        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            var userRoles = await _userManager.GetRolesAsync(user);

            var vm = new EditUserVM
            {
                Id = user.Id,
                FullName = user.FullName,
                UserJob = user.UserJob,
                IsActive = user.IsActive,
                InsertP = user.InsertP,
                UpdateP = user.UpdateP,
                PrintP = user.PrintP,
                UsersP = user.UsersP,
                SettingsP = user.SettingsP,
                SelectedRoles = userRoles.ToList(),
                AvailableRoles = _roleManager.Roles.Select(r => new RoleCheckVM
                {
                    RoleId = r.Id,
                    RoleName = r.Name ?? "",
                    IsSelected = userRoles.Contains(r.Name ?? "")
                }).ToList()
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditUserVM model)
        {
            if (!ModelState.IsValid)
            {
                var userRoles2 = model.SelectedRoles ?? new List<string>();
                model.AvailableRoles = _roleManager.Roles.Select(r => new RoleCheckVM
                {
                    RoleId = r.Id,
                    RoleName = r.Name ?? "",
                    IsSelected = userRoles2.Contains(r.Name ?? "")
                }).ToList();
                return View(model);
            }

            var user = await _userManager.FindByIdAsync(model.Id);
            if (user == null) return NotFound();

            user.FullName = model.FullName;
            user.UserJob = model.UserJob;
            user.IsActive = model.IsActive;
            user.InsertP = model.InsertP;
            user.UpdateP = model.UpdateP;
            user.PrintP = model.PrintP;
            user.UsersP = model.UsersP;
            user.SettingsP = model.SettingsP;

            await _userManager.UpdateAsync(user);

            var currentRoles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, currentRoles);
            if (model.SelectedRoles != null && model.SelectedRoles.Any())
                await _userManager.AddToRolesAsync(user, model.SelectedRoles);

            TempData["Success"] = "تم تحديث بيانات المستخدم بنجاح";
            return RedirectToAction("Index");
        }

        // ===== تفعيل / تعطيل =====
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleActive(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            user.IsActive = !user.IsActive;
            await _userManager.UpdateAsync(user);

            TempData["Success"] = user.IsActive
                ? $"تم تفعيل حساب {user.UserName}"
                : $"تم تعطيل حساب {user.UserName}";

            return RedirectToAction("Index");
        }

        // ===== إعادة تعيين كلمة المرور =====
        [HttpGet]
        public async Task<IActionResult> ResetPassword(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            return View(new ResetPasswordVM
            {
                Id = user.Id,
                UserName = user.UserName ?? ""
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordVM model)
        {
            if (!ModelState.IsValid) return View(model);

            var user = await _userManager.FindByIdAsync(model.Id);
            if (user == null) return NotFound();

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(
                user, token, model.NewPassword);

            if (result.Succeeded)
            {
                TempData["Success"] = $"تم إعادة تعيين كلمة مرور {user.UserName}";
                return RedirectToAction("Index");
            }

            foreach (var e in result.Errors)
                ModelState.AddModelError("", e.Description);

            return View(model);
        }

        // ===== حذف مستخدم =====
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser?.Id == id)
            {
                TempData["Error"] = "لا يمكنك حذف حسابك الخاص";
                return RedirectToAction("Index");
            }

            await _userManager.DeleteAsync(user);
            TempData["Success"] = "تم حذف المستخدم بنجاح";
            return RedirectToAction("Index");
        }
    }
}
 