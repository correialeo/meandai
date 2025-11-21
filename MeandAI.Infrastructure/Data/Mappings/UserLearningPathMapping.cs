using MeandAI.Domain.Entities;
using MeandAI.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MeandAI.Infrastructure.Data.Mappings;

public class UserLearningPathMapping : IEntityTypeConfiguration<UserLearningPath>
{
    public void Configure(EntityTypeBuilder<UserLearningPath> builder)
    {
        builder.ToTable("UserLearningPaths");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Status)
               .HasConversion<int>()
               .IsRequired();
        builder.Property(x => x.CreatedAt).IsRequired();
        builder.HasIndex(x => new { x.UserId, x.LearningPathId }).IsUnique();
    }
}
