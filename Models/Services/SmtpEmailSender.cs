using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace StockAssist.Web.Services
{
    public class SmtpEmailService : IEmailService
    {
        private readonly IConfiguration _config;
        private readonly ILogger<SmtpEmailService> _logger;

        public SmtpEmailService(IConfiguration config, ILogger<SmtpEmailService> logger)
        {
            _config = config;
            _logger = logger;
        }

        public async Task SendAsync(string to, string subject, string htmlBody)
        {
            var host = _config["Smtp:Host"];
            var port = _config.GetValue<int>("Smtp:Port");
            var from = _config["Smtp:From"];
            var user = _config["Smtp:User"];
            var pass = _config["Smtp:Pass"];
            var enableSsl = _config.GetValue<bool?>("Smtp:EnableSsl") ?? true;

            if (string.IsNullOrWhiteSpace(host) || string.IsNullOrWhiteSpace(from))
            {
                _logger.LogError("SMTP is not configured correctly. Host or From is empty.");
                return;
            }

            using var client = new SmtpClient(host, port)
            {
                EnableSsl = enableSsl,
                Credentials = new NetworkCredential(user, pass)
            };

            using var message = new MailMessage
            {
                From = new MailAddress(from),
                Subject = subject,
                Body = htmlBody,
                IsBodyHtml = true
            };

            message.To.Add(to);

            try
            {
                await client.SendMailAsync(message);
                _logger.LogInformation("Email sent to {Email}", to);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending email to {Email}", to);
            }
        }
    }
}