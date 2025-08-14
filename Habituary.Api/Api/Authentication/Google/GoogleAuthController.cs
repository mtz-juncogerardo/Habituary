using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Habituary.Api.Authentication.Google;

[ApiController]
[Route("Api/[controller]")]
public class GoogleAuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public GoogleAuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet(nameof(Login))]
    public async Task<IActionResult> Login()
    {
        return await _mediator.Send(new GoogleAuthRequest.Login(Url));
    }

    [Authorize]
    [HttpGet(nameof(LoginCallback))]
    public async Task<IActionResult> LoginCallback()
    {
        return await _mediator.Send(new GoogleAuthRequest.LoginCallback());
    }

    [HttpGet(nameof(Logout))]
    public async Task<IActionResult> Logout()
    {
        return await _mediator.Send(new GoogleAuthRequest.Logout());
    }
}