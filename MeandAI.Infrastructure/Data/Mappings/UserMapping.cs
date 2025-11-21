using MeandAI.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MeandAI.Infrastructure.Data.Mappings;

public class UserMapping : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).IsRequired().HasMaxLength(150);
        builder.Property(x => x.Email).IsRequired().HasMaxLength(200);
        builder.Property(x => x.CurrentRole).HasMaxLength(150);
        builder.Property(x => x.DesiredArea).HasMaxLength(150);
        builder.Property(x => x.PasswordHash).IsRequired().HasMaxLength(500);
    }
}
