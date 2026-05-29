namespace Artix.API.Core.ApplicationService.Features.Objects.Admin.Commands.Remove;

using Contract.Features.Objects;
using Contract.Features.Objects.Admin.Commands.Remove;
using Domain.Entities.User;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Primitives;

internal sealed class RemoveObjectCommandHandler : CommandHandlerBase<AdminRemoveObjectCommand>
{
    private readonly IObjectCommandRepository _objectCommandRepository;

    public RemoveObjectCommandHandler(IHttpContextAccessor httpContextAccessor, UserManager<AppUser> userManager,
        ILogger<CommandHandlerBase<AdminRemoveObjectCommand>> logger,
        IObjectCommandRepository objectCommandRepository) : base(httpContextAccessor, userManager, logger)
    {
        this._objectCommandRepository = objectCommandRepository;
    }

    public override async Task<Guid> Handle(AdminRemoveObjectCommand command, CancellationToken cancellationToken)
    {
        await this._objectCommandRepository.DeleteAsync(command.Id, cancellationToken);
        return command.Id;
    }
}
