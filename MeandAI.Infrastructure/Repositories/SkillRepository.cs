using MeandAI.Domain.Entities;
using MeandAI.Domain.Repositories;
using MeandAI.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace MeandAI.Infrastructure.Repositories;

public class SkillRepository : ISkillRepository
{
    private readonly MeandAIDbContext _context;

    public SkillRepository(MeandAIDbContext context)
    {
        _context = context;
    }

    public async Task<Skill?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Skills.FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyCollection<Skill>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Skills
            .OrderBy(s => s.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<Skill>> GetByCategoryAsync(string category, CancellationToken cancellationToken = default)
    {
        return await _context.Skills
            .Where(s => s.Category == category)
            .OrderBy(s => s.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<Skill>> GetByIdsAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default)
    {
        return await _context.Skills
            .Where(s => ids.Contains(s.Id))
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Skill skill, CancellationToken cancellationToken = default)
    {
        await _context.Skills.AddAsync(skill, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Skill skill, CancellationToken cancellationToken = default)
    {
        _context.Skills.Update(skill);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Skill skill, CancellationToken cancellationToken = default)
    {
        _context.Skills.Remove(skill);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
