namespace Artix.API.Core.Contract.Primitives.Authorization;

using System.Security.Claims;

public static class AdminAuthorization
{
    public static bool IsAllowed(ClaimsPrincipal user, string permission)
        => user.IsInRole("Admin") || user.IsInRole("SuperAdmin") || user.HasClaim("permission", permission);

    public static bool IsEmergencyAccessActive(ClaimsPrincipal user, DateTimeOffset now)
        => user.HasClaim("emergency_until", value =>
            DateTimeOffset.TryParse(value, out var until) && until > now);
}
