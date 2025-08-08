using Habituary.Core.Interfaces;

namespace Habituary.Core.Extensions;

public static class AuditFieldsExtension
{
    public static void SetAudit(this IAuditFields record, string userId)
    {
        record.IRN = Guid.NewGuid();
        record.CreationUserId = userId;
        record.CreationDate = DateTime.UtcNow;
    }

    public static void UpdateExisting(this IAuditFields record, string userId)
    {
        record.LastModUserId = userId;
        record.LastModDate = DateTime.UtcNow;
    }
}