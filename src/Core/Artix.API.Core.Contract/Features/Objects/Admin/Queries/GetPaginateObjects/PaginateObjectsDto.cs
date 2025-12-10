namespace Artix.API.Core.Contract.Features.Objects.Admin.Queries.GetPaginateObjects;

using Artix.API.Core.Domain.Entities.Object.Enums;

public sealed record PaginateObjectsDto(
    Guid Id,
    string Name,
    string? GeneralInformation,
    string? SpecialInformation,
    string MuseumName,
    ObjectSaleType ObjectSaleType,
    int? Version
);
