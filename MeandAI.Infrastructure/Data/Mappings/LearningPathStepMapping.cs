using MeandAI.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MeandAI.Infrastructure.Data.Mappings;

public class LearningPathStepMapping : IEntityTypeConfiguration<LearningPathStep>
{
    public void Configure(EntityTypeBuilder<LearningPathStep> builder)
    {
        builder.ToTable("LearningPathSteps");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Title).IsRequired().HasMaxLength(200);
        builder.Property(x => x.Description).HasMaxLength(500);
        builder.Property(x => x.ExternalCourseUrl).HasMaxLength(500);
    }
}
