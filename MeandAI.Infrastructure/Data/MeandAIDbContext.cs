using MeandAI.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace MeandAI.Infrastructure.Data;

public class MeandAIDbContext : DbContext
{
    public MeandAIDbContext(DbContextOptions<MeandAIDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Skill> Skills => Set<Skill>();
    public DbSet<LearningPath> LearningPaths => Set<LearningPath>();
    public DbSet<LearningPathStep> LearningPathSteps => Set<LearningPathStep>();
    public DbSet<UserSkill> UserSkills => Set<UserSkill>();
    public DbSet<UserLearningPath> UserLearningPaths => Set<UserLearningPath>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(MeandAIDbContext).Assembly);
    }
}
