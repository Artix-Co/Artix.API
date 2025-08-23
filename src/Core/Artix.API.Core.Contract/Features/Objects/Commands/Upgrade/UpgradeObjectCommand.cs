namespace Artix.API.Core.Contract.Features.Objects.Commands.Upgrade;

using Primitives.Handlers;

public sealed record UpgradeObjectCommand(
    Guid Id,
    string? Name,
    string? GeneralInformation,
    string? SpecializedInformation,
    int? Tier,
    int? Version,
    string? Model3DFileDataBase64,
    string? Model3DFileName,
    string? Model3DFileMimeType,
    string? HistoricalPeriod
) : ICommand;
