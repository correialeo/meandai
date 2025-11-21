using MeandAI.Application.DTOs.Users;
using MeandAI.Application.Services.Interfaces;

namespace MeandAI.Application.UseCases.Users;

public class CreateUserUseCase
{
    private readonly IUsersService _usersService;

    public CreateUserUseCase(IUsersService usersService)
    {
        _usersService = usersService;
    }

    public Task<UserDto> ExecuteAsync(CreateUserRequest request, CancellationToken cancellationToken = default)
    {
        return _usersService.CreateAsync(request, cancellationToken);
    }
}
