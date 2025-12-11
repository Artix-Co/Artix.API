namespace Artix.API.Core.Domain.Entities.Object.Events;

using DomainEvents;

public sealed record RepeatUserScanEvent(
    Guid ObjectBusinessId,
    Guid UserBusinessId,
    long UserId,
    long ObjectId,
    int ScanCount,
    bool IsUpgraded) : IDomainEvent;
