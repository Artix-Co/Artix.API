namespace Artix.API.Tests.Unit.Authorization;

using System.Security.Claims;
using Artix.API.Core.Contract.Primitives.Authorization;
using Artix.API.Infra.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

public sealed class AdminPolicyTests
{
    [Fact]
    public async Task SuperAdmin_satisfies_museum_policy()
    {
        var requirement = await GetRequirementAsync(AdminPermission.MuseumRead);
        var principal = new ClaimsPrincipal(new ClaimsIdentity([
            new Claim(ClaimTypes.Role, "SuperAdmin")], "test"));

        var context = new AuthorizationHandlerContext([requirement], principal, null);
        await requirement.Handler(context);

        Assert.Empty(context.PendingRequirements);
    }

    [Fact]
    public async Task Assigned_permission_satisfies_museum_policy()
    {
        var requirement = await GetRequirementAsync(AdminPermission.MuseumRead);
        var principal = new ClaimsPrincipal(new ClaimsIdentity([
            new Claim("permission", AdminPermission.MuseumRead)], "test"));

        var context = new AuthorizationHandlerContext([requirement], principal, null);
        await requirement.Handler(context);

        Assert.Empty(context.PendingRequirements);
    }

    private static async Task<AssertionRequirement> GetRequirementAsync(string policyName)
    {
        var configuration = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["Authentication:Issuer"] = "test",
            ["Authentication:Audience"] = "test",
            ["Authentication:IssuerSigningKey"] = "this-key-is-long-enough-for-tests-123456789"
        }).Build();
        var services = new ServiceCollection();
        services.AddIdentityService(configuration);
        await using var provider = services.BuildServiceProvider();
        var policyProvider = provider.GetRequiredService<IAuthorizationPolicyProvider>();
        var policy = await policyProvider.GetPolicyAsync(policyName);
        return Assert.IsType<AssertionRequirement>(Assert.Single(policy!.Requirements));
    }
}
