namespace Artix.API.Infra.Sql.Repositories.Features.Notifications;

using Core.Contract.Features.Notifications.Commands;
using Core.Domain.Entities.Notification;
using Data.DbContexts;
using Microsoft.Extensions.Logging;
using Primitives;

public sealed class NotificationCommandRepository : CommandRepository<Notification>, INotificationCommandRepository
{
    private readonly ILogger<NotificationCommandRepository> _logger;

    public NotificationCommandRepository(ArtixCommandDbContext commandDbContext,
        ILogger<NotificationCommandRepository> logger)
        : base(commandDbContext)
    {
        _logger = logger;
    }
}
