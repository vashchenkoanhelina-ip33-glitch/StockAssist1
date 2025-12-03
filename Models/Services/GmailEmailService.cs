using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;

namespace StockAssist.Web.Services
{
    public class GmailEmailService : IEmailService
    {
        private readonly IConfiguration _cfg;

        public GmailEmailService(IConfiguration cfg)
        {
            _cfg = cfg;
        }

        public async Task SendAsync(string to, string subject, string html)
        {
            var user = _cfg["Smtp:Gmail:User"];
            var pass = _cfg["Smtp:Gmail:AppPassword"];

            if (string.IsNullOrWhiteSpace(user) || string.IsNullOrWhiteSpace(pass))
                throw new InvalidOperationException("SMTP Gmail credentials are not configured. Check appsettings.json (Smtp:Gmail).");

            using var client = new SmtpClient("smtp.gmail.com", 587)
            {
                EnableSsl = true,
                Credentials = new NetworkCredential(user, pass)
            };

            using var message = new MailMessage(user, to, subject, html)
            {
                IsBodyHtml = true
            };

            await client.SendMailAsync(message);
        }
    }
}