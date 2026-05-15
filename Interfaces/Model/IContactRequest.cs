namespace portfolio_api.Interfaces.Models;

public interface IContactRequest
{
    string Name    { get; set; }
    string Email   { get; set; }
    string Company { get; set; }
    string Message { get; set; }
}