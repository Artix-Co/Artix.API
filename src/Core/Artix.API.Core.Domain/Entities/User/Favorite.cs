namespace Artix.API.Core.Domain.Entities.User;

using Common;
using Exceptions;
using Museum;

public class Favorite : AggregateRoot
{
    public long UserId { get; private set; }
    public long? MuseumId { get; private set; }
    public long? ObjectId { get; private set; }

    public virtual AppUser User { get; private set; } = null!;
    public virtual Museum? Museum { get; private set; }
    public virtual Artix.API.Core.Domain.Entities.Object.Object? Object { get; private set; }

    private Favorite() { }

    private Favorite(long userId, long? museumId, long? objectId)
    {
        if (userId <= 0)
            throw DomainException.InvalidValue(nameof(userId));
        if ((museumId is null) == (objectId is null))
            throw DomainException.InvalidValue("Exactly one favorite target is required.");

        UserId = userId;
        MuseumId = museumId;
        ObjectId = objectId;
    }

    public static Favorite CreateForMuseum(long userId, long museumId) => new(userId, museumId, null);
    public static Favorite CreateForObject(long userId, long objectId) => new(userId, null, objectId);
}




