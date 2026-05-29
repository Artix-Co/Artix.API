namespace Artix.API.Core.Contract.Features.Objects.Admin.Commands.CreateNewObject;

using Primitives.Handlers;
using Artix.API.Core.Domain.Entities.Object.Enums;

public sealed record AdminCreateNewObjectCommand(
    string Name,
    string? Description,
    Guid? GeneralInformationUploadId,
    Guid? SpecializedInformationUploadId,
    string Slug,
    int? Tier,
    int? Version,
    bool IsSpecial,
    bool IsHidden,
    ObjectSaleType ObjectSaleType,
    Guid? Model3DUploadId,
    Guid? ImageUploadId,
    Guid MuseumId
) : ICommand;
