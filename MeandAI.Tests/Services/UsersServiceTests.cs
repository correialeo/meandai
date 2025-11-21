using MeandAI.Application.DTOs.Skills;
using MeandAI.Application.DTOs.Users;
using MeandAI.Application.Services;
using MeandAI.Application.Services.Interfaces;
using MeandAI.Domain.Entities;
using MeandAI.Domain.Repositories;
using Moq;

namespace MeandAI.Tests.Services;

public class UsersServiceTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<ISkillRepository> _skillRepositoryMock;
    private readonly UsersService _usersService;

    public UsersServiceTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _skillRepositoryMock = new Mock<ISkillRepository>();
        _usersService = new UsersService(_userRepositoryMock.Object, _skillRepositoryMock.Object);
    }

    [Fact]
    public async Task CreateAsync_WithValidData_ShouldReturnUser()
    {
        // Arrange
        CreateUserRequest request = new CreateUserRequest("Test User", "test@example.com", "Developer", "AI", "password123");

        _userRepositoryMock.Setup(x => x.GetByEmailAsync(request.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);
        _userRepositoryMock.Setup(x => x.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        UserDto result = await _usersService.CreateAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(request.Name, result.Name);
        Assert.Equal(request.Email, result.Email);
        Assert.Equal(request.CurrentRole, result.CurrentRole);
        Assert.Equal(request.DesiredArea, result.DesiredArea);

        // Verify that AddAsync was called with correct user data (excluding password hash verification in lambda)
        _userRepositoryMock.Verify(x => x.AddAsync(It.Is<User>(u => 
            u.Name == request.Name && 
            u.Email == request.Email && 
            u.CurrentRole == request.CurrentRole &&
            u.DesiredArea == request.DesiredArea
        ), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_WithDuplicateEmail_ShouldThrowException()
    {
        // Arrange
        CreateUserRequest request = new CreateUserRequest("Test User", "test@example.com", "Developer", "AI", "password123");
        User existingUser = new User("Existing User", "test@example.com", "Manager", "Sales", "hashedpassword");

        _userRepositoryMock.Setup(x => x.GetByEmailAsync(request.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingUser);

        // Act & Assert
        InvalidOperationException exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _usersService.CreateAsync(request));

        Assert.Contains("Já existe um usuário cadastrado com o e-mail", exception.Message);
        _userRepositoryMock.Verify(x => x.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task GetByIdAsync_WithExistingUser_ShouldReturnUser()
    {
        // Arrange
        Guid userId = Guid.NewGuid();
        User user = new User("Test User", "test@example.com", "Developer", "AI", "hashedpassword");

        _userRepositoryMock.Setup(x => x.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        // Act
        UserDto? result = await _usersService.GetByIdAsync(userId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(user.Name, result.Name);
        Assert.Equal(user.Email, result.Email);
        Assert.Equal(user.CurrentRole, result.CurrentRole);
        Assert.Equal(user.DesiredArea, result.DesiredArea);
    }

    [Fact]
    public async Task GetByIdAsync_WithNonExistingUser_ShouldReturnNull()
    {
        // Arrange
        Guid userId = Guid.NewGuid();

        _userRepositoryMock.Setup(x => x.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        // Act
        UserDto? result = await _usersService.GetByIdAsync(userId);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task UpdateProfileAsync_WithExistingUser_ShouldReturnUpdatedUser()
    {
        // Arrange
        Guid userId = Guid.NewGuid();
        User user = new User("Test User", "test@example.com", "Developer", "AI", "hashedpassword");
        UpdateUserProfileRequest request = new UpdateUserProfileRequest("Updated Name", "Senior Developer", "Machine Learning");

        _userRepositoryMock.Setup(x => x.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _userRepositoryMock.Setup(x => x.UpdateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        UserDto? result = await _usersService.UpdateProfileAsync(userId, request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(request.Name, result.Name);
        Assert.Equal(request.CurrentRole, result.CurrentRole);
        Assert.Equal(request.DesiredArea, result.DesiredArea);
        Assert.Equal(user.Email, result.Email); // Email should not change

        _userRepositoryMock.Verify(x => x.UpdateAsync(It.Is<User>(u => 
            u.Name == request.Name && 
            u.CurrentRole == request.CurrentRole &&
            u.DesiredArea == request.DesiredArea
        ), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateProfileAsync_WithNonExistingUser_ShouldReturnNull()
    {
        // Arrange
        Guid userId = Guid.NewGuid();
        UpdateUserProfileRequest request = new UpdateUserProfileRequest("Updated Name", "Senior Developer", "Machine Learning");

        _userRepositoryMock.Setup(x => x.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        // Act
        UserDto? result = await _usersService.UpdateProfileAsync(userId, request);

        // Assert
        Assert.Null(result);
        _userRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task DeleteAsync_WithExistingUser_ShouldReturnTrue()
    {
        // Arrange
        Guid userId = Guid.NewGuid();
        User user = new User("Test User", "test@example.com", "Developer", "AI", "hashedpassword");

        _userRepositoryMock.Setup(x => x.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _userRepositoryMock.Setup(x => x.DeleteAsync(user, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        bool result = await _usersService.DeleteAsync(userId);

        // Assert
        Assert.True(result);
        _userRepositoryMock.Verify(x => x.DeleteAsync(user, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_WithNonExistingUser_ShouldReturnFalse()
    {
        // Arrange
        Guid userId = Guid.NewGuid();

        _userRepositoryMock.Setup(x => x.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        // Act
        bool result = await _usersService.DeleteAsync(userId);

        // Assert
        Assert.False(result);
        _userRepositoryMock.Verify(x => x.DeleteAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task AddSkillAsync_WithValidData_ShouldReturnUserSkill()
    {
        // Arrange
        Guid userId = Guid.NewGuid();
        Guid skillId = Guid.NewGuid();
        User user = new User("Test User", "test@example.com", "Developer", "AI", "hashedpassword");
        Skill skill = new Skill("C#", "Programming");
        AddUserSkillRequest request = new AddUserSkillRequest(skill.Id, 5);

        _userRepositoryMock.Setup(x => x.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _skillRepositoryMock.Setup(x => x.GetByIdAsync(skill.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(skill);
        _userRepositoryMock.Setup(x => x.UpdateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        UserSkillDto? result = await _usersService.AddSkillAsync(userId, request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(skill.Id, result.SkillId);
        Assert.Equal(request.Level, result.Level);
        Assert.Equal("C#", result.SkillName);
        Assert.Equal("Programming", result.Category);
    }

    [Fact]
    public async Task AddSkillAsync_WithNonExistingUser_ShouldReturnNull()
    {
        // Arrange
        Guid userId = Guid.NewGuid();
        AddUserSkillRequest request = new AddUserSkillRequest(Guid.NewGuid(), 5);

        _userRepositoryMock.Setup(x => x.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        // Act
        UserSkillDto? result = await _usersService.AddSkillAsync(userId, request);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task AddSkillAsync_WithNonExistingSkill_ShouldThrowException()
    {
        // Arrange
        Guid userId = Guid.NewGuid();
        Guid skillId = Guid.NewGuid();
        User user = new User("Test User", "test@example.com", "Developer", "AI", "hashedpassword");
        AddUserSkillRequest request = new AddUserSkillRequest(skillId, 5);

        _userRepositoryMock.Setup(x => x.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _skillRepositoryMock.Setup(x => x.GetByIdAsync(skillId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Skill?)null);

        // Act & Assert
        InvalidOperationException exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _usersService.AddSkillAsync(userId, request));

        Assert.Equal("A skill informada não foi encontrada.", exception.Message);
    }
}