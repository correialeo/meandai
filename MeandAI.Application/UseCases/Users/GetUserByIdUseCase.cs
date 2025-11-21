using MeandAI.Application.DTOs.Users;
using MeandAI.Application.Services.Interfaces;

namespace MeandAI.Application.UseCases.Users;

public class GetUserByIdUseCase
{
    private readonly IUsersService _usersService;

    public GetUserByIdUseCase(IUsersService usersService)
    {
        _usersService = usersService;
    }

    public Task<UserDto?> ExecuteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return _usersService.GetByIdAsync(id, cancellationToken);
    }
}
