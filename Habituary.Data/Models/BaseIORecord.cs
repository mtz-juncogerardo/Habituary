using System.ComponentModel.DataAnnotations;
using Habituary.Core.Interfaces;

namespace Habituary.Data.Models;

public abstract class BaseIORecord : IAuditFields
{
    [Key] public Guid IRN { get; set; }
    public DateTime CreationDate { get; set; }
    public DateTime LastModDate { get; set; }
    public string? LastModUserId { get; set; }
    public string CreationUserId { get; set; }
}