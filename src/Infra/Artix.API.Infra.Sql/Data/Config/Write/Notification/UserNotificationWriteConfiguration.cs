namespace Artix.API.Infra.Sql.Data.Config.Write.Notification;

using Artix.API.Core.Domain.Entities.Notification;
using Artix.API.Core.Domain.Entities.Notification.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

internal sealed class UserNotificationWriteConfiguration : BaseEntityConfiguration<UserNotification>
{
    public void Configure(EntityTypeBuilder<UserNotification> entity)
    {
        base.Configure(entity);


        entity.ToTable("UserNotifications");

        entity.HasKey(un => un.Id);

        entity.Property(un => un.UserId)
            .IsRequired();

        entity.HasOne(un => un.User)
            .WithMany()
            .HasForeignKey(un => un.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        entity.Property(un => un.NotificationId)
            .IsRequired();

        entity.Property(un => un.IsRead)
            .HasDefaultValue(false)
            .IsRequired();

        entity.Property(un => un.ReadAt);

        entity.Property(un => un.DeliveryStatus)
            .HasDefaultValue(DeliveryStatus.Pending)
            .IsRequired();

        entity.Property(un => un.DeliveredAt);
    }
}
