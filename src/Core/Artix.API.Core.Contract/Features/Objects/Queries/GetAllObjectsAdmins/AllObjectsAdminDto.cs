namespace Artix.API.Core.Contract.Features.Objects.Queries.GetAllObjectsAdmins;

using Domain.Entities.Object.Enums;

public record AllObjectsAdminDto(
    Guid Id,
    string Name,
    string? GeneralInformation,
    string? SpecialInformation,
    string MuseumName,
    ObjectSaleType ObjectSaleType,
    int? Version
);
