using EgyptianMuseum.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Mail;

namespace EgyptianMuseum.Application.Services.Email
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body, bool isHtml = true)
        {
            try
            {
                var smtpSettings = _configuration.GetSection("Smtp");
                var host = smtpSettings.GetValue<string>("Host");
                var port = smtpSettings.GetValue<int>("Port");
                var username = smtpSettings.GetValue<string>("Username");
                var password = smtpSettings.GetValue<string>("Password");
                var from = smtpSettings.GetValue<string>("From");

                if (string.IsNullOrWhiteSpace(host) || string.IsNullOrWhiteSpace(username) ||
                    string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(from))
                {
                    throw new InvalidOperationException("SMTP configuration is missing or incomplete");
                }

                using (var client = new SmtpClient(host, port))
                {
                    client.EnableSsl = true;
                    client.Credentials = new NetworkCredential(username, password);

                    var mailMessage = new MailMessage(from, toEmail)
                    {
                        From = new MailAddress(from, "MuseWay"),
                        Subject = subject,
                        Body = body,
                        IsBodyHtml = isHtml
                    };

                    await client.SendMailAsync(mailMessage);
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to send email: {ex.Message}", ex);
            }
        }
    }
}
