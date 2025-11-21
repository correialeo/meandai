using MeandAI.Application.DTOs.LearningPaths;
using MeandAI.Application.Services.Interfaces;

namespace MeandAI.Application.UseCases.LearningPaths;

public class AddStepToLearningPathUseCase
{
    private readonly ILearningPathsService _learningPathsService;

    public AddStepToLearningPathUseCase(ILearningPathsService learningPathsService)
    {
        _learningPathsService = learningPathsService;
    }

    public Task<LearningPathStepDto?> ExecuteAsync(Guid learningPathId, AddStepToPathRequest request, CancellationToken cancellationToken = default)
    {
        return _learningPathsService.AddStepAsync(learningPathId, request, cancellationToken);
    }
}
