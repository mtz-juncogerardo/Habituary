namespace Habituary.Core.Interfaces;

public interface ICurrentUser
{
    public Guid IRN { get; }
    public string Email { get; }
    public bool IsAuthenticated { get; }
}