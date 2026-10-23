namespace Artix.API.Tests.Unit.Authorization;

using System.Security.Claims;
using Artix.API.Core.Contract.Primitives.Authorization;

public sealed class AdminPolicyTests
{
    [Fact]
    public void SuperAdmin_is_allowed()
    {
        var user = new ClaimsPrincipal(new ClaimsIdentity([new Claim(ClaimTypes.Role, "SuperAdmin")], "test"));
        Assert.True(AdminAuthorization.IsAllowed(user, AdminPermission.MuseumRead));
    }

    [Fact]
    public void Sensitive_permission_requires_mfa()
    {
        var user = new ClaimsPrincipal(new ClaimsIdentity([
            new Claim(ClaimTypes.Role, "SuperAdmin"),
            new Claim("permission", AdminPermission.SensitiveOperations)], "test"));
        Assert.False(AdminAuthorization.IsAllowed(user, AdminPermission.SensitiveOperations));
    }

    [Fact]
    public void Emergency_permission_expires()
    {
        var user = new ClaimsPrincipal(new ClaimsIdentity([
            new Claim("permission", AdminPermission.EmergencyAccess),
            new Claim("emergency_until", DateTimeOffset.UtcNow.AddMinutes(-1).ToString("O"))], "test"));
        Assert.False(AdminAuthorization.IsAllowed(user, AdminPermission.EmergencyAccess));
    }
}
