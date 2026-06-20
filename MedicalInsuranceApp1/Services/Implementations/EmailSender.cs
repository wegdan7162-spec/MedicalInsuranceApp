using MedicalInsuranceApp1.Models.Interfaces;
using MedicalInsuranceApp1.Models.Settings;
using System.Net;
using System.Net.Mail;

namespace MedicalInsuranceApp1.Services.Implementations
{
    public class EmailSender : IEmailSender
    {
        private readonly MailSettings _mailSettings;

        public EmailSender(MailSettings mailSettings)
        {
            _mailSettings = mailSettings;
        }

        public async Task SendEmailAsync(EmailMessage message)
        {
            var mail = new MailMessage
            {
                From = new MailAddress(_mailSettings.UserName, _mailSettings.DisplayName),
                Subject = message.Subject,
                Body = message.Content,
                IsBodyHtml = true
            };

            foreach (var to in message.To)
                mail.To.Add(to);

            // المرفقات
            if (message.Attachments != null)
            {
                foreach (var file in message.Attachments)
                {
                    var stream = file.OpenReadStream();
                    mail.Attachments.Add(new Attachment(stream, file.FileName, file.ContentType));
                }
            }

            using var smtp = new SmtpClient(_mailSettings.Host, _mailSettings.Port)
            {
                Credentials = new NetworkCredential(_mailSettings.UserName, _mailSettings.Password),
                EnableSsl = _mailSettings.UseSSL
            };

            await smtp.SendMailAsync(mail);
        }
    }
}
