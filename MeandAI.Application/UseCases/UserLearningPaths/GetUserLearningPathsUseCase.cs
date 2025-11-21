using MeandAI.Application.DTOs.UserLearningPaths;
using MeandAI.Application.Services.Interfaces;

namespace MeandAI.Application.UseCases.UserLearningPaths;

public class GetUserLearningPathsUseCase
{
    private readonly IUserLearningPathsService _userLearningPathsService;

    public GetUserLearningPathsUseCase(IUserLearningPathsService userLearningPathsService)
    {
        _userLearningPathsService = userLearningPathsService;
    }

    public Task<IReadOnlyCollection<UserLearningPathDto>> ExecuteAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return _userLearningPathsService.GetByUserAsync(userId, cancellationToken);
    }
}
