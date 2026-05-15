using portfolio_api.Models;

namespace portfolio_api.Interfaces.Services;

public interface IEmailService
{
    Task SendContactEmailAsync(ContactRequest request);
}