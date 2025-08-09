using Habituary.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Habituary.Data.Config;

public class ReminderRecordConfiguration : IEntityTypeConfiguration<ReminderRecord>
{
    public void Configure(EntityTypeBuilder<ReminderRecord> builder)
    {
        builder
            .HasOne(r => r.Habit)
            .WithMany(h => h.Reminders)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

