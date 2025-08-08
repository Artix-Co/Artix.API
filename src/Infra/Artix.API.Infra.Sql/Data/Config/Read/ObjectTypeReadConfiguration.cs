namespace Artix.API.Infra.Sql.Data.Config.Read;

using Artix.API.Core.Domain.Entities.Museum;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

internal sealed class ObjectTypeReadConfiguration: IEntityTypeConfiguration<ObjectType>
{
    public void Configure(EntityTypeBuilder<ObjectType> entity)
    {
        entity.ToTable("ObjectTypes");

        entity.HasKey(ot => new { ot.TypeId, ot.ObjectId });

        entity.Property(ot => ot.TypeId).IsRequired();
        entity.Property(ot => ot.ObjectId).IsRequired();

        entity.HasOne(ot => ot.Type)
            .WithMany(f => f.ObjectTypes)
            .HasForeignKey(ot => ot.TypeId)
            .OnDelete(DeleteBehavior.Restrict);

        entity.HasOne(ot => ot.Object)
            .WithMany(o => o.ObjectTypes)
            .HasForeignKey(ot => ot.ObjectId)
            .OnDelete(DeleteBehavior.Restrict);

        entity.HasIndex(ot => ot.TypeId)
            .HasDatabaseName("IX_ObjectTypes_TypeId");

        entity.HasIndex(ot => ot.ObjectId)
            .HasDatabaseName("IX_ObjectTypes_ObjectId");
    }
}
