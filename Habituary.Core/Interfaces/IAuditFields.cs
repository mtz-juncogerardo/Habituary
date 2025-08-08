namespace Habituary.Core.Interfaces;

public interface IAuditFields
{
    Guid IRN { get; set; }
    DateTime CreationDate { get; set; }
    DateTime LastModDate { get; set; }
    string LastModUserId { get; set; }
    string CreationUserId { get; set; }
}