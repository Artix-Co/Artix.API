namespace Artix.API.Core.Contract.Features.Objects.Queries.GetDetailByIds;

using Museums.Queries.GetObjects;

public sealed record ObjectDetailByIdDto(
    Guid BusinessId,
    string Name,
    string? GeneralInformation,
    string? SpecializedInformation,
    string? Model3DBase64,
    string? ImageBase64,
    List<HistoricalPeriodDto> HistoricalPeriods);
