using MeandAI.Application.Common;
using MeandAI.Application.DTOs.Users;
using MeandAI.Application.Services.Interfaces;

namespace MeandAI.Application.UseCases.Users;

public class GetUsersPagedUseCase
{
    private readonly IUsersService _usersService;

    public GetUsersPagedUseCase(IUsersService usersService)
    {
        _usersService = usersService;
    }

    public Task<PagedResult<UserDto>> ExecuteAsync(int page, int pageSize, CancellationToken cancellationToken = default)
    {
        return _usersService.GetPagedAsync(page, pageSize, cancellationToken);
    }
}
