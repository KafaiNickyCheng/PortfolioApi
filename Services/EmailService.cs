using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using portfolio_api.Interfaces.Service;
using portfolio_api.Models.Contact;
using portfolio_api.Templates;

namespace portfolio_api.Services;

public class EmailService : IEmailService
{
    private readonly IConfiguration _config;
    private readonly HttpClient _httpClient;
    private readonly ILogger<EmailService> _logger;

    public EmailService(
        IConfiguration config,
        IHttpClientFactory httpClientFactory,
        ILogger<EmailService> logger
    )
    {
        _config = config;
        _httpClient = httpClientFactory.CreateClient("Resend");
        _logger = _logger;
    }

    public async Task SendContactEmailAsync(ContactRequest request)
    {
        var apiKey = Environment.GetEnvironmentVariable("RESEND_API_KEY")
                  ?? _config["ResendSettings:ApiKey"]
                  ?? string.Empty;

        var receiverEmail = Environment.GetEnvironmentVariable("SMTP_RECEIVER")
                         ?? _config["SmtpSettings:ReceiverEmail"]
                         ?? string.Empty;

        // Validate config
        if (string.IsNullOrEmpty(apiKey))
            throw new InvalidOperationException("Resend API key is not configured.");

        if (string.IsNullOrEmpty(receiverEmail))
            throw new InvalidOperationException("Receiver email is not configured.");

        var payload = new
        {
            from     = "Portfolio Contact <onboarding@resend.dev>",
            to       = new[] { receiverEmail },
            reply_to = request.Email,
            subject  = $"Portfolio Contact from {request.Name}",
            html     = EmailTemplates.ContactEmail(request)
        };

        var json    = JsonSerializer.Serialize(payload);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        _httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", apiKey);

        HttpResponseMessage response;

        try
        {
            response = await _httpClient.PostAsync("https://api.resend.com/emails", content);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Network error contacting Resend API");
            throw new HttpRequestException("Failed to reach email service. Please try again later.");
        }
        catch (TaskCanceledException ex)
        {
            _logger.LogError(ex, "Resend API request timed out");
            throw new TimeoutException("Email service timed out. Please try again later.");
        }

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            _logger.LogError("Resend API returned {StatusCode}: {Error}", response.StatusCode, error);

            throw response.StatusCode switch
            {
                System.Net.HttpStatusCode.Unauthorized  => new UnauthorizedAccessException("Invalid Resend API key."),
                System.Net.HttpStatusCode.TooManyRequests => new InvalidOperationException("Email rate limit reached. Please try again later."),
                System.Net.HttpStatusCode.UnprocessableEntity => new ArgumentException($"Invalid email payload: {error}"),
                _ => new Exception($"Resend API error {response.StatusCode}: {error}")
            };
        }
    }
}