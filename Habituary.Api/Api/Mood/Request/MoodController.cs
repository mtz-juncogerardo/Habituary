using Microsoft.AspNetCore.Mvc;
using Habituary.Core.Entities;
using MediatR;

namespace Habituary.Api.Mood.Request
{
    [ApiController]
    [Route("api/[controller]")]
    public class MoodController : HabituaryApiControllerBase<MoodEntity>
    {
        public MoodController(IMediator mediator) : base(mediator)
        {
        }
    }
}
