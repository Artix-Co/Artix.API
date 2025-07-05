

namespace Artix.API.Core.Domain.Entities.MarketPlace;

using _primitives;
using Museum;
using User;

public class MarketplaceItem : BaseEntity
{
    public long? SellerId { get; set; }

    public long? ObjectId { get; set; }

    public int? PricePoints { get; set; }

    public DateTime? ListedAt { get; set; }

    public bool? IsSold { get; set; }
 

    public virtual MuseumObject? Object { get; set; }

    public virtual AppUser? Seller { get; set; }
}
