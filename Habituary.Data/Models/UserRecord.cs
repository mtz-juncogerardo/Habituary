namespace Habituary.Data.Models;

public class UserRecord : BaseIORecord
{
    public string Username { get; set; }
    public string Email { get; set; }
    public ICollection<HabitRecord> Habits { get; set; }
    public ICollection<OauthCredentialRecord> OauthCredentials { get; set; }
    public ICollection<MoodRecord> Moods { get; set; }
}