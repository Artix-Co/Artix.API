namespace Artix.API.Core.Contract.Features.Museums.Queries.GetObjects;

using Domain.ValueObjects;

public sealed record AllObjectDto(
    Guid Id,
    string Name,
    string? Description,
    Guid MuseumId,
    string? QRCode,
    bool IsSpecial,
    bool IsHidden,
    int? Tier,
    int? Version,
    DateTime CreatedAt,
    List<TypeDto> Types,
    List<HistoricalPeriodDto> HistoricalPeriods
);


public sealed record TypeDto(
    Guid Id,
    string Name,
    string? Description);

public sealed record HistoricalPeriodDto(
    Guid Id,
    string Name,
    string? Description,
    HistoricalDate? StartDate,
    HistoricalDate? EndDate);
