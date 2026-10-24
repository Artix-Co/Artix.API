namespace Artix.API.Infra.Sql.Data.Config.Read.User;

using Artix.API.Core.Domain.Entities.User;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

internal sealed class FavoriteReadConfiguration : BaseEntityConfiguration<Favorite>
{
    public void Configure(EntityTypeBuilder<Favorite> entity)
    {
        base.Configure(entity);
        entity.ToTable("Favorites");
        entity.Property(x => x.UserId).IsRequired();
        entity.HasOne(x => x.User).WithMany().HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Cascade);
        entity.HasOne(x => x.Museum).WithMany().HasForeignKey(x => x.MuseumId).OnDelete(DeleteBehavior.Cascade);
        entity.HasOne(x => x.Object).WithMany().HasForeignKey(x => x.ObjectId).OnDelete(DeleteBehavior.Cascade);
        entity.HasCheckConstraint("CK_Favorites_OneTarget", "([MuseumId] IS NOT NULL AND [ObjectId] IS NULL) OR ([MuseumId] IS NULL AND [ObjectId] IS NOT NULL)");
        entity.HasIndex(x => new { x.UserId, x.MuseumId, x.ObjectId }).IsUnique();
        entity.HasIndex(x => x.UserId);
    }
}
