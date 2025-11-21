using MeandAI.Application.DTOs.UserLearningPaths;
using MeandAI.Application.Services.Interfaces;

namespace MeandAI.Application.UseCases.UserLearningPaths;

public class UpdateUserLearningPathStatusUseCase
{
    private readonly IUserLearningPathsService _userLearningPathsService;

    public UpdateUserLearningPathStatusUseCase(IUserLearningPathsService userLearningPathsService)
    {
        _userLearningPathsService = userLearningPathsService;
    }

    public Task<UserLearningPathDto?> ExecuteAsync(Guid userId, UpdateUserLearningPathStatusRequest request, CancellationToken cancellationToken = default)
    {
        return _userLearningPathsService.UpdateStatusAsync(userId, request, cancellationToken);
    }
}
