namespace Artix.API.Core.Domain.Entities.User;

using Microsoft.AspNetCore.Identity;

public class AppUserToken : IdentityUserToken<long>
{
    // public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    // public DateTime? ExpiresAt { get; set; }
}
