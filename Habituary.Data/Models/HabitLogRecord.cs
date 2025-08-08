namespace Habituary.Data.Models;

public class HabitLogRecord : BaseIORecord
{
    public Guid HabitIRN { get; set; }
    public Guid MoodIRN { get; set; }
    public bool CompletedFlag { get; set; }
    public string Notes { get; set; } = string.Empty;
    public DateTime LogDate { get; set; }
    public HabitRecord Habit { get; set; }
    public MoodRecord Mood { get; set; }
}