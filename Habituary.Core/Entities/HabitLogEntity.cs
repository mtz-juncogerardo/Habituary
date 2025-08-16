using Habituary.Core.Interfaces;

namespace Habituary.Core.Entities;

public class HabitLogEntity : IEntity
{
    public Guid HabitIRN { get; set; }
    public Guid MoodIRN { get; set; }
    public bool CompletedFlag { get; set; }
    public string Notes { get; set; } = string.Empty;
    public DateTime LogDate { get; set; }
    public MoodEntity Mood { get; set; }
}

