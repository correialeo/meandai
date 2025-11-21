namespace MeandAI.Application.DTOs.Auth;

/// <summary>
/// Result of a login attempt
/// </summary>
public class LoginResult
{
    /// <summary>
    /// Indicates if the login was successful
    /// </summary>
    public bool IsSuccess { get; set; }

    /// <summary>
    /// JWT token (only if login was successful)
    /// </summary>
    public JwtTokenResponse? Token { get; set; }

    /// <summary>
    /// Error message (only if login failed)
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// Creates a successful login result
    /// </summary>
    public static LoginResult Success(JwtTokenResponse token) => new()
    {
        IsSuccess = true,
        Token = token
    };

    /// <summary>
    /// Creates a failed login result
    /// </summary>
    public static LoginResult Failure(string errorMessage) => new()
    {
        IsSuccess = false,
        ErrorMessage = errorMessage
    };
}
