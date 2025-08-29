namespace Artix.API.Core.Contract.Features.Objects.Queries.GetObjectDetailsByIdAdmins;

using Domain.Entities.Object.Enums;
using Museums.Queries.GetObjects;

public record ObjectDetailsByIdAdminDto(
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
