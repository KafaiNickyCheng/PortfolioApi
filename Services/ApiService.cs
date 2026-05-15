using portfolio_api.Interfaces.Services;

namespace portfolio_api.Services;

public class ApiKeyService : IApiKeyService
{
    private readonly IConfiguration _config;

    public ApiKeyService(IConfiguration config)
    {
        _config = config;
    }

    public bool IsValidApiKey(string apiKey)
    {
        var validKey = _config["ApiSettings:ApiKey"] ?? string.Empty;
        return !string.IsNullOrEmpty(apiKey) && apiKey == validKey;
    }
}