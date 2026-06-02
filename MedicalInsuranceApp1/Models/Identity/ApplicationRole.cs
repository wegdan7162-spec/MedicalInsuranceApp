using Microsoft.AspNetCore.Identity;

namespace MedicalInsuranceApp1.Models.Identity
{
    public class ApplicationRole : IdentityRole
    {
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsActive { get; set; } = true;
    }
}