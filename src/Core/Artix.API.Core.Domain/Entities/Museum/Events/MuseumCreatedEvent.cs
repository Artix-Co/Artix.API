namespace Artix.API.Core.Domain.Entities.Museum.Events;

using DomainEvents;

public record MuseumCreatedEvent(Guid BusinessId, string Name, string? Description, bool IsActive) : IDomainEvent;
