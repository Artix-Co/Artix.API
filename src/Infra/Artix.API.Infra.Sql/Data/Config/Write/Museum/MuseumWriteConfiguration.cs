namespace Artix.API.Infra.Sql.Data.Config.Write.Museum;

using Core.Domain.Entities.Museum;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

internal sealed class MuseumWriteConfiguration : BaseEntityConfiguration<Museum>
{
    public override void Configure(EntityTypeBuilder<Museum> entity)
    {
        base.Configure(entity);

        entity.ToTable("Museums");

        entity.Property(e => e.Name)
            .HasMaxLength(100)
            .IsRequired();

        entity.Property(e => e.Description)
            .HasMaxLength(500)
            .IsRequired(false);
        
        entity.Property(e => e.Slug)
            .HasMaxLength(256)
            .IsRequired();

        entity.Property(e => e.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        entity
            .HasMany(m => m.MuseumObjects)
            .WithOne(mo => mo.Museum)
            .HasForeignKey(mo => mo.MuseumId)
            .OnDelete(DeleteBehavior.Cascade);

        // entity.HasIndex(e => e.Name)
        //     .HasDatabaseName("IX_Museums_Name")
        //     .IsUnique();
        
        entity.HasIndex(e => e.Slug)
            .HasDatabaseName("IX_Museums_Slug")
            .IsUnique();

        entity.HasIndex(e => e.IsActive)
            .HasDatabaseName("IX_Museums_IsActive");
    }
}
