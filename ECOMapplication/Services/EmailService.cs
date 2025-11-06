using System.Net;
using System.Net.Mail;

namespace ECOMapplication.Services
{
    public class EmailService
    {
        // Configuration property to access application settings.
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public Task SendEmailAsync(string ToEmail, string Subject, string Body, bool IsBodyHtml = false)
        {
            // Retrieve the data from the configuration.
            string? MailServer = _configuration["EmailSettings:MailServer"];
            string? FromEmail = _configuration["EmailSettings:FromEmail"];
            string? Password = _configuration["EmailSettings:Password"];
            string? SenderName = _configuration["EmailSettings:SenderName"];
            int Port = Convert.ToInt32(_configuration["EmailSettings:MailPort"]);

            // Create a new instance of SmtpClient using the mail server and port number.
            var client = new SmtpClient(MailServer, Port)
            {
                Credentials = new NetworkCredential(FromEmail, Password),
                EnableSsl = true,
            };

            MailAddress fromAddress = new MailAddress(FromEmail, SenderName);
            MailMessage mailMessage = new MailMessage
            {
                From = fromAddress, // Set the sender's email address with display name.
                Subject = Subject, // Set the email subject line.
                Body = Body, // Set the email body content.
                IsBodyHtml = IsBodyHtml // Specify whether the body content is in HTML format.
            };
            mailMessage.To.Add(ToEmail);
            return client.SendMailAsync(mailMessage);
        }
    }
}
