using Microsoft.AspNetCore.Mvc;
using Habituary.Api.HabitLog.Entities;
using MediatR;

namespace Habituary.Api.HabitLog.Request
{
    [ApiController]
    [Route("api/[controller]")]
    public class HabitLogController : HabituaryApiControllerBase<HabitLogEntity>
    {
        public HabitLogController(IMediator mediator) : base(mediator)
        {
        }
    }
}
