namespace Artix.API.Infra.Sql.Data.Config.Write;

using Artix.API.Core.Domain.Entities.Museum;
using Core.Domain.Entities.Object;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

internal sealed class MuseumObjectCategoryWriteConfiguration : IEntityTypeConfiguration<ObjectType>
{
    public void Configure(EntityTypeBuilder<ObjectType> entity)
    {
        entity.ToTable("ObjectTypes");

        // Composite primary key for the relationship
        entity.HasKey(ot => new { ot.ObjectId, CategoryId = ot.TypeId });

        // Relationship with Object
        entity
            .HasOne(ot => ot.Object)
            .WithMany(o => o.ObjectTypes)
            .HasForeignKey(ot => ot.ObjectId)
            .OnDelete(DeleteBehavior.Cascade);

        // Relationship with Type
        entity
            .HasOne(ot => ot.Type)
            .WithMany(t => t.ObjectTypes)
            .HasForeignKey(ot => ot.TypeId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes for performance
        entity.HasIndex(ot => ot.ObjectId)
            .HasDatabaseName("IX_MuseumObjectCategories_ObjectId");

        entity.HasIndex(ot => ot.TypeId)
            .HasDatabaseName("IX_MuseumObjectCategories_CategoryId");
    }
}
