namespace Artix.API.Core.Contract.Features.Objects.Commands.CreateAdmin;

using Domain.Entities.Object.Enums;
using Primitives.Handlers;

public sealed record CreateNewObjectAdminCommand(
    string Name,
    string? GeneralInformation,
    string? SpecializedInformation,
    string QrCode,
    int? Tier,
    int? Version,
    bool IsSpecial,
    bool IsHidden,
    ObjectSaleType ObjectSaleType,
    
    string? Model3DFileDataBase64,
    string? Model3DFileName,
    string? Model3DFileMimeType,
    
    string? ImageFileDataBase64,
    string? ImageFileName,
    string? ImageFileMimeType
) : ICommand;
