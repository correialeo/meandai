namespace MeandAI.Application.DTOs.Auth;

/// <summary>
/// Request model for user authentication
/// </summary>
public class LoginRequest
{
    /// <summary>
    /// User email address
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// User password
    /// </summary>
    public string Password { get; set; } = string.Empty;
}
