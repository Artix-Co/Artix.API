namespace Artix.API.Core.ApplicationService.Features.Objects.Commands.Upgrade;

using Contract.Features.Objects.Commands;
using Contract.Features.Objects.Commands.Upgrade;
using Contract.Features.Objects.Queries;
using Exceptions;
using Microsoft.AspNetCore.Http;
using Primitives;

internal sealed class UpgradeObjectCommandHandler : CommandHandlerBase<UpgradeObjectCommand>
{
    private readonly IObjectCommandRepository _objectCommandRepository;
    private readonly IObjectQueryRepository _objectQueryRepository;
    public UpgradeObjectCommandHandler(IHttpContextAccessor httpContextAccessor, IObjectCommandRepository objectCommandRepository, IObjectQueryRepository objectQueryRepository) : base(httpContextAccessor)
    {
        this._objectCommandRepository = objectCommandRepository;
        this._objectQueryRepository = objectQueryRepository;
    }

    public override async Task<long> Handle(UpgradeObjectCommand command, CancellationToken cancellationToken)
    {
        var @object = await this._objectQueryRepository.GetByIdAsync(command.Id, cancellationToken);

        if (@object == null)
        {
            throw ApplicationServiceNotFoundException.ForEntity(nameof(@object), command.Id);
        }

        
        
        
        return @object.Id;
    }
}
