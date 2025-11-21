using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;

namespace MeandAI.Api.Middleware;

public class ApiKeyAuthenticationMiddleware : IAuthenticationHandler
{
    private const string ApiKeyHeaderName = "X-API-Key";
    private readonly RequestDelegate _next;
    private readonly string _apiKey;
    private HttpContext? _context;
    private AuthenticationScheme? _scheme;

    public ApiKeyAuthenticationMiddleware(RequestDelegate next, IConfiguration configuration)
    {
        _next = next;
        _apiKey = configuration.GetValue<string>("API_KEY") ?? "MeandAI_Default_Api_Key_123456789";
    }

    public Task InitializeAsync(AuthenticationScheme scheme, HttpContext context)
    {
        _scheme = scheme;
        _context = context;
        return Task.CompletedTask;
    }

    public async Task<AuthenticateResult> AuthenticateAsync()
    {
        if (_context == null || _scheme == null)
        {
            return AuthenticateResult.Fail("Context or scheme not initialized");
        }

        if (!_context.Request.Headers.TryGetValue(ApiKeyHeaderName, out var apiKeyValues))
        {
            return AuthenticateResult.NoResult();
        }

        var providedApiKey = apiKeyValues.FirstOrDefault();
        if (string.IsNullOrEmpty(providedApiKey) || providedApiKey != _apiKey)
        {
            return AuthenticateResult.Fail("Invalid API Key");
        }

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, "api-key-user"),
            new Claim(ClaimTypes.Name, "API Key User"),
            new Claim("api_key", "true")
        };

        var identity = new ClaimsIdentity(claims, _scheme.Name);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, _scheme.Name);

        return AuthenticateResult.Success(ticket);
    }

    public Task ChallengeAsync(AuthenticationProperties? properties)
    {
        if (_context != null)
        {
            _context.Response.StatusCode = 401;
            _context.Response.Headers.Add("WWW-Authenticate", "ApiKey");
            return _context.Response.WriteAsync("Invalid or missing API Key");
        }
        return Task.CompletedTask;
    }

    public Task ForbidAsync(AuthenticationProperties? properties)
    {
        if (_context != null)
        {
            _context.Response.StatusCode = 403;
            return _context.Response.WriteAsync("Forbidden");
        }
        return Task.CompletedTask;
    }
}

public class ApiKeyMiddlewareOptions
{
    public string HeaderName { get; set; } = "X-API-Key";
    public string ApiKey { get; set; } = string.Empty;
}
