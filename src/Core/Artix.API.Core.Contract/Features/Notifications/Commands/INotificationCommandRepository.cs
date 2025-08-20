namespace Artix.API.Core.Contract.Features.Notifications.Commands;

using Domain.Entities.Notification;
using Primitives.Repositories;

public interface INotificationCommandRepository : ICommandRepository<Notification>
{
}
