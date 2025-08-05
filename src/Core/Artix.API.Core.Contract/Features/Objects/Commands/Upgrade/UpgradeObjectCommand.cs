namespace Artix.API.Core.Contract.Features.Objects.Commands.Upgrade;

using Primitives.Handlers;

public sealed class UpgradeObjectCommand : ICommand<long>
{
    public long Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? HistoricalPeriod { get; set; }
    public string? GeneralInformation { get; set; }
    public string? SpecializedInformation { get; set; }
    public string? Model3DBase64 { get; set; }
}
