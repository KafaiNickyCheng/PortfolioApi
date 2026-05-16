using portfolio_api.Models;
using portfolio_api.Models.Contact;
namespace portfolio_api.Interfaces.Service;

public interface IEmailService
{
    Task SendContactEmailAsync(ContactRequest request);
}