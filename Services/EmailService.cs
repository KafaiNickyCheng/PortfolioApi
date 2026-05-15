using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using portfolio_api.Interfaces.Services;
using portfolio_api.Models;
using portfolio_api.Templates;

namespace portfolio_api.Services;


public class EmailService : IEmailService
{
    private readonly IConfiguration _config;

    public EmailService(IConfiguration config)
    {
        _config = config;
    }

    public async Task SendContactEmailAsync(ContactRequest request)
    {
        var smtpSettings = _config.GetSection("SmtpSettings");

        var host          = smtpSettings["Host"]          ?? string.Empty;
        var port          = int.Parse(smtpSettings["Port"] ?? "587");
        var senderEmail   = smtpSettings["SenderEmail"]   ?? string.Empty;
        var receiverEmail = smtpSettings["ReceiverEmail"] ?? string.Empty;
        var password      = smtpSettings["Password"]      ?? string.Empty;

        var message = new MimeMessage();

        message.From.Add(new MailboxAddress("Portfolio Contact", senderEmail));
        message.To.Add(new MailboxAddress("Kafai Cheng", receiverEmail));
        message.ReplyTo.Add(new MailboxAddress(request.Name, request.Email));

        message.Subject = $"Portfolio Contact from {request.Name}";

        var bodyBuilder = new BodyBuilder
        {
            HtmlBody = EmailTemplates.ContactEmail(request)
        };

        message.Body = bodyBuilder.ToMessageBody();

        using var smtp = new SmtpClient();

        await smtp.ConnectAsync(host, port, SecureSocketOptions.StartTls);
        await smtp.AuthenticateAsync(senderEmail, password);
        await smtp.SendAsync(message);
        await smtp.DisconnectAsync(true);
    }
}