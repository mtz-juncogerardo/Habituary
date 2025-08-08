namespace Habituary.Core.Interfaces;

public class ICurrentUser
{
    public Guid IRN { get; }
    public string Email { get; }
    public bool IsAuthenticated { get; }
}