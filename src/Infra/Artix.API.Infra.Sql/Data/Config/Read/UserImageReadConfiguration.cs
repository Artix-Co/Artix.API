namespace Artix.API.Infra.Sql.Data.Config.Read;

using Core.Domain.Entities.User;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

internal sealed class UserImageReadConfiguration : IEntityTypeConfiguration<UserImage>
{
    public void Configure(EntityTypeBuilder<UserImage> entity)
    {
        entity.ToTable("UserImages");

        entity.HasKey(of => new { of.FileId, of.UserId });

        entity.Property(of => of.FileId).IsRequired();
        entity.Property(of => of.UserId).IsRequired();

        entity.HasOne(of => of.FileEntity)
            .WithMany(f => f.UserImages)
            .HasForeignKey(of => of.FileId)
            .OnDelete(DeleteBehavior.Restrict);

        entity.HasOne(of => of.AppUser)
            .WithMany(o => o.UserImages)
            .HasForeignKey(of => of.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        entity.HasIndex(of => of.FileId)
            .HasDatabaseName("IX_UserImageFiles_FileId");

        entity.HasIndex(of => of.UserId)
            .HasDatabaseName("IX_UserImageFiles_UserId");
    }
}
