using GodotXR.Application.Services;
using GodotXR.Infrastructure.Configurations;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

namespace GodotXR.Infrastructure.Core
{
    public class SmtpEmailService : IMailService
    {
        private readonly EmailOptions _options;

        public SmtpEmailService(IOptions<EmailOptions> options)
        {
            _options = options.Value;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            try
            {
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(_options.FromName, _options.FromEmail));
                message.To.Add(MailboxAddress.Parse(toEmail));
                message.Subject = subject;

                var bodyBuilder = new BodyBuilder
                {
                    HtmlBody = body
                };
                message.Body = bodyBuilder.ToMessageBody();

                using var client = new SmtpClient();
                client.ServerCertificateValidationCallback = (s, c, h, e) => true;
                await client.ConnectAsync("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
                await client.AuthenticateAsync(_options.FromEmail, _options.ApiKey);
                await client.SendAsync(message);
                await client.DisconnectAsync(true);
            }
            catch (Exception ex)
            {
                var keyMask = string.IsNullOrEmpty(_options.ApiKey) ? "EMPTY" : $"{_options.ApiKey[..Math.Min(3, _options.ApiKey.Length)]}***(len={_options.ApiKey.Length})";
                throw new Exception($"MailKit SMTP Send Error [From: '{_options.FromEmail}', Key: '{keyMask}']: {ex.Message}", ex);
            }
        }
    }
}
