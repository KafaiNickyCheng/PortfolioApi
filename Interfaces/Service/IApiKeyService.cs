namespace portfolio_api.Interfaces.Services;

public interface IApiKeyService
{
    bool IsValidApiKey(string apiKey);
}