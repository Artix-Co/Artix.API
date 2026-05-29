namespace Artix.API.Core.ApplicationService.Features.Objects.Client.Commands.Scan;

using Contract.Features.Museums;
using Contract.Features.Objects;
using Contract.Features.Objects.Client.Commands.Scan;
using Contract.Primitives.Infra.Redis;
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
    private readonly IRequestRatePolicy _requestRatePolicy;


    public ScanObjectCommandHandler(
        IHttpContextAccessor httpContextAccessor,
        UserManager<AppUser> userManager,
        ILogger<CommandHandlerBase<ClientScanObjectCommand>> logger,
        IMuseumCommandRepository museumCommandRepository,
        IObjectCommandRepository objectCommandRepository,
        IRequestRatePolicy requestRatePolicy
    )
        : base(httpContextAccessor, userManager, logger)
    {
        this._museumCommandRepository = museumCommandRepository;
        this._objectCommandRepository = objectCommandRepository;
        this._requestRatePolicy = requestRatePolicy;
    }

    public override async Task<Guid> Handle(ClientScanObjectCommand command, CancellationToken cancellationToken)
    {
        var user = await this.GetCurrentUserAsync(cancellationToken);
        Console.WriteLine("user scans:", user.UserScans.Count.ToString());
        var rateKey = $"scan:{user.Id}";
        var allowed = await this._requestRatePolicy.IsAllowedAsync(rateKey, cancellationToken);

        if (!allowed)
            throw new TooManyRequestsException("You are scanning too fast. Please wait a few seconds.");

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
