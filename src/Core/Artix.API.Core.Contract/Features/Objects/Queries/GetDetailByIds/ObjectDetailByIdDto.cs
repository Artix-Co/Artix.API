namespace Artix.API.Core.Contract.Features.Objects.Queries.GetDetailByIds;

using Domain.Entities.ValueObjects;

public sealed class ObjectDetailByIdDto
{
    public long Id { get; set; }
    public string Name { get; set; }

    public string? GeneralInformation { get; set; }
    public string? SpecializedInformation { get; set; }
    public string? Model3DBase64 { get; set; }
    public List<HistoricalPeriodDto> HistoricalPeriods { get; set; }
}

public sealed class HistoricalPeriodDto
{
    public long Id { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public HistoricalDate? StartDate { get; set; }
    public HistoricalDate? EndDate { get; set; }
}
