using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MedicalInsuranceApp1.Models.Identity;
using MedicalInsuranceApp1.Models.ViewModels;

namespace MedicalInsuranceApp1.Controllers
{
    [Authorize]
    public class RolesController : Controller
    {
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public RolesController(
            RoleManager<ApplicationRole> roleManager,
            UserManager<ApplicationUser> userManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }

        // ===== قائمة الأدوار =====
        public async Task<IActionResult> Index()
        {
            var roles = _roleManager.Roles.ToList();
            var vm = new List<RoleListVM>();

            foreach (var role in roles)
            {
                var users = await _userManager.GetUsersInRoleAsync(role.Name ?? "");
                vm.Add(new RoleListVM
                {
                    Id = role.Id,
                    Name = role.Name ?? "",
                    Description = role.Description ?? "",
                    IsActive = role.IsActive,
                    CreatedAt = role.CreatedAt,
                    UserCount = users.Count
                });
            }

            return View(vm);
        }

        // ===== إضافة دور =====
        [HttpGet]
        public IActionResult Create() => View(new CreateRoleVM());

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateRoleVM model)
        {
            if (!ModelState.IsValid) return View(model);

            if (await _roleManager.RoleExistsAsync(model.Name))
            {
                ModelState.AddModelError("Name", "هذا الدور موجود مسبقاً");
                return View(model);
            }

            var role = new ApplicationRole
            {
                Name = model.Name,
                Description = model.Description,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            var result = await _roleManager.CreateAsync(role);
            if (result.Succeeded)
            {
                TempData["Success"] = $"تم إنشاء الدور {model.Name} بنجاح";
                return RedirectToAction("Index");
            }

            foreach (var e in result.Errors)
                ModelState.AddModelError("", e.Description);

            return View(model);
        }

        // ===== تعديل دور =====
        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);
            if (role == null) return NotFound();

            return View(new EditRoleVM
            {
                Id = role.Id,
                Name = role.Name ?? "",
                Description = role.Description ?? "",
                IsActive = role.IsActive
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditRoleVM model)
        {
            if (!ModelState.IsValid) return View(model);

            var role = await _roleManager.FindByIdAsync(model.Id);
            if (role == null) return NotFound();

            role.Name = model.Name;
            role.Description = model.Description;
            role.IsActive = model.IsActive;

            var result = await _roleManager.UpdateAsync(role);
            if (result.Succeeded)
            {
                TempData["Success"] = "تم تحديث الدور بنجاح";
                return RedirectToAction("Index");
            }

            foreach (var e in result.Errors)
                ModelState.AddModelError("", e.Description);

            return View(model);
        }

        // ===== حذف دور =====
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);
            if (role == null) return NotFound();

            // تحقق إن ما فيه مستخدمين في هذا الدور
            var users = await _userManager.GetUsersInRoleAsync(role.Name ?? "");
            if (users.Any())
            {
                TempData["Error"] = $"لا يمكن حذف الدور — يوجد {users.Count} مستخدم مرتبط به";
                return RedirectToAction("Index");
            }

            await _roleManager.DeleteAsync(role);
            TempData["Success"] = "تم حذف الدور بنجاح";
            return RedirectToAction("Index");
        }

        // ===== مستخدمو الدور =====
        [HttpGet]
        public async Task<IActionResult> Users(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);
            if (role == null) return NotFound();

            var users = await _userManager.GetUsersInRoleAsync(role.Name ?? "");

            var vm = new RoleUsersVM
            {
                RoleId = role.Id,
                RoleName = role.Name ?? "",
                Users = users.Select(u => new UserListVM
                {
                    Id = u.Id,
                    UserName = u.UserName ?? "",
                    FullName = u.FullName,
                    UserJob = u.UserJob,
                    IsActive = u.IsActive
                }).ToList()
            };

            return View(vm);
        }
    }
}