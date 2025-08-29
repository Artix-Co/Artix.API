namespace Artix.API.Core.Contract.Features.Objects.Queries.GetAllObjectsAdmins;

public record AllObjectsAdminDto(
    Guid Id,
    string Name,
    string? GeneralInformation,
    string? SpecialInformation,
    int? Version
);
