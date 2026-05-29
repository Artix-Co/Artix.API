namespace Artix.API.Core.Contract.Features.Objects.Client.Queries.GetPaginateObjects;

using Artix.API.Core.Domain.Entities.Object.ValueObjects;

public sealed record ClientPaginateObjectsDto(
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
    string Slug,
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
