namespace Artix.API.Infra.Sql.Data.Config.Write;

using Core.Domain.Entities.Museum;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

internal sealed class MuseumObjectCategoryWriteConfiguration : IEntityTypeConfiguration<ObjectType>
{
    public void Configure(EntityTypeBuilder<ObjectType> entity)
    {
        entity.ToTable("MuseumObjectCategories");

        entity.HasKey(moc => new { moc.MuseumObjectId, moc.CategoryId });;

       

        entity
            .HasOne(moc => moc.MuseumObject)
            .WithMany(mo => mo.MuseumObjectCategories)
            .HasForeignKey(moc => moc.MuseumObjectId);

        entity
            .HasOne(moc => moc.Type)
            .WithMany(c => c.ObjectTypes)
            .HasForeignKey(moc => moc.CategoryId);
 
        
    }
}
