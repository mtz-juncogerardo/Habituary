using Habituary.Core.Interfaces;
using Habituary.Core.Types;
using System;
using System.Collections.Generic;

namespace Habituary.Core.Entities;

public class ReminderEntity : IEntity
{
    public Guid HabitIRN { get; set; }
    public TimeSpan TimeOfDay { get; set; }
    public ICollection<DaysOfWeek> DaysOfWeek { get; set; }
    public bool ActiveFlag { get; set; }
}

