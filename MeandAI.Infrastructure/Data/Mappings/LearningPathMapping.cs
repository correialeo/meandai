using MeandAI.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MeandAI.Infrastructure.Data.Mappings;

public class LearningPathMapping : IEntityTypeConfiguration<LearningPath>
{
    public void Configure(EntityTypeBuilder<LearningPath> builder)
    {
        builder.ToTable("LearningPaths");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).IsRequired().HasMaxLength(150);
        builder.Property(x => x.TargetArea).IsRequired().HasMaxLength(150);
        builder.Property(x => x.Description).HasMaxLength(500);

        builder.HasMany(lp => lp.Steps)
               .WithOne()
               .HasForeignKey(s => s.LearningPathId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
