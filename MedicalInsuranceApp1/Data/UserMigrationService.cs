using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using MedicalInsuranceApp1.Models.ViewModels.Identity;

namespace MedicalInsuranceApp1.Data
{
    public class UserMigrationService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly IConfiguration _config;
        private readonly ILogger<UserMigrationService> _logger;

        public UserMigrationService(
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager,
            IConfiguration config,
            ILogger<UserMigrationService> logger)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _config = config;
            _logger = logger;
        }

        public async Task<MigrationResult> MigrateUsersAsync()
        {
            var result = new MigrationResult();
            var oldUsers = await GetOldUsersAsync();

            foreach (var old in oldUsers)
            {
                try
                {
                    // ← التعديل هنا: نمرر old.Id
                    var sanitizedName = SanitizeUserName(old.UserName, old.Id);

                    // تجاهل إذا نُقل مسبقاً
                    if (await _userManager.FindByNameAsync(sanitizedName) != null)
                    {
                        result.Skipped++;
                        continue;
                    }

                    var newUser = new ApplicationUser
                    {
                        UserName = sanitizedName,
                        FullName = old.UserName,
                        UserJob = old.UserJob ?? "Employee",
                        UserPic = old.UserPic,
                        InsertP = old.InsertP,
                        UpdateP = old.UpdateP,
                        PrintP = old.PrintP,
                        UsersP = old.UsersP,
                        SettingsP = old.SettingsP,
                        IsActive = true,
                        OldUserId = old.Id,
                        CreatedAt = DateTime.UtcNow,
                        EmailConfirmed = true
                    };

                    var tempPassword = "123456";
                    var createResult = await _userManager.CreateAsync(newUser, tempPassword);

                    if (createResult.Succeeded)
                    {
                        var role = DetermineRole(old);
                        await _userManager.AddToRoleAsync(newUser, role);

                        result.Succeeded++;
                        result.Details.Add(new MigrationDetail
                        {
                            OldId = old.Id,
                            UserName = sanitizedName,
                            FullName = old.UserName,
                            AssignedRole = role,
                            TempPassword = tempPassword,
                            Status = "✅ نجح"
                        });

                        _logger.LogInformation(
                            "نُقل المستخدم {UserName} بنجاح → Role: {Role}",
                            sanitizedName, role);
                    }
                    else
                    {
                        result.Failed++;
                        var errors = string.Join(", ",
                            createResult.Errors.Select(e => e.Description));
                        result.Details.Add(new MigrationDetail
                        {
                            OldId = old.Id,
                            UserName = sanitizedName,
                            FullName = old.UserName,
                            Status = $"❌ فشل: {errors}"
                        });
                    }
                }
                catch (Exception ex)
                {
                    result.Failed++;
                    _logger.LogError(ex, "خطأ في نقل المستخدم {UserName}", old.UserName);
                }
            }

            return result;
        }

        // ===== تحديد الدور =====
        private string DetermineRole(OldUser user)
        {
            if (user.UsersP && user.SettingsP) return "Admin";
            if (user.UsersP) return "Supervisor";
            if (user.UpdateP && user.InsertP) return "Manager";
            if (user.InsertP) return "Employee";
            return "Employee";
        }

        // ===== جلب المستخدمين من DB القديمة =====
        private async Task<List<OldUser>> GetOldUsersAsync()
        {
            var users = new List<OldUser>();
            var connStr = _config.GetConnectionString("MedicalInsuranceDB");

            await using var conn = new SqlConnection(connStr);
            await conn.OpenAsync();

            var cmd = new SqlCommand(
                "SELECT Id, UserName, UserPassword, UserJob, UserPic, " +
                "UpdateP, InsertP, PrintP, UsersP, SettingsP FROM TblUsers",
                conn);

            await using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                users.Add(new OldUser
                {
                    Id = reader.GetInt32(0),
                    UserName = reader.GetString(1),
                    UserPassword = reader.GetString(2),
                    UserJob = reader.IsDBNull(3) ? null : reader.GetString(3),
                    UserPic = reader.IsDBNull(4) ? null : (byte[])reader[4],
                    UpdateP = reader.GetBoolean(5),
                    InsertP = reader.GetBoolean(6),
                    PrintP = reader.GetBoolean(7),
                    UsersP = reader.GetBoolean(8),
                    SettingsP = reader.GetBoolean(9),
                });
            }

            return users;
        }

        // ===== تنظيف اسم المستخدم ← التعديل هنا =====
        private string SanitizeUserName(string userName, int oldId)
        {
            // إذا فيه أحرف عربية → استخدم User_ID
            bool hasArabic = userName.Any(c => c >= '\u0600' && c <= '\u06FF');
            if (hasArabic)
                return $"User_{oldId}";

            // إذا إنجليزي → نظفه
            var sanitized = userName.Trim().Replace(" ", "_");
            sanitized = System.Text.RegularExpressions.Regex.Replace(
                sanitized, @"[^\w\-.]", "_");
            return sanitized;
        }
    }

    // ===== Models المساعدة =====
    public class OldUser
    {
        public int Id { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string UserPassword { get; set; } = string.Empty;
        public string? UserJob { get; set; }
        public byte[]? UserPic { get; set; }
        public bool UpdateP { get; set; }
        public bool InsertP { get; set; }
        public bool PrintP { get; set; }
        public bool UsersP { get; set; }
        public bool SettingsP { get; set; }
    }

    public class MigrationResult
    {
        public int Succeeded { get; set; }
        public int Failed { get; set; }
        public int Skipped { get; set; }
        public List<MigrationDetail> Details { get; set; } = new();
    }

    public class MigrationDetail
    {
        public int OldId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string AssignedRole { get; set; } = string.Empty;
        public string TempPassword { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
    }
}