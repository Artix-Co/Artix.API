namespace Artix.API.Core.Domain.Entities.Common;


public interface IEntity
{
    long Id { get; }
    DateTime CreatedAt { get; }
    DateTime? ModifiedAt { get; }
    bool IsDeleted { get; }
    Guid BusinessId { get; }
}
