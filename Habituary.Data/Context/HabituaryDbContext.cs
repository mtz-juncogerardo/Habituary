using Habituary.Data.Config;
using Habituary.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Habituary.Data.Context;

public class HabituaryDbContext : DbContext
{
    public HabituaryDbContext(DbContextOptions<HabituaryDbContext> options) : base(options)
    {
    }

    public DbSet<UserRecord> Users { get; set; }
    public DbSet<HabitRecord> Habits { get; set; }
    public DbSet<ReminderRecord> Reminders { get; set; }
    public DbSet<HabitLogRecord> HabitLogs { get; set; }
    public DbSet<MoodRecord> Moods { get; set; }
    public DbSet<OauthCredentialRecord> OauthCredentials { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        BaseConfiguration.InitialConfig(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(UserRecordConfiguration).Assembly);
    }

    public override int SaveChanges()
    {
        //Set all DateTime properties to Utc for postgress database
        var dateTimeProperties = ChangeTracker.Entries()
            .SelectMany(entry => entry.Properties)
            .Where(property => property.CurrentValue is DateTime);

        foreach (var property in dateTimeProperties)
            property.CurrentValue = DateTime.SpecifyKind((DateTime)property.CurrentValue!, DateTimeKind.Utc);

        return base.SaveChanges();
    }
}