using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MedicalInsuranceApp1.Data;
using MedicalInsuranceApp1.Infrastrcture;
using MedicalInsuranceApp1.Models.Entities;
using MedicalInsuranceApp1.Models.ViewModels;
using MedicalInsuranceApp1.Models.ViewModels.Identity;

namespace MedicalInsuranceApp1.Controllers
{
    [Authorize(Roles = "Prog,Admin,SuperAdmin")]
    [RequirePermission(AppModules.Roles)]
    public class RolesController : Controller
    {
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly AppDbContext _db;

        public RolesController(
            RoleManager<ApplicationRole> roleManager,
            UserManager<ApplicationUser> userManager,
            AppDbContext db)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _db = db;
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

        // ===== تفاصيل الدور =====
        [HttpGet]
        public async Task<IActionResult> Details(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);
            if (role == null) return NotFound();

            var usersInRole  = await _userManager.GetUsersInRoleAsync(role.Name ?? "");
            var allUsers     = _userManager.Users.ToList();
            var availableUsers = allUsers
                .Where(u => usersInRole.All(r => r.Id != u.Id))
                .ToList();

            var existingPerms = await _db.RolePermissions
                .Where(p => p.RoleId == id)
                .ToListAsync();

            var permissions = AppModules.All.Select(m =>
            {
                var existing = existingPerms.FirstOrDefault(p => p.Module == m.Key);
                return new RolePermissionVM
                {
                    Id          = existing?.Id ?? 0,
                    RoleId      = id,
                    Module      = m.Key,
                    ModuleLabel = m.Label,
                    CanView     = existing?.CanView   ?? false,
                    CanAdd      = existing?.CanAdd    ?? false,
                    CanEdit     = existing?.CanEdit   ?? false,
                    CanDelete   = existing?.CanDelete ?? false,
                };
            }).ToList();

            var vm = new RoleDetailsVM
            {
                Id          = role.Id,
                Name        = role.Name ?? "",
                Description = role.Description ?? "",
                IsActive    = role.IsActive,
                CreatedAt   = role.CreatedAt,
                Users = usersInRole.Select(u => new UserListVM
                {
                    Id = u.Id, UserName = u.UserName ?? "",
                    FullName = u.FullName, UserJob = u.UserJob, IsActive = u.IsActive
                }).ToList(),
                AvailableUsers = availableUsers.Select(u => new UserListVM
                {
                    Id = u.Id, UserName = u.UserName ?? "",
                    FullName = u.FullName, UserJob = u.UserJob, IsActive = u.IsActive
                }).ToList(),
                Permissions = permissions
            };

            return View(vm);
        }

        // ===== حفظ الصلاحيات =====
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SavePermissions()
        {
            var form    = Request.Form;
            var roleId  = form["roleId"].ToString();
            var modules = form["modules"].ToList();

            var role = await _roleManager.FindByIdAsync(roleId);
            if (role == null) return NotFound();

            var existing = await _db.RolePermissions
                .Where(p => p.RoleId == roleId)
                .ToListAsync();

            // الـ checkboxes المحددة فقط تُرسل — نقرأها كـ keys
            var checkedViews   = form["canView_idx"].Select(int.Parse).ToHashSet();
            var checkedAdds    = form["canAdd_idx"].Select(int.Parse).ToHashSet();
            var checkedEdits   = form["canEdit_idx"].Select(int.Parse).ToHashSet();
            var checkedDeletes = form["canDelete_idx"].Select(int.Parse).ToHashSet();

            for (int i = 0; i < modules.Count; i++)
            {
                var module = modules[i];
                var perm   = existing.FirstOrDefault(p => p.Module == module);
                if (perm == null)
                {
                    perm = new RolePermission { RoleId = roleId, Module = module };
                    _db.RolePermissions.Add(perm);
                }
                perm.CanView   = checkedViews.Contains(i);
                perm.CanAdd    = checkedAdds.Contains(i);
                perm.CanEdit   = checkedEdits.Contains(i);
                perm.CanDelete = checkedDeletes.Contains(i);
            }

            await _db.SaveChangesAsync();
            TempData["Success"] = "تم حفظ الصلاحيات بنجاح";
            return RedirectToAction("Details", new { id = roleId });
        }

        // ===== تعيين مستخدم للدور =====
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignUser(string roleId, string userId)
        {
            var role = await _roleManager.FindByIdAsync(roleId);
            var user = await _userManager.FindByIdAsync(userId);
            if (role == null || user == null) return NotFound();

            if (!await _userManager.IsInRoleAsync(user, role.Name!))
                await _userManager.AddToRoleAsync(user, role.Name!);

            TempData["Success"] = $"تم تعيين {user.FullName} للدور {role.Name}";
            return RedirectToAction("Details", new { id = roleId });
        }

        // ===== إزالة مستخدم من الدور =====
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveUser(string roleId, string userId)
        {
            var role = await _roleManager.FindByIdAsync(roleId);
            var user = await _userManager.FindByIdAsync(userId);
            if (role == null || user == null) return NotFound();

            await _userManager.RemoveFromRoleAsync(user, role.Name!);

            TempData["Success"] = $"تم إزالة {user.FullName} من الدور {role.Name}";
            return RedirectToAction("Details", new { id = roleId });
        }
    }
}