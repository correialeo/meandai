using MeandAI.Domain.Entities;

namespace MeandAI.Domain.Repositories;

public interface ILearningPathRepository
{
    Task<LearningPath?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<LearningPath>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<LearningPath>> GetForTargetAreaAsync(string targetArea, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<LearningPath>> GetByIdsAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default);
    Task AddAsync(LearningPath path, CancellationToken cancellationToken = default);
    Task UpdateAsync(LearningPath path, CancellationToken cancellationToken = default);
    Task DeleteAsync(LearningPath path, CancellationToken cancellationToken = default);
}
