using Habituary.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Habituary.Data.Config;

public class HabitRecordConfiguration : IEntityTypeConfiguration<HabitRecord>
{
    public void Configure(EntityTypeBuilder<HabitRecord> builder)
    {
        builder
            .HasMany(r => r.Reminders)
            .WithOne(r => r.Habit)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasMany(h => h.HabitLogs)
            .WithOne(l => l.Habit)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasOne(r => r.User)
            .WithMany(u => u.Habits)
            .HasForeignKey(r => r.UserIRN)
            .OnDelete(DeleteBehavior.Cascade);
    }
}