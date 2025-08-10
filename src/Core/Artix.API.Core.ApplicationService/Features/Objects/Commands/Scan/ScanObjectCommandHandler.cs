namespace Artix.API.Core.ApplicationService.Features.Objects.Commands.Scan;

using Contract.Features.Museums.Commands;
using Contract.Features.Objects.Commands.Scan;
using Contract.Features.UserObjects.Commands;
using Domain.Entities.User;
using Exceptions;
using Infra.RabbitMQ.Interfaces;
using Infra.RabbitMQ.Models;
using Infra.RabbitMQ.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Primitives;

internal sealed class ScanObjectCommandHandler : CommandHandlerBase<ScanObjectCommand>
{
    private readonly IMuseumCommandRepository _museumCommandRepository;
    private readonly IUserObjectCommandRepository _userObjectCommandRepository;
    private readonly INotificationProducer _notificationProducer;


    public ScanObjectCommandHandler(IHttpContextAccessor httpContextAccessor, UserManager<AppUser> userManager,
        IMuseumCommandRepository museumCommandRepository,
        IUserObjectCommandRepository userObjectCommandRepository, INotificationProducer notificationProducer) : base(
        httpContextAccessor, userManager)
    {
        this._museumCommandRepository = museumCommandRepository;
        this._userObjectCommandRepository = userObjectCommandRepository;
        this._notificationProducer = notificationProducer;
    }

    public override async Task<long> Handle(ScanObjectCommand command, CancellationToken cancellationToken)
    {
        var user = await GetCurrentUserAsync(cancellationToken);
        var museum = await this._museumCommandRepository.GetByIdAsync(command.MuseumId, cancellationToken);
        if (museum == null)
            throw ApplicationServiceNotFoundException.ForEntity(nameof(museum), command.MuseumId);

        var museumObject = museum.MuseumObjects.FirstOrDefault(o => o.Id == command.ObjectId);
        if (museumObject == null)
            throw ApplicationServiceNotFoundException.ForEntity(nameof(museumObject), command.ObjectId);

        var userObject = user.UserObjects.FirstOrDefault(uo => uo.UserId == user.Id && uo.ObjectId == command.ObjectId);

        if (userObject == null)
        {
            userObject = UserObject.Create(user.Id, museumObject.Id);
            userObject.RecordScan();
            userObject.SetInCollection(true);
            userObject.SetAcquiredAt(DateTime.UtcNow);
            await this._userObjectCommandRepository.InsertAsync(userObject, cancellationToken);
        }
        else
        {
            userObject.RecordScan();
            userObject.Upgrade();
            await this._userObjectCommandRepository.UpdateAsync(userObject, cancellationToken);
        }

        await this._museumCommandRepository.UpdateAsync(museum, cancellationToken);


        var message = new NotificationMessage(
            NotificationId: Guid.NewGuid(),
            UserId: 2,
            Title: "ماموریت جدید",
            Body: "یک ماموریت جدید داری!",
            Type: NotificationType.InApp,
            CreatedAt: DateTime.UtcNow,
            Metadata: null
        );
        await _notificationProducer.PublishAsync(message, "inapp.notifications");

        return userObject.Id;
    }
}
