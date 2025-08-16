namespace Artix.API.Core.ApplicationService.Features.Objects.Commands.Scan;

using Contract.Features.Museums.Commands;
using Contract.Features.Objects.Commands;
using Contract.Features.Objects.Commands.Scan;
using Domain.Entities.User;
using Exceptions;
using Infra.RabbitMQ.Interfaces.Notification;
using Infra.RabbitMQ.Models.Notification;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Primitives;

internal sealed class ScanObjectCommandHandler : CommandHandlerBase<ScanObjectCommand>
{
    private readonly IMuseumCommandRepository _museumCommandRepository;
    private readonly IObjectCommandRepository _objectCommandRepository;
    private readonly INotificationProducer _notificationProducer;

    public ScanObjectCommandHandler(
        IHttpContextAccessor httpContextAccessor,
        UserManager<AppUser> userManager,
        IMuseumCommandRepository museumCommandRepository,
        INotificationProducer notificationProducer,
        IObjectCommandRepository objectCommandRepository)
        : base(httpContextAccessor, userManager)
    {
        _museumCommandRepository = museumCommandRepository;
        _notificationProducer = notificationProducer;
        _objectCommandRepository = objectCommandRepository;
    }

    public override async Task<Guid> Handle(ScanObjectCommand command, CancellationToken cancellationToken)
    {
        var user = await GetCurrentUserAsync(cancellationToken);
        var museum = await this._museumCommandRepository.GetByIdAsync(command.MuseumId, cancellationToken);
        if (museum == null)
            throw ApplicationServiceNotFoundException.ForEntity(nameof(museum), command.MuseumId);

        var @object = museum.FindObject(command.ObjectId);
        if (@object == null)
            throw ApplicationServiceNotFoundException.ForEntity(nameof(@object), command.ObjectId);

        var userObject = user.UserObjects.FirstOrDefault(uo => uo.UserId == user.Id && uo.ObjectId == @object.Id);

        if (userObject == null)
        {
            @object.ProcessUserInteraction(user.Id);
        }
        else
        {
            @object.UpgradeUserObject(userObject);
        }

        await _objectCommandRepository.UpdateAsync(@object, cancellationToken);

        var message = new NotificationMessage(
            NotificationId: Guid.NewGuid(),
            UserId: user.Id,
            Title: "ماموریت جدید",
            Body: "یک ماموریت جدید داری!",
            Type: NotificationType.InApp,
            CreatedAt: DateTime.UtcNow,
            Metadata: null
        );
        await _notificationProducer.PublishAsync(message, "inapp.notifications");

        return @object.BusinessId;
    }
}
