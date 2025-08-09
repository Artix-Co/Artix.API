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
    private readonly IMessageSerializer _messageSerializer;


    public ScanObjectCommandHandler(IHttpContextAccessor httpContextAccessor, UserManager<AppUser> userManager,
        IMuseumCommandRepository museumCommandRepository,
        IUserObjectCommandRepository userObjectCommandRepository, IMessageSerializer messageSerializer) : base(httpContextAccessor, userManager)
    {
        this._museumCommandRepository = museumCommandRepository;
        this._userObjectCommandRepository = userObjectCommandRepository;
        this._messageSerializer = messageSerializer;
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
            await this._userObjectCommandRepository.UpdateAsync(userObject, cancellationToken);
        }

        await this._museumCommandRepository.UpdateAsync(museum, cancellationToken);
        
        
        var factory = new RabbitMqConnectionFactory(); 
        var producer = new NotificationProducer(factory, this._messageSerializer);

        var message = new NotificationMessage(
            NotificationId: Guid.NewGuid(),
            UserId: 2,
            Title: "ماموریت جدید",
            Body: "یک ماموریت جدید داری!",
            Type: NotificationType.InApp,
            CreatedAt: DateTime.UtcNow,
            Metadata: null
        );

        await producer.PublishAsync(message, routingKey: "inapp.notifications");
        return userObject.Id;
    }
}
