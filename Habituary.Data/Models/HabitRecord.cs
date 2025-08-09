using Habituary.Core.Interfaces;

namespace Habituary.Data.Models;

public class HabitRecord : BaseIORecord
{
    public Guid UserIRN { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public UserRecord User { get; set; }
    public Frequency Frequency { get; set; }
    public DateTime StartDate { get; set; }
    public ICollection<ReminderRecord> Reminders { get; set; }
    public ICollection<HabitLogRecord> HabitLogs { get; set; }
}