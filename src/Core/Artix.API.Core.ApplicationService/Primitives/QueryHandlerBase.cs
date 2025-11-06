namespace Artix.API.Core.ApplicationService.Primitives;

using System.Security.Claims;
using Contract.Primitives.Handlers;
using Contract.Primitives.Models;
using Domain.Entities.User;
using Domain.Entities.User.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

public abstract class QueryHandlerBase<TQuery, TResponse> : IQueryHandler<TQuery, TResponse>
    where TQuery : IQuery<TResponse>
{
    protected readonly IHttpContextAccessor _httpContextAccessor;
    protected readonly UserManager<AppUser> _userManager;


    protected QueryHandlerBase(IHttpContextAccessor httpContextAccessor,
        UserManager<AppUser> userManager)
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

    protected async Task ValidateClientAccessAsync(AppUser user)
    {
        var roles = await _userManager.GetRolesAsync(user);
        if (!roles.Contains(nameof(Role.Client)))
            throw new UnauthorizedAccessException("User is not a Client.");

        var claims = await _userManager.GetClaimsAsync(user);
        if (!claims.Any(c => c is { Type: "ClientType", Value: nameof(ClientType.Emerald) }))
            throw new UnauthorizedAccessException("User is not an Emerald client.");
    }

    protected string? GetRemoteIp() =>
        _httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString();

    protected string? GetUserAgent() =>
        _httpContextAccessor.HttpContext?.Request.Headers["User-Agent"].ToString();


    public abstract Task<Result<TResponse>> Handle(TQuery query, CancellationToken cancellationToken);
}
