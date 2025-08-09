using Habituary.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Habituary.Data.Config;

public class MoodRecordConfiguration : IEntityTypeConfiguration<MoodRecord>
{
    public void Configure(EntityTypeBuilder<MoodRecord> builder)
    {
        builder
            .HasOne(m => m.User)
            .WithMany(u => u.Moods)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .Property(m => m.Emoji)
            .HasColumnType("varchar(10)")
            .HasDefaultValue(string.Empty);
    }
}
