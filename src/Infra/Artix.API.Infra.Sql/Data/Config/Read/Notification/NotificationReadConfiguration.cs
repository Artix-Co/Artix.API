namespace Artix.API.Infra.Sql.Data.Config.Read.Notification;

using Artix.API.Core.Domain.Entities.Notification;
using Artix.API.Core.Domain.Entities.Notification.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

internal sealed class NotificationReadConfiguration : BaseEntityConfiguration<Notification>
{
    public void Configure(EntityTypeBuilder<Notification> entity)
    {
        base.Configure(entity);

        
        entity.ToTable("Notifications");

        entity.HasKey(n => n.Id);

        entity.Property(n => n.Title)
            .IsRequired()
            .HasMaxLength(200);

        entity.Property(n => n.Body)
            .IsRequired();

        entity.Property(n => n.Type)
            .IsRequired();

        entity.Property(n => n.Metadata)
            .HasMaxLength(2000);

        entity.Property(n => n.SenderId);

        entity.Property(n => n.IsBroadcast)
            .IsRequired();

        entity.Property(n => n.Status)
            .HasDefaultValue(NotificationStatus.Pending)
            .IsRequired();

        entity.Property(n => n.SentAt);

        entity.Property(n => n.ExpirationDate);

        entity.Property(n => n.Priority)
            .HasDefaultValue(Priority.Medium)
            .IsRequired();

        entity.Property(n => n.FailedAttempts)
            .HasDefaultValue(0);

        entity.Property(n => n.LastErrorMessage)
            .HasMaxLength(2000);

        entity.Property(n => n.CreatedAt)
            .IsRequired();

        entity.HasMany(n => n.UserNotifications)
            .WithOne(un => un.Notification)
            .HasForeignKey(un => un.NotificationId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
