namespace Artix.API.Core.ApplicationService.Primitives;

using System.Security.Claims;
using Contract.Primitives.Handlers;
using Contract.Primitives.Models;
using Domain.Entities.User;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

public abstract class QueryHandlerBase<TQuery, TResponse> : IQueryHandler<TQuery, TResponse>
    where TQuery : IQuery<TResponse>
{
    protected readonly IMemoryCache _cache;
    protected readonly IHttpContextAccessor _httpContextAccessor;
    private readonly UserManager<AppUser> _userManager;

    protected QueryHandlerBase(IMemoryCache cache, IHttpContextAccessor httpContextAccessor, UserManager<AppUser> userManager)
    {
        this._cache = cache;
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
    
    public abstract Task<Result<TResponse>> Handle(TQuery query, CancellationToken cancellationToken);
}
