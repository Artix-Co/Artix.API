namespace Artix.API.Infra.Sql.Data.Config.Write;

using Core.Domain.Entities.Museum;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

internal sealed class MuseumWriteConfiguration : BaseEntityConfiguration<Museum>,
    IEntityTypeConfiguration<Museum>
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


        entity.HasMany(e => e.MuseumObjects)
            .WithOne()
            .HasForeignKey("MuseumId")
            .OnDelete(DeleteBehavior.Restrict);


        entity.HasIndex(e => e.Name)
            .HasDatabaseName("IX_Museums_Name")
            .IsUnique();

        entity.HasIndex(e => e.IsActive)
            .HasDatabaseName("IX_Museums_IsActive");
    }
}
