namespace WCABNetwork.Cab.IdentityService.Models.Dtos.Mail
{
    public class SendEmailConfig
    {
        public SendEmailConfig(string fromName, string fromEmail, string toName, string toEmail, string subject, string body)
        {
            FromName = fromName;
            FromEmail = fromEmail;
            ToName = toName;
            ToEmail = toEmail;
            Subject = subject;
            Body = body;
        }

        public string FromName { get; set; }
        public string FromEmail { get; set; }
        public string ToName { get; set; }
        public string ToEmail { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
    }
}
