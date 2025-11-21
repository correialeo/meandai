using MeandAI.Domain.Entities;
using MeandAI.Domain.Repositories;
using MeandAI.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace MeandAI.Infrastructure.Repositories;

public class LearningPathRepository : ILearningPathRepository
{
    private readonly MeandAIDbContext _context;

    public LearningPathRepository(MeandAIDbContext context)
    {
        _context = context;
    }

    public async Task<LearningPath?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.LearningPaths
            .Include(lp => lp.Steps)
            .FirstOrDefaultAsync(lp => lp.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyCollection<LearningPath>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.LearningPaths
            .Include(lp => lp.Steps)
            .OrderBy(lp => lp.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<LearningPath>> GetForTargetAreaAsync(string targetArea, CancellationToken cancellationToken = default)
    {
        return await _context.LearningPaths
            .Where(lp => lp.TargetArea == targetArea)
            .Include(lp => lp.Steps)
            .OrderBy(lp => lp.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<LearningPath>> GetByIdsAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default)
    {
        return await _context.LearningPaths
            .Where(lp => ids.Contains(lp.Id))
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(LearningPath path, CancellationToken cancellationToken = default)
    {
        await _context.LearningPaths.AddAsync(path, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(LearningPath path, CancellationToken cancellationToken = default)
    {
        _context.LearningPaths.Update(path);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(LearningPath path, CancellationToken cancellationToken = default)
    {
        _context.LearningPaths.Remove(path);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
