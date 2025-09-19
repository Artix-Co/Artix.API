namespace Artix.API.Core.Domain.Entities.Object.Events;

using DomainEvents;

public record RepeatUserScanEvent(
    Guid ObjectBusinessId,
    Guid UserBusinessId,
    long UserId,
    long ObjectId,
    int ScanCount,
    bool IsUpgraded) : IDomainEvent;
