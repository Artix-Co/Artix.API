namespace Artix.API.Core.Contract.Features.Objects.Admin.Queries.GetObjectDetailsById;

using Artix.API.Core.Domain.Entities.Object.Enums;
using Client.Queries.GetPaginateObjects;

public sealed record AdminObjectDetailsByIdDto(
    Guid Id,
    string Name,
    string? Description,
    string Slug,
    int? Version,
    int? Tier,
    bool IsSpecial,
    bool IsHidden,
    ObjectSaleType ObjectSaleType,
    DateTime CreatedAt,
    string? ImageUrl,
    string? Model3DUrl,
    string? GeneralInformationUrl,
    string? SpecialInformationUrl,
    List<TypeDto> ObjectTypes,
    List<HistoricalPeriodDto> HistoricalPeriods
);
