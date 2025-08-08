using Microsoft.AspNetCore.Mvc;
using Habituary.Api.Habit.Entities;
using MediatR;

namespace Habituary.Api.Habit.Request
{
    [ApiController]
    [Route("api/[controller]")]
    public class HabitController : HabituaryApiControllerBase<HabitEntity>
    {
        public HabitController(IMediator mediator) : base(mediator)
        {
        }
    }
}
