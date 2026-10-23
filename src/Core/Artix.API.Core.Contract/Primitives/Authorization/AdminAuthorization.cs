namespace Artix.API.Core.Contract.Primitives.Authorization;

using System.Security.Claims;

public static class AdminAuthorization
{
    public static bool IsAllowed(ClaimsPrincipal user, string permission)
        => user.IsInRole("Admin") || user.IsInRole("SuperAdmin") || user.HasClaim("permission", permission);
}
