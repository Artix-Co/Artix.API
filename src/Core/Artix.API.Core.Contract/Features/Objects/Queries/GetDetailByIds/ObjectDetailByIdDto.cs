namespace Artix.API.Core.Contract.Features.Objects.Queries.GetDetailByIds;

using Museums.Queries.GetObjects;

public sealed class ObjectDetailByIdDto
{
    public long Id { get; set; }
    public Guid BusinessId { get; set; }
    public string Name { get; set; }

    public string? GeneralInformation { get; set; }
    public string? SpecializedInformation { get; set; }
    public string? Model3DBase64 { get; set; }
    public List<HistoricalPeriodDto> HistoricalPeriods { get; set; }
}

 
