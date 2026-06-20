using MedicalInsuranceApp1.Models.Entities;
using MedicalInsuranceApp1.Models.ViewModels.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace MedicalInsuranceApp1.Data
{
    public static class SeedData
    {
        public static async Task InitializeAsync(
            RoleManager<ApplicationRole> roleManager,
            UserManager<ApplicationUser> userManager,
            AppDbContext db)
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

            // ===== أنواع المصروفات الافتراضية =====
            var defaultExpenseTypes = new[]
            {
                "اتعاب إدارة القضايا",
                "مصاريف أخرى",
                "اتعاب المحاماة",
                "مصاريف اللجنة"
            };

            foreach (var typeName in defaultExpenseTypes)
            {
                if (!await db.ExpenseTypes.AnyAsync(e => e.ExpenseName == typeName))
                {
                    db.ExpenseTypes.Add(new ExpenseType { ExpenseName = typeName });
                }
            }

            await db.SaveChangesAsync();
        }
    }
}