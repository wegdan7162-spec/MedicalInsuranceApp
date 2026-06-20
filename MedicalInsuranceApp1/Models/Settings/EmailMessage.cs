namespace MedicalInsuranceApp1.Models.Settings
{
    public class EmailMessage
    {
        public IEnumerable<string> To { get; set; }
        public string Subject { get; set; }
        public string Content { get; set; }
        public IFormFileCollection? Attachments { get; set; }

        public EmailMessage(IEnumerable<string> to, string subject, string content, IFormFileCollection? attachments = null)
        {
            To = to;
            Subject = subject;
            Content = content;
            Attachments = attachments;
        }
    }
}
