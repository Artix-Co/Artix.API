namespace Artix.API.Core.Contract.Features.Museums.Admin.Commands.CreateNewMuseum;

using Primitives.Handlers;

public sealed record CreateNewMuseumCommand(
    string Name,
    string Description,
    Guid? ImageUploadId
) : ICommand;
