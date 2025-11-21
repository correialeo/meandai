using MeandAI.Application.DTOs.Auth;

namespace MeandAI.Application.Services.Interfaces;

/// <summary>
/// Service interface for JWT token generation and validation
/// </summary>
public interface IJwtService
{
    /// <summary>
    /// Generates a JWT token for the specified user
    /// </summary>
    /// <param name="email">User email</param>
    /// <returns>JWT token response</returns>
    JwtTokenResponse GenerateToken(string email);

    /// <summary>
    /// Validates a JWT token
    /// </summary>
    /// <param name="token">JWT token string</param>
    /// <returns>Token validation result</returns>
    bool ValidateToken(string token);

    /// <summary>
    /// Gets the user email from a valid JWT token
    /// </summary>
    /// <param name="token">JWT token string</param>
    /// <returns>User email or null if invalid</returns>
    string? GetEmailFromToken(string token);
}
