using MedicalInsuranceApp1.Infrastrcture;
using MedicalInsuranceApp1.Models.Entities;
using MedicalInsuranceApp1.Models.Interfaces;
using MedicalInsuranceApp1.Models.Settings;
using MedicalInsuranceApp1.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MedicalInsuranceApp1.Controllers
{
    [ViewLayout("_LayoutDashboard")]
    [Authorize(Roles = "Prog,Admin,SuperAdmin")]
    public class MailController : Controller
    {


        private readonly MailSettings _mailSettings;
        private readonly IWebHostEnvironment _host;
        private readonly IEmailSender _emailSender;

        public MailController(MailSettings mailSettings,
                              IWebHostEnvironment host,
                              IEmailSender emailSender)
        {
            _mailSettings = mailSettings;
            _host = host;
            _emailSender = emailSender;
        }

        [HttpGet]
        public ActionResult EmailSettings()
        {
            var mailSettings = _mailSettings;
            if (mailSettings == null)
            {
                return View("Notfound");
            }

            return View(mailSettings);
        }


        [HttpGet]
        public IActionResult SendEmail()
        {
            var emailVM = new EmailVM
            {
                MailSettings = _mailSettings
            };

            return View(emailVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SendEmail(EmailVM emailVM)
        {
            //-------------------------تجهيز التمبلت فقط--------------------------------
            var filePath = _host.WebRootPath + "\\templates" + "\\Email.html";
            StreamReader htmlFile = new StreamReader(filePath);
            string content = htmlFile.ReadToEnd(); // string content1 = "<!DOCTYPE html>\n <html>";
            htmlFile.Close();

            //تم استعماله مرتين: مرة ضمن الرسالة ومرة اخرى في عنوان الايميل ولكنه نفس العنوان// Subject
            content = content.Replace("{Subject}", emailVM.Subject); // يظهر داخل الرسالة
            content = content.Replace("{Content}", emailVM.Content);

            //var domain = $"{Request.Scheme}://{Request.Host.Host}";
            //content = content.Replace("{domain}", domain);
            //---------------------------------------------------------

            var message = new EmailMessage(new string[] { emailVM.To }, emailVM.Subject, content, emailVM.Attachments);

            try
            {
                await _emailSender.SendEmailAsync(message);
                TempData["SuccessMessage"] = "The email has been sent successfully";
            }
            catch
            {
                ViewBag.errorMessage = "Failed to send email";
                TempData["ErrorMessage"] = "Failed to send email";
            }

            emailVM.MailSettings = _mailSettings;
            return View(emailVM);

        }
    }
}
