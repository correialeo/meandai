using MeandAI.Application.DTOs.LearningPaths;
using MeandAI.Application.Services.Interfaces;

namespace MeandAI.Application.UseCases.LearningPaths;

public class GetLearningPathByIdUseCase
{
    private readonly ILearningPathsService _learningPathsService;

    public GetLearningPathByIdUseCase(ILearningPathsService learningPathsService)
    {
        _learningPathsService = learningPathsService;
    }

    public Task<LearningPathDto?> ExecuteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return _learningPathsService.GetByIdAsync(id, cancellationToken);
    }
}
