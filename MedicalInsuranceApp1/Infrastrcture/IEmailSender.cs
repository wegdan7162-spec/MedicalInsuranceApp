using MedicalInsuranceApp1.Models.Settings;

namespace MedicalInsuranceApp1.Models.Interfaces
{
    public interface IEmailSender
    {
        Task SendEmailAsync(EmailMessage message);
    }
}
