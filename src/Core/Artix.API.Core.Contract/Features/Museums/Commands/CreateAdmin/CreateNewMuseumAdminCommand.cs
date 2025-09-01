namespace Artix.API.Core.Contract.Features.Museums.Commands.CreateAdmin;

using Primitives.Handlers;

public sealed record CreateNewMuseumAdminCommand(
    string Name,
    string Description,
    string? ImageFileDataBase64,
    string? ImageFileName,
    string? ImageFileMimeType
) : ICommand;
