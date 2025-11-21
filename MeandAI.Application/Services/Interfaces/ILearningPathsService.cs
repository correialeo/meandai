using MeandAI.Application.DTOs.LearningPaths;

namespace MeandAI.Application.Services.Interfaces;

public interface ILearningPathsService
{
    Task<LearningPathDto> CreateAsync(CreateLearningPathRequest request, CancellationToken cancellationToken = default);
    Task<LearningPathDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<LearningPathDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<LearningPathDto>> GetForTargetAreaAsync(string targetArea, CancellationToken cancellationToken = default);
    Task<LearningPathDto?> UpdateAsync(Guid id, UpdateLearningPathRequest request, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<LearningPathStepDto?> AddStepAsync(Guid pathId, AddStepToPathRequest request, CancellationToken cancellationToken = default);
}
