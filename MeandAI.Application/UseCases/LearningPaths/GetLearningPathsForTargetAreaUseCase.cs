using MeandAI.Application.DTOs.LearningPaths;
using MeandAI.Application.Services.Interfaces;

namespace MeandAI.Application.UseCases.LearningPaths;

public class GetLearningPathsForTargetAreaUseCase
{
    private readonly ILearningPathsService _learningPathsService;

    public GetLearningPathsForTargetAreaUseCase(ILearningPathsService learningPathsService)
    {
        _learningPathsService = learningPathsService;
    }

    public Task<IReadOnlyCollection<LearningPathDto>> ExecuteAsync(string targetArea, CancellationToken cancellationToken = default)
    {
        return _learningPathsService.GetForTargetAreaAsync(targetArea, cancellationToken);
    }
}
