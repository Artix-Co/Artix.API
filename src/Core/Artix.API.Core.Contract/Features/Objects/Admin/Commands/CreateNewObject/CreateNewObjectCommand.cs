namespace Artix.API.Core.Contract.Features.Objects.Admin.Commands.CreateNewObject;

using Primitives.Handlers;
using Artix.API.Core.Domain.Entities.Object.Enums;

public sealed record CreateNewObjectCommand(
    string Name,
    string? GeneralInformation,
    string? SpecializedInformation,
    string QrCode,
    int? Tier,
    int? Version,
    bool IsSpecial,
    bool IsHidden,
    ObjectSaleType ObjectSaleType,
    
    Guid? Model3DUploadId,
    Guid? ImageUploadId,
    
    Guid MuseumId
) : ICommand;
