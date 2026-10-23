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
        // Properties
        // --------------------

        entity.Property(x => x.SessionId)
            .IsRequired();

        entity.Property(x => x.JwtId)
            .HasMaxLength(64)
            .IsRequired();

        entity.Property(x => x.RefreshTokenFamilyId)
            .IsRequired();

        entity.Property(x => x.RefreshTokenHash)
            .HasMaxLength(64)
            .IsRequired();

        entity.Property(x => x.DeviceId)
            .HasMaxLength(128)
            .IsRequired();

        entity.Property(x => x.Fingerprint)
            .HasMaxLength(128)
            .IsRequired();

        entity.Property(x => x.IpAddress)
            .HasMaxLength(45)
            .IsRequired();

        entity.Property(x => x.UserAgent)
            .HasMaxLength(1024)
            .IsRequired();

        entity.Property(x => x.RefreshTokenExpiresAt)
            .IsRequired();

        entity.Property(x => x.CreatedAt)
            .IsRequired();

        entity.Property(x => x.LastSeenAt)
            .IsRequired();

        entity.Property(x => x.RevokedAt);

        entity.Property(x => x.RevokeReason)
            .HasMaxLength(256);

        // --------------------
        // Relationships
        // --------------------

        entity.HasOne(x => x.User)
            .WithMany(x => x.UserSessions)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // --------------------
        // Indexes
        // --------------------

        entity.HasIndex(x => x.UserId)
            .HasDatabaseName("IX_UserSessions_UserId");

        // entity.HasIndex(x => x.SessionId)
        //     .IsUnique()
        //     .HasDatabaseName("IX_UserSessions_SessionId");

        entity.HasIndex(x => x.JwtId)
            .IsUnique()
            .HasDatabaseName("IX_UserSessions_JwtId");

        entity.HasIndex(x => x.RefreshTokenHash)
            .IsUnique()
            .HasDatabaseName("IX_UserSessions_RefreshTokenHash");

        entity.HasIndex(x => x.RefreshTokenFamilyId)
            .HasDatabaseName("IX_UserSessions_RefreshTokenFamilyId");

        entity.HasIndex(x => x.DeviceId)
            .HasDatabaseName("IX_UserSessions_DeviceId");

        entity.HasIndex(x => new { x.UserId, x.RevokedAt })
            .HasDatabaseName("IX_UserSessions_UserId_RevokedAt");

        entity.HasIndex(x => new { x.UserId, x.LastSeenAt })
            .HasDatabaseName("IX_UserSessions_UserId_LastSeenAt");

        entity.HasIndex(x => new { x.UserId, x.RefreshTokenExpiresAt })
            .HasDatabaseName("IX_UserSessions_UserId_RefreshTokenExpiresAt");
    }
}
