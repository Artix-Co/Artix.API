namespace Artix.API.Core.ApplicationService.Features.Museums.Admin.Commands.Remove;

using Contract.Features.Museums;
using Contract.Features.Museums.Admin.Commands.Remove;
using Domain.Entities.User;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Primitives;

internal sealed class RemoveMuseumCommandHandler : CommandHandlerBase<AdminRemoveMuseumCommand>
{
    private readonly IMuseumCommandRepository _museumCommandRepository;

    public RemoveMuseumCommandHandler(IHttpContextAccessor httpContextAccessor, UserManager<AppUser> userManager,
        ILogger<CommandHandlerBase<AdminRemoveMuseumCommand>> logger,
        IMuseumCommandRepository museumCommandRepository) : base(httpContextAccessor, userManager, logger)
    {
        this._museumCommandRepository = museumCommandRepository;
    }

    public override async Task<Guid> Handle(AdminRemoveMuseumCommand command, CancellationToken cancellationToken)
    {
        await this._museumCommandRepository.DeleteAsync(command.Id, cancellationToken);
        return command.Id;
    }
}
