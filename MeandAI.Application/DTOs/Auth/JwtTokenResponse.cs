namespace MeandAI.Application.DTOs.Auth;

/// <summary>
/// Response model for JWT token generation
/// </summary>
public class JwtTokenResponse
{
    /// <summary>
    /// JWT access token
    /// </summary>
    public string Token { get; set; } = string.Empty;

    /// <summary>
    /// Token expiration time in UTC
    /// </summary>
    public DateTime ExpiresAt { get; set; }

    /// <summary>
    /// Token expiration in seconds
    /// </summary>
    public int ExpiresIn { get; set; }
}
