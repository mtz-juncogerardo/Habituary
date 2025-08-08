using Habituary.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Habituary.Data.Config;

public class UserRecordConfiguration : IEntityTypeConfiguration<UserRecord>
{
    public void Configure(EntityTypeBuilder<UserRecord> builder)
    {
        builder
            .HasIndex(r => new { r.Email })
            .IsUnique();
        builder
            .HasMany(r => r.Habits)
            .WithOne(r => r.User)
            .OnDelete(DeleteBehavior.Cascade);
        builder
            .HasMany(r => r.OauthCredentials)
            .WithOne(r => r.User)
            .OnDelete(DeleteBehavior.Cascade);
    }
}