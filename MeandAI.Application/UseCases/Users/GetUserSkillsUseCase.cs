using MeandAI.Application.DTOs.Skills;
using MeandAI.Application.Services.Interfaces;

namespace MeandAI.Application.UseCases.Users;

public class GetUserSkillsUseCase
{
    private readonly IUsersService _usersService;

    public GetUserSkillsUseCase(IUsersService usersService)
    {
        _usersService = usersService;
    }

    public Task<IReadOnlyCollection<UserSkillDto>> ExecuteAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return _usersService.GetSkillsAsync(userId, cancellationToken);
    }
}
