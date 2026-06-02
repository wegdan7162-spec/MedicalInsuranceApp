using System.ComponentModel.DataAnnotations;

namespace MedicalInsuranceApp1.Models.ViewModels
{
    public class RoleListVM
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public int UserCount { get; set; }
    }

    public class CreateRoleVM
    {
        [Required(ErrorMessage = "اسم الدور مطلوب")]
        [StringLength(50, ErrorMessage = "الاسم أقل من 50 حرف")]
        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;
    }

    public class EditRoleVM
    {
        public string Id { get; set; } = string.Empty;

        [Required(ErrorMessage = "اسم الدور مطلوب")]
        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }

    public class RoleUsersVM
    {
        public string RoleId { get; set; } = string.Empty;
        public string RoleName { get; set; } = string.Empty;
        public List<UserListVM> Users { get; set; } = new();
    }
}