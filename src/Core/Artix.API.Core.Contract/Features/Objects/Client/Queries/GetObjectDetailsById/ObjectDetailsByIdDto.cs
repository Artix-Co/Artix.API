namespace Artix.API.Core.Contract.Features.Objects.Client.Queries.GetObjectDetailsById;

using GetPaginateObjects;

public sealed record ObjectDetailsByIdDto(
    Guid Id,
    string Name,
    string? GeneralInformation,
    string? SpecialInformation,
    string? Model3DUrl,
    string? ImageUrl,
    List<HistoricalPeriodDto> HistoricalPeriods);
