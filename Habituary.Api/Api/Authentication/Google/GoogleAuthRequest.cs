using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Habituary.Api.Authentication.Google;

public static class GoogleAuthRequest
{
    public record Login(IUrlHelper url) : IRequest<IActionResult>;

    public record LoginCallback : IRequest<IActionResult>;

    public record Logout : IRequest<IActionResult>;
}