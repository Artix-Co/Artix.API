namespace Artix.API.Core.Contract.Features.Objects.Queries.GetObjectDetailsByIdClients;

using Artix.API.Core.Contract.Features.Museums.Queries.GetObjects;

public sealed record ObjectDetailsByIdClientDto(
    Guid Id,
    string Name,
    string? GeneralInformation,
    string? SpecialInformation,
    string? Model3DBase64,
    string? ImageBase64,
    List<HistoricalPeriodDto> HistoricalPeriods);
