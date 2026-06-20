using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace MedicalInsuranceApp1.Models.ViewModels.Identity
{
    public class ApplicationUser : IdentityUser
    {
        [Range(18, 100)]
        
        public DateTime? LastAccessTime { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime? ModifiedDate { get; set; }
        public bool? Approval { get; set; }


        [ValidateNever]
        public UserProfile? UserProfile { get; set; }

        [ValidateNever]
        public Employee? Employee { get; set; }

        //public DateTime? LastAccessTimeLocal => LastAccessTime?.ToLocalTime();

        //public DateTime CreatedDateLocal => CreatedDate.ToLocalTime();

        public DateTime? ModifiedDateLocal => ModifiedDate?.ToLocalTime();


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