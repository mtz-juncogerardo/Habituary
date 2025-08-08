namespace Habituary.Core.Interfaces;

public class IEntity
{
    //IRN must be optional cuz the create operation will not have it
    public string? IRN { get; set; }
}