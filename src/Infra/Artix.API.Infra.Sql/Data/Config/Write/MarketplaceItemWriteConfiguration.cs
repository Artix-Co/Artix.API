namespace Artix.API.Infra.Sql.Data.Config.Write;

using Core.Domain.Entities.JournalEntry;
using Core.Domain.Entities.MarketPlace;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

internal sealed class MarketplaceItemWriteConfiguration : BaseEntityConfiguration<MarketplaceItem>,
    IEntityTypeConfiguration<MarketplaceItem>
{
    public void Configure(EntityTypeBuilder<MarketplaceItem> entity)
    {
        // Table mapping
        entity.ToTable("MarketplaceItems");

        base.Configure(entity);


        entity.Property(e => e.PricePoints)
            .IsRequired(false);

        entity.Property(e => e.ListedAt)
            .IsRequired(false);

        entity.Property(e => e.IsSold)
            .IsRequired(false);

        entity.Property(e => e.ObjectId)
            .IsRequired(false);

        entity.Property(e => e.SellerId)
            .IsRequired(false);


        entity.HasOne(e => e.Object)
            .WithMany()
            .HasForeignKey(e => e.ObjectId)
            .OnDelete(DeleteBehavior.Restrict);


        entity.HasOne(e => e.Seller)
            .WithMany()
            .HasForeignKey(e => e.SellerId)
            .OnDelete(DeleteBehavior.Restrict);

        entity.HasIndex(e => e.ObjectId)
            .HasDatabaseName("IX_MarketplaceItems_ObjectId");

        entity.HasIndex(e => e.SellerId)
            .HasDatabaseName("IX_MarketplaceItems_SellerId");

        entity.HasIndex(e => e.IsSold)
            .HasDatabaseName("IX_MarketplaceItems_IsSold");
    }
}
