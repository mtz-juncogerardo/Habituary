using Habituary.Core.Interfaces;
using System;

namespace Habituary.Core.Entities;

public class MoodEntity : IEntity
{
    public string Name { get; set; }
    public string Emoji { get; set; }
    public bool SystemFlag { get; set; }
    public Guid UserIRN { get; set; }
}

