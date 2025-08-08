using Habituary.Core.Interfaces;

namespace Habituary.Api.Mood.Entities
{
    public class MoodEntity : IEntity
    {
        public string Name { get; set; }
        public string Emoji { get; set; }
        public bool SystemFlag { get; set; }
        public Guid UserIRN { get; set; }
    }
}
