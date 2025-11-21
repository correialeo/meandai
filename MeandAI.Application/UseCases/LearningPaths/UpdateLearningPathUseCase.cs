using MeandAI.Application.DTOs.LearningPaths;
using MeandAI.Application.Services.Interfaces;

namespace MeandAI.Application.UseCases.LearningPaths;

public class UpdateLearningPathUseCase
{
    private readonly ILearningPathsService _learningPathsService;

    public UpdateLearningPathUseCase(ILearningPathsService learningPathsService)
    {
        _learningPathsService = learningPathsService;
    }

    public Task<LearningPathDto?> ExecuteAsync(Guid id, UpdateLearningPathRequest request, CancellationToken cancellationToken = default)
    {
        return _learningPathsService.UpdateAsync(id, request, cancellationToken);
    }
}
