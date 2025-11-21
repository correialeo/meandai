using MeandAI.Application.DTOs.Auth;
using MeandAI.Application.Services;
using MeandAI.Application.Services.Interfaces;
using MeandAI.Domain.Entities;
using MeandAI.Domain.Repositories;
using Moq;

namespace MeandAI.Tests.Services;

public class AuthServiceTests
{
    private readonly Mock<IJwtService> _jwtServiceMock;
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly AuthService _authService;

    public AuthServiceTests()
    {
        _jwtServiceMock = new Mock<IJwtService>();
        _userRepositoryMock = new Mock<IUserRepository>();
        _authService = new AuthService(_jwtServiceMock.Object, _userRepositoryMock.Object);
    }

    [Fact]
    public async Task LoginAsync_WithValidCredentials_ShouldReturnSuccess()
    {
        // Arrange
        string email = "test@example.com";
        string password = "password123";
        string passwordHash = BCrypt.Net.BCrypt.HashPassword(password);
        User user = new User("Test User", email, "Developer", "AI", passwordHash);
        JwtTokenResponse tokenResponse = new JwtTokenResponse
        {
            Token = "test-token",
            ExpiresAt = DateTime.UtcNow.AddHours(24),
            ExpiresIn = 86400
        };
        LoginRequest request = new LoginRequest
        {
            Email = email,
            Password = password
        };

        _userRepositoryMock.Setup(x => x.GetByEmailAsync(email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _jwtServiceMock.Setup(x => x.GenerateToken(email))
            .Returns(tokenResponse);

        // Act
        LoginResult result = await _authService.LoginAsync(request);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(tokenResponse, result.Token);
        Assert.Null(result.ErrorMessage);
    }

    [Fact]
    public async Task LoginAsync_WithInvalidEmail_ShouldReturnFailure()
    {
        // Arrange
        string email = "nonexistent@example.com";
        string password = "password123";
        LoginRequest request = new LoginRequest
        {
            Email = email,
            Password = password
        };

        _userRepositoryMock.Setup(x => x.GetByEmailAsync(email, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        // Act
        LoginResult result = await _authService.LoginAsync(request);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Null(result.Token);
        Assert.Equal("Invalid email or password.", result.ErrorMessage);
    }

    [Fact]
    public async Task LoginAsync_WithInvalidPassword_ShouldReturnFailure()
    {
        // Arrange
        string email = "test@example.com";
        string correctPassword = "password123";
        string wrongPassword = "wrongpassword";
        string passwordHash = BCrypt.Net.BCrypt.HashPassword(correctPassword);
        User user = new User("Test User", email, "Developer", "AI", passwordHash);
        LoginRequest request = new LoginRequest
        {
            Email = email,
            Password = wrongPassword
        };

        _userRepositoryMock.Setup(x => x.GetByEmailAsync(email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        // Act
        LoginResult result = await _authService.LoginAsync(request);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Null(result.Token);
        Assert.Equal("Invalid email or password.", result.ErrorMessage);
    }

    [Fact]
    public async Task LoginAsync_WithEmptyEmail_ShouldReturnFailure()
    {
        // Arrange
        LoginRequest request = new LoginRequest
        {
            Email = "",
            Password = "password123"
        };

        // Act
        LoginResult result = await _authService.LoginAsync(request);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Null(result.Token);
        Assert.Equal("Email and password are required.", result.ErrorMessage);
    }

    [Fact]
    public async Task LoginAsync_WithEmptyPassword_ShouldReturnFailure()
    {
        // Arrange
        LoginRequest request = new LoginRequest
        {
            Email = "test@example.com",
            Password = ""
        };

        // Act
        LoginResult result = await _authService.LoginAsync(request);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Null(result.Token);
        Assert.Equal("Email and password are required.", result.ErrorMessage);
    }

    [Fact]
    public async Task LoginAsync_WithNullEmail_ShouldReturnFailure()
    {
        // Arrange
        LoginRequest request = new LoginRequest
        {
            Email = null!,
            Password = "password123"
        };

        // Act
        LoginResult result = await _authService.LoginAsync(request);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Null(result.Token);
        Assert.Equal("Email and password are required.", result.ErrorMessage);
    }

    [Fact]
    public async Task LoginAsync_WithNullPassword_ShouldReturnFailure()
    {
        // Arrange
        LoginRequest request = new LoginRequest
        {
            Email = "test@example.com",
            Password = null!
        };

        // Act
        LoginResult result = await _authService.LoginAsync(request);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Null(result.Token);
        Assert.Equal("Email and password are required.", result.ErrorMessage);
    }

    [Fact]
    public async Task LoginAsync_WhenExceptionThrown_ShouldReturnFailure()
    {
        // Arrange
        LoginRequest request = new LoginRequest
        {
            Email = "test@example.com",
            Password = "password123"
        };

        _userRepositoryMock.Setup(x => x.GetByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Database error"));

        // Act
        LoginResult result = await _authService.LoginAsync(request);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Null(result.Token);
        Assert.Equal("An error occurred during authentication. Please try again.", result.ErrorMessage);
    }
}