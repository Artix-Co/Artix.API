namespace Artix.API.Core.Contract.Features.Users.Client.Commands.ModifyProfile;

using Primitives.Handlers;

public sealed record ClientModifyProfileCommand(
    string? Username,
    string? Password,
    string? Email,
    string? PhoneNumber,
    string? DisplayName,
    string? ImageFileDataBase64,
    string? ImageFileName,
    string? ImageFileMimeType
) : ICommand;
