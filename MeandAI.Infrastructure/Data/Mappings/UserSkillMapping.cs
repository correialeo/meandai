using MeandAI.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MeandAI.Infrastructure.Data.Mappings;

public class UserSkillMapping : IEntityTypeConfiguration<UserSkill>
{
    public void Configure(EntityTypeBuilder<UserSkill> builder)
    {
        builder.ToTable("UserSkills");
        builder.HasKey(x => new { x.UserId, x.SkillId });
        builder.Property(x => x.Level).IsRequired();
        builder.Property(x => x.UpdatedAt).IsRequired();
    }
}
