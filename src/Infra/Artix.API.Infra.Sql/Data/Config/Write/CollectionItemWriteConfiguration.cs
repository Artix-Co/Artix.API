namespace Artix.API.Infra.Sql.Data.Config.Write;

using Core.Domain.Entities.Collection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

internal sealed class CollectionItemWriteConfiguration : IEntityTypeConfiguration<CollectionItem>
{
    public void Configure(EntityTypeBuilder<CollectionItem> entity)
    {
        entity.ToTable("CollectionItems");

        entity.HasKey(ci => new { ci.CollectionId, ci.ObjectId });

        entity.HasOne(ci => ci.Collection)
            .WithMany(c => c.Items)
            .HasForeignKey(ci => ci.CollectionId)
            .OnDelete(DeleteBehavior.Cascade);

        entity.HasOne(ci => ci.Object)
            .WithMany() 
            .HasForeignKey(ci => ci.ObjectId)
            .OnDelete(DeleteBehavior.Restrict); 

        entity.HasIndex(ci => ci.CollectionId)
            .HasDatabaseName("IX_CollectionItems_CollectionId");

        entity.HasIndex(ci => ci.ObjectId)
            .HasDatabaseName("IX_CollectionItems_ObjectId");
    }
}
