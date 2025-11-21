using MeandAI.Application.Services.Interfaces;

namespace MeandAI.Application.UseCases.LearningPaths;

public class DeleteLearningPathUseCase
{
    private readonly ILearningPathsService _learningPathsService;

    public DeleteLearningPathUseCase(ILearningPathsService learningPathsService)
    {
        _learningPathsService = learningPathsService;
    }

    public Task<bool> ExecuteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return _learningPathsService.DeleteAsync(id, cancellationToken);
    }
}
