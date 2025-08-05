namespace Artix.API.Core.Contract.Features.Objects.Queries.GetDetailByIds;

public sealed class ObjectDetailByIdDto
{
    public string Name { get; set; }
    public string HistoricalPeriod { get; set; }
    public string? GeneralInformation { get; set; }
    public string? SpecializedInformation { get; set; }
    public string Model3DBase64 { get; set; }
}
