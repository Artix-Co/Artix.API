namespace Artix.API.Core.Contract.Features.Museums.Admin.Commands.CreateNew;

using Primitives.Handlers;

public sealed record CreateNewMuseumCommand(
    string Name,
    string Slug,
    string? Description,
    Guid? ImageUploadId
) : ICommand;
