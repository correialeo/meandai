using MeandAI.Application.DTOs.Auth;
using MeandAI.Application.Services;
using System.IdentityModel.Tokens.Jwt;

namespace MeandAI.Tests.Services;

public class JwtServiceTests
{
    private readonly JwtService _jwtService;

    public JwtServiceTests()
    {
        // Set environment variables for testing
        Environment.SetEnvironmentVariable("JWT_KEY", "test_secret_key_12345678901234567890123456789012");
        Environment.SetEnvironmentVariable("JWT_ISSUER", "TestIssuer");
        Environment.SetEnvironmentVariable("JWT_AUDIENCE", "TestAudience");
        Environment.SetEnvironmentVariable("JWT_TOKEN_EXPIRATION_HOURS", "1");

        _jwtService = new JwtService();
    }

    [Fact]
    public void GenerateToken_WithValidEmail_ShouldReturnToken()
    {
        // Arrange
        string email = "test@example.com";

        // Act
        JwtTokenResponse result = _jwtService.GenerateToken(email);

        // Assert
        Assert.NotNull(result.Token);
        Assert.True(result.ExpiresIn > 0);
        Assert.True(result.ExpiresAt > DateTime.UtcNow);
        Assert.True(result.ExpiresAt < DateTime.UtcNow.AddHours(2)); // Should be ~1 hour
    }

    [Fact]
    public void GenerateToken_ShouldContainCorrectClaims()
    {
        // Arrange
        string email = "test@example.com";

        // Act
        JwtTokenResponse result = _jwtService.GenerateToken(email);

        // Assert
        JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
        JwtSecurityToken token = tokenHandler.ReadJwtToken(result.Token);

        Assert.Equal(email, token.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.NameIdentifier)?.Value);
        Assert.Equal(email, token.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.Email)?.Value);
        Assert.Equal(email, token.Claims.FirstOrDefault(c => c.Type == "sub")?.Value);
        Assert.Equal("TestIssuer", token.Issuer);
        Assert.Equal("TestAudience", token.Audiences.FirstOrDefault());
    }

    [Fact]
    public void ValidateToken_WithValidToken_ShouldReturnTrue()
    {
        // Arrange
        string email = "test@example.com";
        string token = _jwtService.GenerateToken(email).Token;

        // Act
        bool result = _jwtService.ValidateToken(token);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void ValidateToken_WithInvalidToken_ShouldReturnFalse()
    {
        // Arrange
        string invalidToken = "invalid.token.here";

        // Act
        bool result = _jwtService.ValidateToken(invalidToken);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void ValidateToken_WithEmptyToken_ShouldReturnFalse()
    {
        // Arrange
        string emptyToken = "";

        // Act
        bool result = _jwtService.ValidateToken(emptyToken);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void ValidateToken_WithNullToken_ShouldReturnFalse()
    {
        // Act
        bool result = _jwtService.ValidateToken(null!);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void GetEmailFromToken_WithValidToken_ShouldReturnEmail()
    {
        // Arrange
        string email = "test@example.com";
        string token = _jwtService.GenerateToken(email).Token;

        // Act
        string? result = _jwtService.GetEmailFromToken(token);

        // Assert
        Assert.Equal(email, result);
    }

    [Fact]
    public void GetEmailFromToken_WithInvalidToken_ShouldReturnNull()
    {
        // Arrange
        string invalidToken = "invalid.token.here";

        // Act
        string? result = _jwtService.GetEmailFromToken(invalidToken);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void GetEmailFromToken_WithEmptyToken_ShouldReturnNull()
    {
        // Arrange
        string emptyToken = "";

        // Act
        string? result = _jwtService.GetEmailFromToken(emptyToken);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void GetEmailFromToken_WithNullToken_ShouldReturnNull()
    {
        // Act
        string? result = _jwtService.GetEmailFromToken(null!);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void GenerateToken_WithDefaultEnvironment_ShouldUseDefaults()
    {
        // Arrange - Clear environment variables
        Environment.SetEnvironmentVariable("JWT_KEY", null);
        Environment.SetEnvironmentVariable("JWT_ISSUER", null);
        Environment.SetEnvironmentVariable("JWT_AUDIENCE", null);
        Environment.SetEnvironmentVariable("JWT_TOKEN_EXPIRATION_HOURS", null);

        JwtService jwtServiceWithDefaults = new JwtService();

        // Act
        JwtTokenResponse result = jwtServiceWithDefaults.GenerateToken("test@example.com");

        // Assert
        Assert.NotNull(result.Token);
        Assert.Equal(86400, result.ExpiresIn); // 24 hours default
    }

    [Fact]
    public void GenerateToken_WithCustomExpiration_ShouldUseCustomValue()
    {
        // Arrange
        Environment.SetEnvironmentVariable("JWT_TOKEN_EXPIRATION_HOURS", "2");
        JwtService jwtServiceCustom = new JwtService();

        // Act
        JwtTokenResponse result = jwtServiceCustom.GenerateToken("test@example.com");

        // Assert
        Assert.Equal(7200, result.ExpiresIn); // 2 hours
    }
}
