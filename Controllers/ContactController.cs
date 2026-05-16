using Microsoft.AspNetCore.Mvc;
using portfolio_api.Interfaces.Service;
using portfolio_api.Models.Api;
using portfolio_api.Models.Contact;

namespace portfolio_api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ContactController : ControllerBase
{
    private readonly IEmailService _emailService;
    private readonly ILogger<ContactController> _logger;

    public ContactController(IEmailService emailService, ILogger<ContactController> logger)
    {
        _emailService = emailService;
        _logger = logger;
    }

    [HttpPost]
    public async Task<IActionResult> Send([FromBody] ContactRequest request)
    {
        // ── 400 Bad Request — missing required fields
        if (string.IsNullOrWhiteSpace(request.Name)    ||
            string.IsNullOrWhiteSpace(request.Email)   ||
            string.IsNullOrWhiteSpace(request.Message))
        {
            return BadRequest(new ApiResponse
            {
                Success    = false,
                StatusCode = 400,
                Error      = "Name, email and message are required."
            });
        }

        // ── 400 Bad Request — invalid email format
        if (!request.Email.Contains('@'))
        {
            return BadRequest(new ApiResponse
            {
                Success    = false,
                StatusCode = 400,
                Error      = "Invalid email address format."
            });
        }

        try
        {
            await _emailService.SendContactEmailAsync(request);

            _logger.LogInformation("Contact email sent from {Email} at {Time}",
                request.Email, DateTime.UtcNow);

            // ── 200 OK
            return Ok(new ApiResponse
            {
                Success    = true,
                StatusCode = 200,
                Message    = "Email sent successfully."
            });
        }

        // ── 400 Bad Request — invalid payload sent to Resend
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid email payload");
            return BadRequest(new ApiResponse
            {
                Success    = false,
                StatusCode = 400,
                Error      = ex.Message
            });
        }

        // ── 401 Unauthorized — bad Resend API key
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogError(ex, "Resend API authentication failed");
            return StatusCode(401, new ApiResponse
            {
                Success    = false,
                StatusCode = 401,
                Error      = ex.Message
            });
        }

        // ── 429 Too Many Requests — rate limit hit
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Email rate limit or config issue");
            return StatusCode(429, new ApiResponse
            {
                Success    = false,
                StatusCode = 429,
                Error      = ex.Message
            });
        }

        // ── 408 Request Timeout — Resend timed out
        catch (TimeoutException ex)
        {
            _logger.LogError(ex, "Email service timed out");
            return StatusCode(408, new ApiResponse
            {
                Success    = false,
                StatusCode = 408,
                Error      = ex.Message
            });
        }

        // ── 503 Service Unavailable — network error reaching Resend
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Network error sending email");
            return StatusCode(503, new ApiResponse
            {
                Success    = false,
                StatusCode = 503,
                Error      = ex.Message
            });
        }

        // ── 500 Internal Server Error — anything unexpected
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error sending email");
            return StatusCode(500, new ApiResponse
            {
                Success    = false,
                StatusCode = 500,
                Error      = "An unexpected error occurred. Please try again."
            });
        }
    }
}