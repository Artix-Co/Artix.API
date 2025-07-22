namespace Artix.API.Core.Domain.Entities.User;

using Microsoft.AspNetCore.Identity;

public sealed class AppRole : IdentityRole<long>
{
    public AppRole(string roleName) : base(roleName)
    {
    }

    public AppRole() : base()
    {
    }
}
