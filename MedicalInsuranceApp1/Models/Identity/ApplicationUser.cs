using Microsoft.AspNetCore.Identity;

namespace MedicalInsuranceApp1.Models.Identity
{
    public class ApplicationUser : IdentityUser
    {
        // ===== بيانات من TblUsers القديمة =====
        public string FullName { get; set; } = string.Empty;
        public string UserJob { get; set; } = string.Empty;
        public byte[]? UserPic { get; set; }

        // ===== صلاحيات TblUsers القديمة (نحتفظ بها مرحلياً) =====
        public bool UpdateP { get; set; }
        public bool InsertP { get; set; }
        public bool PrintP { get; set; }
        public bool UsersP { get; set; }
        public bool SettingsP { get; set; }

        // ===== حقول جديدة =====
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? LastLoginAt { get; set; }
        public string? BranchId { get; set; }

        // ===== بيانات من TblUsers القديمة للمرجعية =====
        public int? OldUserId { get; set; }
    }
}