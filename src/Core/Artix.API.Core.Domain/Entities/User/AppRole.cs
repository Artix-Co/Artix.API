namespace Artix.API.Core.Domain.Entities.User;

using Common;
using Microsoft.AspNetCore.Identity;

public class AppRole : IdentityRole<long>
{
    public AppRole(string roleName) : base(roleName)
    {
    }

    public AppRole() : base()
    {
    }
}
