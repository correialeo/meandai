using MeandAI.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MeandAI.Infrastructure.Data.Mappings;

public class SkillMapping : IEntityTypeConfiguration<Skill>
{
    public void Configure(EntityTypeBuilder<Skill> builder)
    {
        builder.ToTable("Skills");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).IsRequired().HasMaxLength(150);
        builder.Property(x => x.Category).IsRequired().HasMaxLength(100);
        builder.Property(x => x.Description).HasMaxLength(500);
    }
}
