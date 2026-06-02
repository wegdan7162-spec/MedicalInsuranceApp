using MedicalInsuranceApp1.Models.Identity;
using Microsoft.AspNetCore.Identity;

namespace MedicalInsuranceApp1.Data
{
    public static class SeedData
    {
        public static async Task InitializeAsync(
            RoleManager<ApplicationRole> roleManager,
            UserManager<ApplicationUser> userManager)
        {
            // ===== الأدوار الأساسية =====
            var roles = new[]
            {
                new { Name = "SuperAdmin",  Desc = "صلاحية كاملة على النظام" },
                new { Name = "Admin",       Desc = "مدير النظام"             },
                new { Name = "Manager",     Desc = "مدير الإدارة"            },
                new { Name = "Supervisor",  Desc = "رئيس القسم"              },
                new { Name = "Employee",    Desc = "موظف"                    },
            };

            foreach (var r in roles)
            {
                if (!await roleManager.RoleExistsAsync(r.Name))
                {
                    await roleManager.CreateAsync(new ApplicationRole
                    {
                        Name = r.Name,
                        Description = r.Desc,
                        CreatedAt = DateTime.UtcNow,
                        IsActive = true
                    });
                }
            }

            // ===== SuperAdmin افتراضي =====
            const string adminUser = "admin";
            const string adminPass = "123456";

            if (await userManager.FindByNameAsync(adminUser) == null)
            {
                var admin = new ApplicationUser
                {
                    UserName = adminUser,
                    FullName = "مدير النظام",
                    UserJob = "SuperAdmin",
                    IsActive = true,
                    InsertP = true,
                    UpdateP = true,
                    PrintP = true,
                    UsersP = true,
                    SettingsP = true,
                    CreatedAt = DateTime.UtcNow,
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(admin, adminPass);
                if (result.Succeeded)
                    await userManager.AddToRoleAsync(admin, "SuperAdmin");
            }
        }
    }
}