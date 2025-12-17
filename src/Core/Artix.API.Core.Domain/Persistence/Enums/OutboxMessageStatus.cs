namespace Artix.API.Core.Domain.Persistence.Enums;

public enum OutboxMessageStatus
{
    Pending,
    Failed,
    Processed,
    Dead
}
