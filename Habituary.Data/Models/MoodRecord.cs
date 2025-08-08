namespace Habituary.Data.Models;

public class MoodRecord : BaseIORecord
{
    public string Name { get; set; }
    public string Emoji { get; set; }
    public bool SystemFlag { get; set; }
    public Guid UserIRN { get; set; }
    public UserRecord User { get; set; }
}