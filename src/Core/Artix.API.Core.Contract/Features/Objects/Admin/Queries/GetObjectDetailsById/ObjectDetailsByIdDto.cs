namespace Artix.API.Core.Contract.Features.Objects.Admin.Queries.GetObjectDetailsById;

using Artix.API.Core.Domain.Entities.Object.Enums;
using Client.Queries.GetAll;

public sealed record ObjectDetailsByIdDto(
    Guid Id,
    string Name,
    string? GeneralInformation,
    string? SpecialInformation,
    int? Version,
    int? Tier,
    bool IsSpecial,
    bool IsHidden,
    ObjectSaleType ObjectSaleType,
    DateTime CreatedAt,
    string? ImageBase64,
    string? Model3DBase64,
    List<TypeDto> ObjectTypes,
    List<HistoricalPeriodDto> HistoricalPeriods
);
