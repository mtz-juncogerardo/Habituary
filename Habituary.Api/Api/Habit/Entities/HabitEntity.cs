using Habituary.Api.HabitLog.Entities;
using Habituary.Api.Reminder.Entities;
using Habituary.Core.Interfaces;

namespace Habituary.Api.Habit.Entities
{
    public class HabitEntity : IEntity
    {
        public Guid UserIRN { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public Frequency Frequency { get; set; }
        public DateTime StartDate { get; set; }
        public IEnumerable<ReminderEntity> Reminders { get; set; }
        public IEnumerable<HabitLogEntity> HabitLogs { get; set; }
    }
}
