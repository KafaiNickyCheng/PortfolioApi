namespace portfolio_api.Interfaces.Model.Contact;

public interface IContactRequest
{
    string Name    { get; set; }
    string Email   { get; set; }
    string Company { get; set; }
    string Message { get; set; }
}