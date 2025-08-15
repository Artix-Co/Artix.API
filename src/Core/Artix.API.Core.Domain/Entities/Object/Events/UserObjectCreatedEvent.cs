namespace Artix.API.Core.Domain.Entities.Object.Events;

using DomainEvents;

public record UserObjectCreatedEvent(
    Guid BusinessId,
    long UserId,
    long ObjectId,
    int ScanCount,
    DateTime AcquiredAt,
    bool InCollection) : IDomainEvent;
