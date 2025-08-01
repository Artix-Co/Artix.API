namespace Artix.API.Infra.Sql.Data.Config.Read;

using Core.Domain.Entities.Museum;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

internal sealed class MuseumReadConfiguration : BaseEntityConfiguration<Museum>
{
    public void Configure(EntityTypeBuilder<Museum> entity)
    {
        base.Configure(entity);

        entity.ToTable("Museums");

        entity.Property(e => e.Name)
            .HasMaxLength(100)
            .IsRequired();

        entity.Property(e => e.Description)
            .HasMaxLength(500)
            .IsRequired(false);

        entity.Property(e => e.IsActive)
            .IsRequired()
            .HasDefaultValue(true);


        entity
            .HasMany(m => m.MuseumObjects)
            .WithOne(mo => mo.Museum)
            .HasForeignKey(mo => mo.MuseumId);


        entity.HasIndex(e => e.Name)
            .HasDatabaseName("IX_Museums_Name")
            .IsUnique();

        entity.HasIndex(e => e.IsActive)
            .HasDatabaseName("IX_Museums_IsActive");
    }
}
