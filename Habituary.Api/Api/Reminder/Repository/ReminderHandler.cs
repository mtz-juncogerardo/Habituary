using Habituary.Api.Reminder.Entities;
using Habituary.Api.Repository;
using Habituary.Core.Interfaces;
using Habituary.Data.Context;
using Habituary.Data.Models;

namespace Habituary.Api.Reminder.Repository;

public class ReminderHandler : HabituaryApiHandler<ReminderEntity, ReminderRecord>
{
    public ReminderHandler(HabituaryDbContext context, ICurrentUser currentUser) : base(context, currentUser)
    {
    }
}