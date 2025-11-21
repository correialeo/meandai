using MeandAI.Application.DTOs.LearningPaths;
using MeandAI.Application.Services.Interfaces;

namespace MeandAI.Application.UseCases.LearningPaths;

public class CreateLearningPathUseCase
{
    private readonly ILearningPathsService _learningPathsService;

    public CreateLearningPathUseCase(ILearningPathsService learningPathsService)
    {
        _learningPathsService = learningPathsService;
    }

    public Task<LearningPathDto> ExecuteAsync(CreateLearningPathRequest request, CancellationToken cancellationToken = default)
    {
        return _learningPathsService.CreateAsync(request, cancellationToken);
    }
}
