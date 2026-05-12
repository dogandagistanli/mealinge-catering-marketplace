using System.Net;
using System.Net.Mail;
using Ceng382_25_26_202311031.Data;
using Ceng382_25_26_202311031.Models;

namespace Ceng382_25_26_202311031.Services
{
    public class EmailService
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public EmailService(
            ApplicationDbContext context,
            IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task SendAsync(
            string recipientEmail,
            string subject,
            string body)
        {
            var senderEmail = _configuration["EmailSettings:SenderEmail"];
            var senderPassword = _configuration["EmailSettings:SenderPassword"];

            using var client = new SmtpClient("smtp.gmail.com", 587)
            {
                Credentials = new NetworkCredential(
                    senderEmail,
                    senderPassword),

                EnableSsl = true
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(senderEmail!),
                Subject = subject,
                Body = body,
                IsBodyHtml = false
            };

            mailMessage.To.Add(recipientEmail);

            await client.SendMailAsync(mailMessage);

            _context.EmailRecords.Add(new EmailRecord
            {
                RecipientEmail = recipientEmail,
                Subject = subject,
                Body = body,
                SentAt = DateTime.Now
            });

            await _context.SaveChangesAsync();
        }
    }
}