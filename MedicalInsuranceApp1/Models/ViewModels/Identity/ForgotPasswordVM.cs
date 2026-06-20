using System.ComponentModel.DataAnnotations;

namespace MedicalInsuranceApp1.ViewModels.Identity
{
    public class ForgotPasswordVM
    {
        [EmailAddress]
        public required string Email { get; set; }

    }
}
