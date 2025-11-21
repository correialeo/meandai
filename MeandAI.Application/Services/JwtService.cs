using MeandAI.Application.DTOs.Auth;
using MeandAI.Application.Services.Interfaces;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MeandAI.Application.Services;

/// <summary>
/// Service for JWT token generation and validation
/// </summary>
public class JwtService : IJwtService
{
    private readonly string _jwtKey;
    private readonly string _jwtIssuer;
    private readonly string _jwtAudience;
    private readonly int _tokenExpirationHours;

    public JwtService()
    {
        _jwtKey = Environment.GetEnvironmentVariable("JWT_KEY") ?? "MeandAI_Default_Secret_Key_123456789";
        _jwtIssuer = Environment.GetEnvironmentVariable("JWT_ISSUER") ?? "MeandAI";
        _jwtAudience = Environment.GetEnvironmentVariable("JWT_AUDIENCE") ?? "MeandAI_Users";

        string? expirationHoursStr = Environment.GetEnvironmentVariable("JWT_TOKEN_EXPIRATION_HOURS");
        _tokenExpirationHours = int.TryParse(expirationHoursStr, out int hours) ? hours : 24;
    }

    /// <summary>
    /// Generates a JWT token for the specified user
    /// </summary>
    /// <param name="email">User email</param>
    /// <returns>JWT token response</returns>
    public JwtTokenResponse GenerateToken(string email)
    {
        SymmetricSecurityKey securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtKey));
        SigningCredentials credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        Claim[] claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, email),
            new Claim(ClaimTypes.Email, email),
            new Claim(JwtRegisteredClaimNames.Sub, email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
        };

        JwtSecurityToken token = new JwtSecurityToken(
            issuer: _jwtIssuer,
            audience: _jwtAudience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(_tokenExpirationHours),
            signingCredentials: credentials
        );

        string tokenString = new JwtSecurityTokenHandler().WriteToken(token);

        return new JwtTokenResponse
        {
            Token = tokenString,
            ExpiresAt = DateTime.UtcNow.AddHours(_tokenExpirationHours),
            ExpiresIn = _tokenExpirationHours * 60 * 60 // Convert to seconds
        };
    }

    /// <summary>
    /// Validates a JWT token
    /// </summary>
    /// <param name="token">JWT token string</param>
    /// <returns>Token validation result</returns>
    public bool ValidateToken(string token)
    {
        try
        {
            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            TokenValidationParameters validationParameters = GetTokenValidationParameters();

            tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);
            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Gets the user email from a valid JWT token
    /// </summary>
    /// <param name="token">JWT token string</param>
    /// <returns>User email or null if invalid</returns>
    public string? GetEmailFromToken(string token)
    {
        try
        {
            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            TokenValidationParameters validationParameters = GetTokenValidationParameters();

            ClaimsPrincipal principal = tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);
            return principal.FindFirst(ClaimTypes.Email)?.Value;
        }
        catch
        {
            return null;
        }
    }

    private TokenValidationParameters GetTokenValidationParameters()
    {
        return new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = _jwtIssuer,
            ValidAudience = _jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtKey)),
            ClockSkew = TimeSpan.Zero
        };
    }
}
