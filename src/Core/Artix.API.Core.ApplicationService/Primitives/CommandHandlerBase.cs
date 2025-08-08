namespace Artix.API.Core.ApplicationService.Primitives;

using System.Security.Claims;
using Contract.Primitives.Handlers;
using Domain.Entities.User;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

public abstract class CommandHandlerBase<TCommand> : ICommandHandler<TCommand>
    where TCommand : ICommand<long>
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly UserManager<AppUser> _userManager;

    protected CommandHandlerBase(IHttpContextAccessor httpContextAccessor, UserManager<AppUser> userManager)
    {
        this._httpContextAccessor = httpContextAccessor;
        this._userManager = userManager;
    }

    protected async Task<AppUser> GetCurrentUserAsync(CancellationToken cancellationToken)
    {
        var userIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
            throw new Exception("User is not authenticated or user ID is invalid.");

        var user = await _userManager.Users
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

        if (user == null)
            throw new UnauthorizedAccessException("User not found");

        return user;
    }

    public abstract Task<long> Handle(TCommand command, CancellationToken cancellationToken);
}
