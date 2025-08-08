using Habituary.Api.Reminder.Entities;
using MediatR;

namespace Habituary.Api.Reminder;

public class ReminderController: HabituaryApiControllerBase<ReminderEntity>
{
    public ReminderController(IMediator mediator) : base(mediator)
    {
    }
}