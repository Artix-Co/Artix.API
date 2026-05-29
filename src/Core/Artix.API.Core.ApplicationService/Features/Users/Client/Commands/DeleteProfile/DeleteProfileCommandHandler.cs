namespace Artix.API.Core.ApplicationService.Features.Users.Client.Commands.DeleteProfile;

using Primitives;
using Artix.API.Core.Contract.Features.Users.Client.Commands.DeleteProfile;
using Domain.Entities.User;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

internal sealed class DeleteProfileCommandHandler : CommandHandlerBase<ClientDeleteProfileCommand>
{
    public DeleteProfileCommandHandler(
        IHttpContextAccessor httpContextAccessor,
        UserManager<AppUser> userManager,
        ILogger<CommandHandlerBase<ClientDeleteProfileCommand>> logger)
        : base(httpContextAccessor, userManager, logger)
    {
    }

    public override async Task<Guid> Handle(ClientDeleteProfileCommand command, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Delete profile(also remove all background histories) started for authenticated user.");


        var user = await GetCurrentUserAsync(cancellationToken);

        _logger.LogInformation(
            "Retrieved user {UserId} with BusinessId={BusinessId}",
            user.Id, user.BusinessId);


        var result = await _userManager.DeleteAsync(user);

        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            _logger.LogError(
                "Failed to delete user {UserId}. Errors: {Errors}",
                user.Id, errors);

            throw new InvalidOperationException($"Failed to delete user: {errors}");
        }

        _logger.LogInformation(
            "Successfully deleted user {UserId} with BusinessId={BusinessId}",
            user.Id, user.BusinessId);

        return user.BusinessId;
    }
}
