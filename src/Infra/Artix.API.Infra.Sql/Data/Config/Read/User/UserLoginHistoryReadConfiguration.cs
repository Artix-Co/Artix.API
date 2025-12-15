namespace Artix.API.Infra.Sql.Data.Config.Read.User;

using Core.Domain.Entities.User;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

internal sealed class UserLoginHistoryReadConfiguration: BaseEntityConfiguration<UserSession> 
{
    public void Configure(EntityTypeBuilder<UserSession> entity)
    {
        base.Configure(entity);

        entity.ToTable("UserLoginHistories");
        

        entity.Property(ulh => ulh.IpAddress)
            .IsRequired()
            .HasMaxLength(45); // Supports IPv4 (15 chars) and IPv6 (45 chars)

        entity.Property(ulh => ulh.UserAgent)
            .IsRequired()
            .HasMaxLength(500); // Adjust length based on requirements

 

        // Foreign key relationship with AppUser
        entity.HasOne(ulh => ulh.User)
            .WithMany(u => u.UserSessions) // Assumes AppUser has a collection: ICollection<UserLoginHistory> LoginHistories
            .HasForeignKey(ulh => ulh.UserId)
            .OnDelete(DeleteBehavior.Cascade); // Deletes login history if user is deleted (adjust based on requirements)

        // Index configurations
        entity.HasIndex(ulh => ulh.UserId)
            .HasDatabaseName("IX_UserLoginHistory_UserId");

        entity.HasIndex(ulh => ulh.CreatedAt)
            .HasDatabaseName("IX_UserLoginHistory_CreatedAt");

        entity.HasIndex(ulh => new { ulh.UserId, ulh.CreatedAt })
            .HasDatabaseName("IX_UserLoginHistory_UserId_CreatedAt");
    }
}
