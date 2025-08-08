using Habituary.Api.User.Entities;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Habituary.Api.User.Request;

public static class UserRequest
{
    public record Get : IRequest<UserEntity>;

    public record CheckRegistration(string email) : IRequest<bool>;

    public record Update(UserEntity UserEntity) : IRequest<UserEntity>;

    public record Delete : IRequest<IActionResult>;
}