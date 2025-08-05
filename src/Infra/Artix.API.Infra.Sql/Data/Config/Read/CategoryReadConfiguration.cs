namespace Artix.API.Infra.Sql.Data.Config.Read;

using Core.Domain.Entities.Museum;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

internal sealed class CategoryReadConfiguration : BaseEntityConfiguration<Type>
{
    public void Configure(EntityTypeBuilder<Type> entity)
    {
        base.Configure(entity);

        entity.ToTable("Categories");

        entity.Property(e => e.Name)
            .HasMaxLength(255)
            .IsRequired();

        entity.Property(e => e.Description)
            .HasMaxLength(1000)
            .IsRequired(false);

        entity.HasMany(e => e.ObjectTypes)
            .WithOne(moc => moc.Type)
            .HasForeignKey(moc => moc.CategoryId)
            .OnDelete(DeleteBehavior.Cascade);

        entity.HasIndex(e => e.Name)
            .HasDatabaseName("IX_Categories_Name")
            .IsUnique();
    }
}
