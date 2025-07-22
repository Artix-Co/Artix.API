

namespace Artix.API.Core.Domain.Entities.MarketPlace;

using _primitives;
using Museum;
using User;

public sealed class MarketplaceItem : BaseEntity
{
    public int? PricePoints { get; set; }

    public DateTime? ListedAt { get; set; }

    public bool? IsSold { get; set; }
 

    public long? ObjectId { get; set; }
    public MuseumObject? Object { get; set; }

    
    public long? SellerId { get; set; }
    public AppUser? Seller { get; set; }
}
