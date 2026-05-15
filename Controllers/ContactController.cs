using Microsoft.AspNetCore.Mvc;
using portfolio_api.Interfaces.Services;
using portfolio_api.Models;

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
        if (string.IsNullOrWhiteSpace(request.Name) ||
            string.IsNullOrWhiteSpace(request.Email) ||
            string.IsNullOrWhiteSpace(request.Message))
        {
            return BadRequest(new { error = "Name, email and message are required." });
        }

        try
        {
            await _emailService.SendContactEmailAsync(request);
            _logger.LogInformation("Contact email sent from {Email}", request.Email);
            return Ok(new { success = true, message = "Email sent successfully." });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send contact email");
            return StatusCode(500, new { error = "Failed to send email. Please try again." });
        }
    }
}