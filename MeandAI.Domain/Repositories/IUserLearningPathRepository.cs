using MeandAI.Domain.Entities;

namespace MeandAI.Domain.Repositories;

public interface IUserLearningPathRepository
{
    Task<UserLearningPath?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<UserLearningPath>> GetByUserAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<UserLearningPath?> GetByUserAndPathAsync(Guid userId, Guid pathId, CancellationToken cancellationToken = default);
    Task AddAsync(UserLearningPath userPath, CancellationToken cancellationToken = default);
    Task UpdateAsync(UserLearningPath userPath, CancellationToken cancellationToken = default);
}
