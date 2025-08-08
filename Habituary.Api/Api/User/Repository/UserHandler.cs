using Habituary.Api.User.Entities;
using Habituary.Api.User.Request;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Habituary.Api.User.Repository;

public class UserHandler : IRequestHandler<UserRequest.Get, UserEntity>,
    IRequestHandler<UserRequest.Update, UserEntity>,
    IRequestHandler<UserRequest.Delete, IActionResult>,
    IRequestHandler<UserRequest.CheckRegistration, bool>
{
    private readonly UserRepository _repository;

    public UserHandler(UserRepository userRepository)
    {
        _repository = userRepository;
    }

    public Task<bool> Handle(UserRequest.CheckRegistration request, CancellationToken cancellationToken)
    {
        return _repository.UserIsRegistered(request.email);
    }

    public Task<IActionResult> Handle(UserRequest.Delete request, CancellationToken cancellationToken)
    {
        return _repository.DeleteUser();
    }

    public Task<UserEntity> Handle(UserRequest.Get request, CancellationToken cancellationToken)
    {
        return _repository.GetLoggedUser();
    }

    public Task<UserEntity> Handle(UserRequest.Update request, CancellationToken cancellationToken)
    {
        return _repository.UpdateUser(request.UserEntity);
    }
}