using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Habituary.Api;

[Authorize]
[Route("Api/[controller]")]
[ApiController]
public class BaseController : ControllerBase
{
    protected readonly IMediator Mediator;

    public BaseController(IMediator mediator)
    {
        Mediator = mediator;
    }
    
    [HttpGet("ping")]
    public IActionResult Ping()
    { 
        return  Ok("Pong"); 
    }
}