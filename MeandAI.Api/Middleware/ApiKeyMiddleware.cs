using System.Security.Claims;

namespace MeandAI.Api.Middleware;

public class ApiKeyMiddleware
{
    private readonly RequestDelegate _next;
    private readonly string _apiKey;
    private const string ApiKeyHeaderName = "X-API-Key";

    public ApiKeyMiddleware(RequestDelegate next, IConfiguration configuration)
    {
        _next = next;
        _apiKey = configuration.GetValue<string>("API_KEY") ?? "MeandAI_Default_Api_Key_123456789";
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (context.User?.Identity?.IsAuthenticated == true)
        {
            await _next(context);
            return;
        }

        if (context.Request.Headers.TryGetValue(ApiKeyHeaderName, out var apiKeyValues))
        {
            var providedApiKey = apiKeyValues.FirstOrDefault();
            
            if (!string.IsNullOrEmpty(providedApiKey) && providedApiKey == _apiKey)
            {
                var claims = new List<Claim>
                {
                    new(ClaimTypes.NameIdentifier, "api-key-user"),
                    new(ClaimTypes.Name, "API Key User"),
                    new("api_key", "true"),
                    new("role", "ApiUser")
                };

                var identity = new ClaimsIdentity(claims, "ApiKey");
                var principal = new ClaimsPrincipal(identity);
                
                context.User = principal;
                await _next(context);
                return;
            }
        }

        var endpoint = context.GetEndpoint();
        if (endpoint?.Metadata?.GetMetadata<Microsoft.AspNetCore.Authorization.AuthorizeAttribute>() != null)
        {
            context.Response.StatusCode = 401;
            context.Response.Headers.Add("WWW-Authenticate", "ApiKey");
            await context.Response.WriteAsync("Missing or invalid API Key");
            return;
        }

        await _next(context);
    }
}
