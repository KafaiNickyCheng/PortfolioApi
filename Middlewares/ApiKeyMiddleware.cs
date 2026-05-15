using portfolio_api.Interfaces.Services;

namespace portfolio_api.Middlewares;

public class ApiKeyMiddleware
{
    private readonly RequestDelegate _next;
    private const string ApiKeyHeader = "x-api-key";

    public ApiKeyMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, IApiKeyService apiKeyService)
    {
        // Skip middleware for swagger in development
        if (context.Request.Path.StartsWithSegments("/swagger") ||
            context.Request.Path.StartsWithSegments("/scalar") ||
            context.Request.Path == "/")
        {
            await _next(context);
            return;
        }

        // Check if x-api-key header exists
        if (!context.Request.Headers.TryGetValue(ApiKeyHeader, out var extractedApiKey))
        {
            context.Response.StatusCode = 401;
            await context.Response.WriteAsJsonAsync(new
            {
                error = "API key is missing."
            });
            return;
        }

        // Validate the key
        if (!apiKeyService.IsValidApiKey(extractedApiKey!))
        {
            context.Response.StatusCode = 403;
            await context.Response.WriteAsJsonAsync(new
            {
                error = "Invalid API key."
            });
            return;
        }

        await _next(context);
    }
}