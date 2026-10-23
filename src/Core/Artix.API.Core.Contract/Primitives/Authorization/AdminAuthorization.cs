namespace Artix.API.Core.Contract.Primitives.Authorization;

using System.Security.Claims;

public static class AdminAuthorization
{
    public static bool IsAllowed(ClaimsPrincipal user, string permission)
    {
        if (permission == AdminPermission.SensitiveOperations && !user.HasClaim("amr", "mfa"))
            return false;

        if (permission == AdminPermission.EmergencyAccess && !IsEmergencyAccessActive(user, DateTimeOffset.UtcNow))
            return false;

        return user.IsInRole("Admin") || user.IsInRole("SuperAdmin") || user.HasClaim("permission", permission);
    }

    public static bool IsEmergencyAccessActive(ClaimsPrincipal user, DateTimeOffset now)
        => user.Claims.Any(claim => claim.Type == "emergency_until" &&
            DateTimeOffset.TryParse(claim.Value, out var until) && until > now);
}
