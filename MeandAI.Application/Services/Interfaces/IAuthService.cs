using MeandAI.Application.DTOs.Auth;

namespace MeandAI.Application.Services.Interfaces;

/// <summary>
/// Service for user authentication operations
/// </summary>
public interface IAuthService
{
    /// <summary>
    /// Authenticates a user and returns a JWT token
    /// </summary>
    /// <param name="request">Login credentials</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Login result with token or error</returns>
    Task<LoginResult> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);
}
