namespace Artix.API.Core.ApplicationService.Primitives;

using System.Security.Claims;
using Contract.Primitives.Handlers;
using Domain.Entities.User;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

public abstract class CommandHandlerBase<TCommand> : ICommandHandler<TCommand>
    where TCommand : ICommand
{
    protected readonly IHttpContextAccessor _httpContextAccessor;
    protected readonly UserManager<AppUser> _userManager;
    protected readonly ILogger<CommandHandlerBase<TCommand>> _logger;

    protected CommandHandlerBase(
        IHttpContextAccessor httpContextAccessor,
        UserManager<AppUser> userManager,
        ILogger<CommandHandlerBase<TCommand>> logger)
    {
        _httpContextAccessor = httpContextAccessor;
        _userManager = userManager;
        _logger = logger;
    }

    protected async Task<AppUser> GetCurrentUserAsync(CancellationToken cancellationToken)
    {
        _logger.LogDebug("Attempting to retrieve current user from HttpContext.");

        var userIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userIdClaim))
        {
            _logger.LogWarning("User ID claim is missing from HttpContext.");
            throw new UnauthorizedAccessException("User is not authenticated.");
        }

        if (!long.TryParse(userIdClaim, out var userId))
        {
            _logger.LogWarning("Invalid User ID claim format: {UserIdClaim}", userIdClaim);
            throw new UnauthorizedAccessException("User ID is invalid.");
        }

        _logger.LogDebug("Parsed user ID: {UserId}", userId);

        var user = await _userManager.FindByIdAsync(userId.ToString(System.Globalization.CultureInfo.InvariantCulture));

        if (user == null)
        {
            _logger.LogWarning("User with ID {UserId} not found in database.", userId);
            throw new UnauthorizedAccessException("User not found.");
        }

        _logger.LogInformation("Successfully retrieved user {UserName} with ID {UserId}.", user.UserName, user.Id);
        return user;
    }


    public abstract Task<Guid> Handle(TCommand command, CancellationToken cancellationToken);
}
