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
    private readonly INotificationService _notificationService;

    public ScanObjectCommandHandler(
        IHttpContextAccessor httpContextAccessor,
        UserManager<AppUser> userManager,
        IMuseumCommandRepository museumCommandRepository,
        IObjectCommandRepository objectCommandRepository, INotificationService notificationService)
        : base(httpContextAccessor, userManager)
    {
        _museumCommandRepository = museumCommandRepository;
        _objectCommandRepository = objectCommandRepository;
        _notificationService = notificationService;
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

        // منطق ثبت سفارش
        var notification = new NotificationMessage(
            NotificationId: Guid.NewGuid(),
            UserId: user.Id,
            Title: "سفارش شما ثبت شد",
            Body: $"سفارش #{@object.BusinessId} با موفقیت ثبت شد.",
            Type: NotificationType.Push,
            CreatedAt: DateTime.UtcNow,
            Metadata: null
        );
        await _notificationService.SendUserNotificationAsync(notification);

        return @object.BusinessId;
    }
}
