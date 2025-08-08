using Habituary.Core.Types;

namespace Habituary.Data.Models;

public class ReminderRecord : BaseIORecord
{
    public Guid HabitIRN { get; set; }
    public HabitRecord Habit { get; set; }
    public TimeSpan TimeOfDay { get; set; }
    public ICollection<DaysOfWeek> DaysOfWeek { get; set; }
    public bool ActiveFlag { get; set; }
}