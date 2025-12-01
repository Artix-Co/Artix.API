namespace Artix.API.Core.ApplicationService.Features.Museums.Commands.CreateAdmin;

using Contract.Configs.FileSettings;
using Contract.Features.Museums.Commands;
using Contract.Features.Museums.Commands.CreateAdmin;
using Domain.Entities.Museum;
using Domain.Entities.User;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Primitives;

internal sealed class CreateNewMuseumAdminCommandHandler : CommandHandlerBase<CreateNewMuseumAdminCommand>
{
    private readonly IMuseumCommandRepository _museumCommandRepository;
    private readonly string[] _allowedImageMimeTypes;

    public CreateNewMuseumAdminCommandHandler(IHttpContextAccessor httpContextAccessor,
        UserManager<AppUser> userManager, IMuseumCommandRepository museumCommandRepository,
        IOptions<FileSettings> options) : base(
        httpContextAccessor,
        userManager)
    {
        this._museumCommandRepository = museumCommandRepository;
        this._allowedImageMimeTypes = options.Value.AllowedImageMimeTypes;
    }

    public override async Task<Guid> Handle(CreateNewMuseumAdminCommand command, CancellationToken cancellationToken)
    {
        var user = await GetCurrentUserAsync(cancellationToken);
        var museum = Museum.Create(command.Name, command.Description);


 

        await this._museumCommandRepository.InsertAsync(museum, cancellationToken);

        return museum.BusinessId;
    }
}
