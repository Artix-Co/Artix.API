namespace Artix.API.Core.Domain.Entities.Object.Events;

using DomainEvents;

public record RepeatUserScanEvent(
    Guid BusinessId,
    long UserId,
    long ObjectId,
    int ScanCount,
    bool IsUpgraded) : IDomainEvent;
