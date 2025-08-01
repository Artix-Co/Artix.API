namespace Artix.API.Infra.Sql.Data.Config.Read;

using Core.Domain.Entities.Collection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

internal sealed class CollectionReadConfiguration : BaseEntityConfiguration<Collection>
{
    public void Configure(EntityTypeBuilder<Collection> entity)
    {
        base.Configure(entity);

        entity.ToTable("Collections");

        entity.Property(e => e.Name)
            .HasMaxLength(100)
            .IsRequired(false);

        entity.Property(e => e.Description)
            .HasMaxLength(500)
            .IsRequired(false);

        entity.Property(e => e.IsPublic)
            .IsRequired();

        entity.Property(e => e.UserId)
            .IsRequired();

        entity.HasOne(e => e.User)
            .WithMany()
            .HasForeignKey(e => e.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        entity.HasMany(e => e.Items)
            .WithOne()
            .HasForeignKey("CollectionId")
            .OnDelete(DeleteBehavior.Restrict);

        entity.HasIndex(e => e.UserId)
            .HasDatabaseName("IX_Collections_UserId");

        entity.HasIndex(e => e.Name)
            .HasDatabaseName("IX_Collections_Name")
            .IsUnique();
    }
}
