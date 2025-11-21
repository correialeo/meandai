using MeandAI.Application.DTOs.Auth;
using MeandAI.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MeandAI.Api.Controllers;

/// <summary>
/// Authentication endpoints
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/auth")]
public class AuthController : ApiControllerBase
{
    private readonly IAuthService _authService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IAuthService authService, ILogger<AuthController> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    /// <summary>
    /// Authenticates a user and returns a JWT token
    /// </summary>
    /// <param name="request">Login credentials</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>JWT token information</returns>
    [HttpPost("login")]
    [ProducesResponseType(typeof(JwtTokenResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> LoginAsync([FromBody] LoginRequest request, CancellationToken cancellationToken)
    {
        LoginResult result = await _authService.LoginAsync(request, cancellationToken);

        if (!result.IsSuccess)
        {
            if (result.ErrorMessage?.Contains("required", StringComparison.InvariantCultureIgnoreCase) ?? false)
            {
                return BadRequestProblem(result.ErrorMessage!);
            }

            return Unauthorized(new ProblemDetails
            {
                Title = "Unauthorized",
                Detail = result.ErrorMessage,
                Status = StatusCodes.Status401Unauthorized
            });
        }

        return Ok(result.Token!);
    }

}
