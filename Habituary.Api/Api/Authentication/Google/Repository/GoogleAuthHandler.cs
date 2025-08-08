using Habituary.Core.Extensions;
using Habituary.Core.Interfaces;
using Habituary.Core.Types;
using Habituary.Data.Context;
using Habituary.Data.Models;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Habituary.Api.Authentication.Google;

public class GoogleAuthHandler(GoogleAuthRepository repository)
    : IRequestHandler<GoogleAuthRequest.Login, IActionResult>,
        IRequestHandler<GoogleAuthRequest.LoginCallback, IActionResult>,
        IRequestHandler<GoogleAuthRequest.Logout, IActionResult>
{
    public async Task<IActionResult> Handle(GoogleAuthRequest.Login request, CancellationToken cancellationToken)
    {
        return await repository.Login(request.url.Action("LoginCallback"));
    }

    public async Task<IActionResult> Handle(GoogleAuthRequest.LoginCallback request,
        CancellationToken cancellationToken)
    {
        return await repository.LoginCallback();
    }

    public async Task<IActionResult> Handle(GoogleAuthRequest.Logout request, CancellationToken cancellationToken)
    {
        return await repository.Logout();
    }
}