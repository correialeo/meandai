using MeandAI.Application.DTOs.Users;
using MeandAI.Application.Services.Interfaces;

namespace MeandAI.Application.UseCases.Users;

public class UpdateUserProfileUseCase
{
    private readonly IUsersService _usersService;

    public UpdateUserProfileUseCase(IUsersService usersService)
    {
        _usersService = usersService;
    }

    public Task<UserDto?> ExecuteAsync(Guid userId, UpdateUserProfileRequest request, CancellationToken cancellationToken = default)
    {
        return _usersService.UpdateProfileAsync(userId, request, cancellationToken);
    }
}
