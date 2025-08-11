namespace Artix.API.Core.Contract.Features.Objects.Commands.Upgrade;

using Microsoft.AspNetCore.Http;
using Primitives.Handlers;

public sealed class UpgradeObjectCommand: ICommand
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? GeneralInformation { get; set; }
    public string? SpecializedInformation { get; set; }
    public int? Tier { get; set; }
    public int? Version { get; set; }
    public string? Model3DFileDataBase64 { get; set; } // Base64 string for 3D model
    public string? Model3DFileName { get; set; }
    public string? Model3DFileMimeType { get; set; }
    // public string? HistoricalPeriod { get; set; } // Commented out as in original
}
