using Habituary.Api.User.Entities;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Habituary.Api.User.Request;

public class UserController : BaseController
{
    public UserController(IMediator mediator) : base(mediator)
    {
    }

    [HttpGet]
    public async Task<UserEntity> Get()
    {
        return await Mediator.Send(new UserRequest.Get());
    }

    [HttpGet(nameof(CheckRegistration))]
    public async Task<bool> CheckRegistration(string email)
    {
        return await Mediator.Send(new UserRequest.CheckRegistration(email));
    }

    [HttpPut("{irn}")]
    public async Task<UserEntity> Update(string irn, [FromBody]UserRequest.Update user)
    {
        return await Mediator.Send(user);
    }

    [HttpDelete]
    public async Task<IActionResult> Delete()
    {
        return await Mediator.Send(new UserRequest.Delete());
    }
}