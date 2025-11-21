using MeandAI.Application.DTOs.UserLearningPaths;
using MeandAI.Application.Services.Interfaces;

namespace MeandAI.Application.UseCases.UserLearningPaths;

public class EnrollUserInLearningPathUseCase
{
    private readonly IUserLearningPathsService _userLearningPathsService;

    public EnrollUserInLearningPathUseCase(IUserLearningPathsService userLearningPathsService)
    {
        _userLearningPathsService = userLearningPathsService;
    }

    public Task<UserLearningPathDto> ExecuteAsync(Guid userId, EnrollUserInPathRequest request, CancellationToken cancellationToken = default)
    {
        return _userLearningPathsService.EnrollAsync(userId, request, cancellationToken);
    }
}
