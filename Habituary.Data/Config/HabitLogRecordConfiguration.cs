using Habituary.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Habituary.Data.Config;

public class HabitLogRecordConfiguration : IEntityTypeConfiguration<HabitLogRecord>
{
    public void Configure(EntityTypeBuilder<HabitLogRecord> builder)
    {
        builder
            .HasOne(l => l.Habit)
            .WithMany(h => h.HabitLogs)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasOne(l => l.Mood)
            .WithMany();

        builder
            .Property(l => l.Notes)
            .HasColumnType("text")
            .HasDefaultValue(string.Empty);
    }
}
