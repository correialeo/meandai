using MeandAI.Application.Common;
using MeandAI.Application.DTOs.Skills;
using MeandAI.Application.DTOs.Users;

namespace MeandAI.Application.Services.Interfaces;

public interface IUsersService
{
    Task<UserDto> CreateAsync(CreateUserRequest request, CancellationToken cancellationToken = default);
    Task<UserDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<UserDto?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<PagedResult<UserDto>> GetPagedAsync(int page, int pageSize, CancellationToken cancellationToken = default);
    Task<UserDto?> UpdateProfileAsync(Guid id, UpdateUserProfileRequest request, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<UserSkillDto?> AddSkillAsync(Guid userId, AddUserSkillRequest request, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<UserSkillDto>> GetSkillsAsync(Guid userId, CancellationToken cancellationToken = default);
}
