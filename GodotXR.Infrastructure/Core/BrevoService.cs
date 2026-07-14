using GodotXR.Application.Services;
using GodotXR.Infrastructure.Configurations;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;

namespace GodotXR.Infrastructure.Core
{
    public class BrevoEmailService : IMailService
    {
        private readonly HttpClient _httpClient;
        private readonly EmailOptions _options;

        public BrevoEmailService(
            HttpClient httpClient,
            IOptions<EmailOptions> options)
        {
            _httpClient = httpClient;
            _options = options.Value;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            var payload = new
            {
                sender = new
                {
                    name = _options.FromName,
                    email = _options.FromEmail
                },
                to = new[]
                {
                new
                {
                    email = toEmail
                }
            },
                subject,
                htmlContent = body
            };

            using var request = new HttpRequestMessage(HttpMethod.Post, "v3/smtp/email");

            request.Headers.Add("api-key", _options.ApiKey);

            request.Content = JsonContent.Create(payload);

            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();

                throw new Exception(
                    $"Brevo send failed. Status: {response.StatusCode}. Error: {error}");
            }
        }
    }
}
