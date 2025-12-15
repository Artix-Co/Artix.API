namespace Artix.API.Infra.Sql.Data.Config.Write.User;

using Core.Domain.Entities.User;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
internal sealed class UserSessionWriteConfiguration 
    : BaseEntityConfiguration<UserSession>
{
    public override void Configure(EntityTypeBuilder<UserSession> entity)
    {
        base.Configure(entity);

        entity.ToTable("UserSessions");

        // --------------------
        // Ignore computed props
        // --------------------
        entity.Ignore(x => x.IsActive);

        // --------------------
        // Properties
        // --------------------
        entity.Property(x => x.JwtId)
            .IsRequired()
            .HasMaxLength(100);

        entity.Property(x => x.RefreshTokenHash)
            .IsRequired()
            .HasMaxLength(256);

        entity.Property(x => x.IpAddress)
            .IsRequired()
            .HasMaxLength(45);

        entity.Property(x => x.UserAgent)
            .IsRequired()
            .HasMaxLength(500);

        entity.Property(x => x.CreatedAt)
            .IsRequired();

        entity.Property(x => x.ExpiresAt)
            .IsRequired();

        entity.Property(x => x.RevokedAt)
            .IsRequired(false);

        // --------------------
        // Relationships
        // --------------------
        entity.HasOne(x => x.User)
            .WithMany(u => u.UserSessions)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade); 
        

        // --------------------
        // Indexes (critical)
        // --------------------
        entity.HasIndex(x => x.JwtId)
            .IsUnique()
            .HasDatabaseName("UX_UserSession_JwtId");

        entity.HasIndex(x => x.UserId)
            .HasDatabaseName("IX_UserSession_UserId");

        entity.HasIndex(x => x.ExpiresAt)
            .HasDatabaseName("IX_UserSession_ExpiresAt");

        entity.HasIndex(x => x.RevokedAt)
            .HasDatabaseName("IX_UserSession_RevokedAt");

        entity.HasIndex(x => new { x.UserId, x.RevokedAt })
            .HasDatabaseName("IX_UserSession_UserId_RevokedAt");

        // --------------------
        // Check constraints (data sanity)
        // --------------------
        entity.HasCheckConstraint(
            "CK_UserSession_ExpiresAfterCreated",
            "[ExpiresAt] > [CreatedAt]"
        );
    }
}
