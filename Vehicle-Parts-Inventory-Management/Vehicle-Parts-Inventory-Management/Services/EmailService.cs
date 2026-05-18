using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using Vehicle_Parts_Inventory_Management.Interfaces;
using Vehicle_Parts_Inventory_Management.Models;

namespace Vehicle_Parts_Inventory_Management.Services
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _settings;

        public EmailService(IOptions<EmailSettings> settings)
        {
            _settings = settings.Value;
        }

        public async Task SendAsync(string toEmail, string subject, string body)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_settings.FromName, _settings.FromEmail));
            message.To.Add(MailboxAddress.Parse(toEmail));
            message.Subject = subject;

            message.Body = new TextPart("plain") { Text = body };

            using var smtp = new SmtpClient();

            var secureOption = _settings.UseStartTls ? SecureSocketOptions.StartTls : SecureSocketOptions.Auto;

            await smtp.ConnectAsync(_settings.Host, _settings.Port, secureOption);
            await smtp.AuthenticateAsync(_settings.Username, _settings.Password);
            await smtp.SendAsync(message);
            await smtp.DisconnectAsync(true);
        }
    }
}