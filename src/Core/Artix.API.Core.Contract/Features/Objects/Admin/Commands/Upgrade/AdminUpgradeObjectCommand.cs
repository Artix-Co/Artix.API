namespace Artix.API.Core.Contract.Features.Objects.Admin.Commands.Upgrade;

using Primitives.Handlers;

public sealed record AdminUpgradeObjectCommand(
    Guid Id,
    string? Name,
    // string? GeneralInformation,
    // string? SpecializedInformation,
    int? Tier,
    int? Version,
    Guid? Model3DUploadId,
    Guid? ImageUploadId
) : ICommand;
