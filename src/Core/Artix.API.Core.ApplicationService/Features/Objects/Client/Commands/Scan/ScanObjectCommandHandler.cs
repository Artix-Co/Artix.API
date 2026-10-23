namespace Artix.API.Core.ApplicationService.Features.Objects.Client.Commands.Scan;

using Contract.Features.Museums;
using Contract.Features.Objects;
using Contract.Features.Objects.Client.Commands.Scan;
using Domain.Entities.User;
using Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Primitives;

internal sealed class ScanObjectCommandHandler : CommandHandlerBase<ClientScanObjectCommand>
{
    private readonly IMuseumCommandRepository _museumCommandRepository;
    private readonly IObjectCommandRepository _objectCommandRepository;


    public ScanObjectCommandHandler(
        IHttpContextAccessor httpContextAccessor,
        UserManager<AppUser> userManager,
        ILogger<CommandHandlerBase<ClientScanObjectCommand>> logger,
        IMuseumCommandRepository museumCommandRepository,
        IObjectCommandRepository objectCommandRepository
    )
        : base(httpContextAccessor, userManager, logger)
    {
        this._museumCommandRepository = museumCommandRepository;
        this._objectCommandRepository = objectCommandRepository;
    }

    public override async Task<Guid> Handle(ClientScanObjectCommand command, CancellationToken cancellationToken)
    {
        var user = await this.GetCurrentUserAsync(cancellationToken);


        var museum = await this._museumCommandRepository.GetByIdAsync(command.MuseumId, cancellationToken);
        if (museum == null)
            throw ApplicationServiceNotFoundException.ForEntity(nameof(museum), command.MuseumId);

        var @object = museum.FindObject(command.ObjectId);
        if (@object == null)
            throw ApplicationServiceNotFoundException.ForEntity(nameof(@object), command.ObjectId);

        user.ProcessScan(@object);

        await this._objectCommandRepository.UpdateAsync(@object, cancellationToken);

        return @object.BusinessId;
    }
}
