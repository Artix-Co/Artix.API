namespace Artix.API.Core.Domain.Entities.User;

using Artix.API.Core.Domain.Entities.Common;
using Artix.API.Core.Domain.Entities.Museum;

public class MarketplaceItem : BaseEntity
{
    public int? PricePoints { get; set; }

    public DateTime? ListedAt { get; set; }

    public bool? IsSold { get; set; }


    public long? ObjectId { get; set; }
    public virtual MuseumObject? Object { get; set; }


    public long? SellerId { get; set; }
    public virtual AppUser? Seller { get; set; }
}
