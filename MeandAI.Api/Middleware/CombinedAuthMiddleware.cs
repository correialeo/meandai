using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;

namespace MeandAI.Api.Middleware;

public class CombinedAuthMiddleware
{
    private readonly RequestDelegate _next;
    private readonly string _apiKey;
    private readonly string _jwtKey;
    private readonly string _jwtIssuer;
    private readonly string _jwtAudience;
    private const string ApiKeyHeaderName = "X-API-Key";
    private const string BearerHeaderName = "Authorization";

    public CombinedAuthMiddleware(RequestDelegate next, IConfiguration configuration)
    {
        _next = next;
        _apiKey = configuration.GetValue<string>("API_KEY") ?? "MeandAI_Default_Api_Key_123456789";
        _jwtKey = configuration.GetValue<string>("JWT_KEY") ?? "MeandAI_Default_Secret_Key_123456789";
        _jwtIssuer = configuration.GetValue<string>("JWT_ISSUER") ?? "MeandAI";
        _jwtAudience = configuration.GetValue<string>("JWT_AUDIENCE") ?? "MeandAI_Users";
    }

    public async Task InvokeAsync(HttpContext context)
    {
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

        if (context.Request.Headers.TryGetValue(BearerHeaderName, out var bearerValues))
        {
            var bearerToken = bearerValues.FirstOrDefault();
            
            if (!string.IsNullOrEmpty(bearerToken) && bearerToken.StartsWith("Bearer "))
            {
                var token = bearerToken.Substring("Bearer ".Length).Trim();
                
                try
                {
                    var tokenHandler = new JwtSecurityTokenHandler();
                    var key = Encoding.UTF8.GetBytes(_jwtKey);
                    
                    var validationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = _jwtIssuer,
                        ValidAudience = _jwtAudience,
                        IssuerSigningKey = new SymmetricSecurityKey(key),
                        ClockSkew = TimeSpan.Zero
                    };

                    var principal = tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);
                    
                    context.User = principal;
                    await _next(context);
                    return;
                }
                catch
                {
                    // JWT inválido, continua para verificar se endpoint requer autenticação
                }
            }
        }

        var endpoint = context.GetEndpoint();
        if (endpoint?.Metadata?.GetMetadata<Microsoft.AspNetCore.Authorization.AuthorizeAttribute>() != null)
        {
            context.Response.StatusCode = 401;
            context.Response.Headers["WWW-Authenticate"] = "Bearer, ApiKey";
            await context.Response.WriteAsync("Missing or invalid API Key or Bearer token");
            return;
        }

        await _next(context);
    }
}
