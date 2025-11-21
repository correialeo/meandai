using MeandAI.Application.DTOs.LearningPaths;
using MeandAI.Application.Services.Interfaces;

namespace MeandAI.Application.UseCases.LearningPaths;

public class GetAllLearningPathsUseCase
{
    private readonly ILearningPathsService _learningPathsService;

    public GetAllLearningPathsUseCase(ILearningPathsService learningPathsService)
    {
        _learningPathsService = learningPathsService;
    }

    public Task<IReadOnlyCollection<LearningPathDto>> ExecuteAsync(CancellationToken cancellationToken = default)
    {
        return _learningPathsService.GetAllAsync(cancellationToken);
    }
}
