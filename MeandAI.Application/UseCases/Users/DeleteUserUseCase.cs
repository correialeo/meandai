using MeandAI.Application.Services.Interfaces;

namespace MeandAI.Application.UseCases.Users;

public class DeleteUserUseCase
{
    private readonly IUsersService _usersService;

    public DeleteUserUseCase(IUsersService usersService)
    {
        _usersService = usersService;
    }

    public Task<bool> ExecuteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return _usersService.DeleteAsync(id, cancellationToken);
    }
}
