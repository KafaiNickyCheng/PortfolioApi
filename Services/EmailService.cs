using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using portfolio_api.Interfaces.Services;
using portfolio_api.Models;
using portfolio_api.Templates;

namespace portfolio_api.Services;

public class EmailService : IEmailService
{
    private readonly IConfiguration _config;
    private readonly HttpClient _httpClient;

    public EmailService(IConfiguration config, IHttpClientFactory httpClientFactory)
    {
        _config = config;
        _httpClient = httpClientFactory.CreateClient("Resend");
    }

    public async Task SendContactEmailAsync(ContactRequest request)
    {
        var apiKey = Environment.GetEnvironmentVariable("RESEND_API_KEY")
                  ?? _config["ResendSettings:ApiKey"]
                  ?? string.Empty;

        var receiverEmail = Environment.GetEnvironmentVariable("SMTP_RECEIVER")
                         ?? _config["SmtpSettings:ReceiverEmail"]
                         ?? string.Empty;

        var payload = new
        {
            from    = "Portfolio Contact <onboarding@resend.dev>",
            to      = new[] { receiverEmail },
            reply_to = request.Email,
            subject = $"Portfolio Contact from {request.Name}",
            html    = EmailTemplates.ContactEmail(request)
        };

        var json    = JsonSerializer.Serialize(payload);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        _httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", apiKey);

        var response = await _httpClient.PostAsync("https://api.resend.com/emails", content);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            throw new Exception($"Resend API error: {error}");
        }
    }
}