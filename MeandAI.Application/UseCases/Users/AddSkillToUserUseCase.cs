using MeandAI.Application.DTOs.Skills;
using MeandAI.Application.Services.Interfaces;

namespace MeandAI.Application.UseCases.Users;

public class AddSkillToUserUseCase
{
    private readonly IUsersService _usersService;

    public AddSkillToUserUseCase(IUsersService usersService)
    {
        _usersService = usersService;
    }

    public Task<UserSkillDto?> ExecuteAsync(Guid userId, AddUserSkillRequest request, CancellationToken cancellationToken = default)
    {
        return _usersService.AddSkillAsync(userId, request, cancellationToken);
    }
}
