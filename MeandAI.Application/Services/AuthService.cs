using MeandAI.Application.DTOs.Auth;
using MeandAI.Application.Services.Interfaces;
using MeandAI.Domain.Repositories;
using BCrypt.Net;
using MeandAI.Domain.Entities;

namespace MeandAI.Application.Services;

/// <summary>
/// Service for user authentication operations
/// </summary>
public class AuthService : IAuthService
{
    private readonly IJwtService _jwtService;
    private readonly IUserRepository _userRepository;

    public AuthService(IJwtService jwtService, IUserRepository userRepository)
    {
        _jwtService = jwtService;
        _userRepository = userRepository;
    }

    /// <summary>
    /// Authenticates a user and returns a JWT token
    /// </summary>
    /// <param name="request">Login credentials</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Login result with token or error</returns>
    public async Task<LoginResult> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            // Validate input
            if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
            {
                return LoginResult.Failure("Email and password are required.");
            }

            // Validate user credentials
            if (!await ValidateUserCredentialsAsync(request.Email, request.Password, cancellationToken))
            {
                return LoginResult.Failure("Invalid email or password.");
            }

            // Generate JWT token
            JwtTokenResponse token = _jwtService.GenerateToken(request.Email);

            return LoginResult.Success(token);
        }
        catch
        {
            return LoginResult.Failure("An error occurred during authentication. Please try again.");
        }
    }

    /// <summary>
    /// Validates user credentials against the database
    /// </summary>
    /// <param name="email">User email</param>
    /// <param name="password">User password</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if credentials are valid</returns>
    private async Task<bool> ValidateUserCredentialsAsync(string email, string password, CancellationToken cancellationToken)
    {
        User? user = await _userRepository.GetByEmailAsync(email, cancellationToken);
        if (user is null)
        {
            return false;
        }

        return BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
    }
}
