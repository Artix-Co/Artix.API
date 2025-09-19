namespace Artix.API.Core.ApplicationService.Features.Objects.Commands.Scan;

using Contract.Features.Museums.Commands;
using Contract.Features.Notifications.Commands.AddUserNotification;
using Contract.Features.Objects.Commands;
using Contract.Features.Objects.Commands.Scan;
using Domain.Entities.Notification.Enums;
using Domain.Entities.User;
using DomainService.Interfaces.Notification;
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
   

    public ScanObjectCommandHandler(
        IHttpContextAccessor httpContextAccessor,
        UserManager<AppUser> userManager,
        IMuseumCommandRepository museumCommandRepository,
        IObjectCommandRepository objectCommandRepository)
        : base(httpContextAccessor, userManager)
    {
        _museumCommandRepository = museumCommandRepository;
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

        var userObject = user.UserScans.FirstOrDefault(uo => uo.UserId == user.Id && uo.ObjectId == @object.Id);

        if (userObject == null)
        {
            @object.FirstTimeUserScan(user.Id);
        }
        else
        {
            @object.RepeatUserScan(userObject);
        }

        await _objectCommandRepository.UpdateAsync(@object, cancellationToken);
        
        return @object.BusinessId;
    }
}
