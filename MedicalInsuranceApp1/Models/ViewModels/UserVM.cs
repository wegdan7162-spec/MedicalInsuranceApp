using System.ComponentModel.DataAnnotations;

namespace MedicalInsuranceApp1.Models.ViewModels
{
    public class UserListVM
    {
        public string Id { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string UserJob { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DateTime? LastLoginAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public IList<string> Roles { get; set; } = new List<string>();
    }

    public class CreateUserVM
    {
        [Required(ErrorMessage = "اسم المستخدم مطلوب")]
        [StringLength(50, ErrorMessage = "الاسم أقل من 50 حرف")]
        public string UserName { get; set; } = string.Empty;

        [Required(ErrorMessage = "الاسم الكامل مطلوب")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "المسمى الوظيفي مطلوب")]
        public string UserJob { get; set; } = string.Empty;

        [Required(ErrorMessage = "كلمة المرور مطلوبة")]
        [MinLength(6, ErrorMessage = "كلمة المرور 6 أحرف على الأقل")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "تأكيد كلمة المرور مطلوب")]
        [Compare("Password", ErrorMessage = "كلمات المرور غير متطابقة")]
        public string ConfirmPassword { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;

        // الصلاحيات القديمة
        public bool InsertP { get; set; }
        public bool UpdateP { get; set; }
        public bool PrintP { get; set; }
        public bool UsersP { get; set; }
        public bool SettingsP { get; set; }

        // الأدوار
        public List<string> SelectedRoles { get; set; } = new();
        public List<RoleCheckVM> AvailableRoles { get; set; } = new();
    }

    public class EditUserVM
    {
        public string Id { get; set; } = string.Empty;

        [Required(ErrorMessage = "الاسم الكامل مطلوب")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "المسمى الوظيفي مطلوب")]
        public string UserJob { get; set; } = string.Empty;

        public bool IsActive { get; set; }
        public bool InsertP { get; set; }
        public bool UpdateP { get; set; }
        public bool PrintP { get; set; }
        public bool UsersP { get; set; }
        public bool SettingsP { get; set; }

        public List<string> SelectedRoles { get; set; } = new();
        public List<RoleCheckVM> AvailableRoles { get; set; } = new();
    }

    public class RoleCheckVM
    {
        public string RoleId { get; set; } = string.Empty;
        public string RoleName { get; set; } = string.Empty;
        public bool IsSelected { get; set; }
    }

    public class ResetPasswordVM
    {
        public string Id { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;

        [Required(ErrorMessage = "كلمة المرور الجديدة مطلوبة")]
        [MinLength(6, ErrorMessage = "كلمة المرور 6 أحرف على الأقل")]
        public string NewPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "تأكيد كلمة المرور مطلوب")]
        [Compare("NewPassword", ErrorMessage = "كلمات المرور غير متطابقة")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}