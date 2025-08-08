using Habituary.Core.Interfaces;
using Habituary.Core.Types;

namespace Habituary.Api.User.Entities;

public class UserEntity : IEntity
{
    public string? Email { get; set; }
    public string Username { get; set; }
}