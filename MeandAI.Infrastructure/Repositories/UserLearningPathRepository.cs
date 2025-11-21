using MeandAI.Domain.Entities;
using MeandAI.Domain.Repositories;
using MeandAI.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace MeandAI.Infrastructure.Repositories;

public class UserLearningPathRepository : IUserLearningPathRepository
{
    private readonly MeandAIDbContext _context;

    public UserLearningPathRepository(MeandAIDbContext context)
    {
        _context = context;
    }

    public async Task<UserLearningPath?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.UserLearningPaths
            .FirstOrDefaultAsync(ulp => ulp.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyCollection<UserLearningPath>> GetByUserAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _context.UserLearningPaths
            .Where(ulp => ulp.UserId == userId)
            .ToListAsync(cancellationToken);
    }

    public async Task<UserLearningPath?> GetByUserAndPathAsync(Guid userId, Guid pathId, CancellationToken cancellationToken = default)
    {
        return await _context.UserLearningPaths
            .FirstOrDefaultAsync(ulp => ulp.UserId == userId && ulp.LearningPathId == pathId, cancellationToken);
    }

    public async Task AddAsync(UserLearningPath userPath, CancellationToken cancellationToken = default)
    {
        await _context.UserLearningPaths.AddAsync(userPath, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(UserLearningPath userPath, CancellationToken cancellationToken = default)
    {
        _context.UserLearningPaths.Update(userPath);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
