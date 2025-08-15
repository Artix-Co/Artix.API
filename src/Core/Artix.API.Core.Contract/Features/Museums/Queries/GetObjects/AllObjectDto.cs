namespace Artix.API.Core.Contract.Features.Museums.Queries.GetObjects;

using Domain.ValueObjects;

public sealed class AllObjectDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public Guid MuseumId { get; set; }
    public string? QRCode { get; set; } 
    public bool IsSpecial { get; set; }
    public bool IsHidden { get; set; }
    public int? Tier { get; set; }
    public int? Version { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<TypeDto> Types { get; set; }
    public List<HistoricalPeriodDto> HistoricalPeriods { get; set; }
}


public sealed class TypeDto
{
    public Guid Id { get; set; }
    public string Name { get;  set; }
    public string? Description { get;  set; }
}

public sealed class HistoricalPeriodDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public HistoricalDate? StartDate { get; set; }
    public HistoricalDate? EndDate { get; set; }
}
