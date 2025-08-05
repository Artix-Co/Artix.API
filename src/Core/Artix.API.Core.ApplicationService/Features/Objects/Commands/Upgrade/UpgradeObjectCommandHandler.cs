namespace Artix.API.Core.ApplicationService.Features.Objects.Commands.Upgrade;

using Contract.Features.Objects.Commands;
using Contract.Features.Objects.Commands.Upgrade;
using Contract.Features.Objects.Queries;
using Exceptions;
using Microsoft.AspNetCore.Http;
using Primitives;
using Object = Domain.Entities.Museum.Object;

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

    var objectBuilder = Object.CreateBuilder();

    if (!string.IsNullOrWhiteSpace(command.Name))
    {
        objectBuilder.WithName(command.Name);
    }
    
    if (!string.IsNullOrWhiteSpace(command.GeneralInformation))
    {
        objectBuilder.WithGeneralInformation(command.GeneralInformation);
    }

    if (!string.IsNullOrWhiteSpace(command.SpecializedInformation))
    {
        objectBuilder.WithSpecialInformation(command.SpecializedInformation);
    }

    if (command.Tier.HasValue)
    {
        objectBuilder.WithTier(command.Tier.Value);
    }

    if (command.Version.HasValue)
    {
        objectBuilder.WithVersion(command.Version.Value);
    }

    if (!string.IsNullOrWhiteSpace(command.Model3DBase64))
    {
        objectBuilder.WithModel3DBase64(command.Model3DBase64);
    }

    // TODO: Handle HistoricalPeriod if provided
    // if (!string.IsNullOrWhiteSpace(command.HistoricalPeriod))
    // {
    //     // Assuming HistoricalPeriod is a name that needs to be resolved to a HistoricalPeriod entity
    //     var historicalPeriod = await this._historicalPeriodQueryRepository.GetByNameAsync(command.HistoricalPeriod, cancellationToken);
    //     if (historicalPeriod != null)
    //     {
    //         var objectHistoricalPeriod = ObjectHistoricalPeriod.Create(@object, historicalPeriod);
    //         objectBuilder.WithHistoricalPeriod(objectHistoricalPeriod);
    //     }
    //     else
    //     {
    //         throw ApplicationServiceNotFoundException.ForEntity(nameof(HistoricalPeriod), command.HistoricalPeriod);
    //     }
    // }

    // Apply special status if explicitly set in the command
    // Assuming a property or logic to determine IsSpecial (not directly in command, so we skip unless specified)
    // Example: if (command.IsSpecial.HasValue) objectBuilder.AsSpecial(); // Add if command is extended

    // Build the updated object
    var updatedObject = objectBuilder.Build();

    // Update the object in the repository
    await this._objectCommandRepository.UpdateAsync(updatedObject, cancellationToken);

    return @object.Id;
}
}
