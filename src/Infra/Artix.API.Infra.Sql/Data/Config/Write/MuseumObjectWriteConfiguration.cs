namespace Artix.API.Infra.Sql.Data.Config.Write;

using Core.Domain.Entities.Museum;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

internal sealed class MuseumObjectWriteConfiguration : BaseEntityConfiguration<MuseumObject>
{
    public override void Configure(EntityTypeBuilder<MuseumObject> entity)
    {
        base.Configure(entity);

        entity.ToTable("MuseumObjects");

        // Properties
        entity.Property(mo => mo.MuseumId)
            .IsRequired();

        entity.Property(mo => mo.ObjectId)
            .IsRequired();

        // Relationships
        entity
            .HasOne(mo => mo.Museum)
            .WithMany(m => m.MuseumObjects)
            .HasForeignKey(mo => mo.MuseumId)
            .OnDelete(DeleteBehavior.Cascade);

        entity
            .HasOne(mo => mo.Object)
            .WithMany() // No navigation property in Object for MuseumObjects
            .HasForeignKey(mo => mo.ObjectId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        entity.HasIndex(mo => mo.MuseumId)
            .HasDatabaseName("IX_MuseumObjects_MuseumId");

        // TODO: uncomment it when production, i commented it to seeding db with redundant object
        // entity.HasIndex(mo => mo.ObjectId)
        //     .HasDatabaseName("IX_MuseumObjects_ObjectId")
        //     .IsUnique(); // Assuming each Object can be linked to only one Museum
    }
}
