using MeandAI.Domain.Entities;

namespace MeandAI.Domain.Repositories;

public interface ISkillRepository
{
    Task<Skill?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<Skill>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<Skill>> GetByCategoryAsync(string category, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<Skill>> GetByIdsAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default);
    Task AddAsync(Skill skill, CancellationToken cancellationToken = default);
    Task UpdateAsync(Skill skill, CancellationToken cancellationToken = default);
    Task DeleteAsync(Skill skill, CancellationToken cancellationToken = default);
}
