namespace Artix.API.Core.Contract.Features.Objects.Client.Queries.GetObjectDetailsById;

using GetPaginateObjects;

public sealed record ClientObjectDetailsByIdDto(
    Guid Id,
    string Name,
    string? GeneralInformation,
    string? SpecialInformation,
    string? Model3DUrl,
    string? ImageUrl,
    string? GeneralInformationUrl,
    string? SpecialInformationUrl,
    List<HistoricalPeriodDto> HistoricalPeriods);
