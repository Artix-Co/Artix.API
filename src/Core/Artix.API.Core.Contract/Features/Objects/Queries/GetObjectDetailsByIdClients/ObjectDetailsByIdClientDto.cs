namespace Artix.API.Core.Contract.Features.Objects.Queries.GetObjectDetailsByIdClients;

using Artix.API.Core.Contract.Features.Museums.Queries.GetObjects;

public sealed record ObjectDetailsByIdClientDto(
    Guid Id,
    string Name,
    string? GeneralInformation,
    string? SpecialInformation,
    string? Model3DUrl,
    string? ImageUrl,
    List<HistoricalPeriodDto> HistoricalPeriods);
