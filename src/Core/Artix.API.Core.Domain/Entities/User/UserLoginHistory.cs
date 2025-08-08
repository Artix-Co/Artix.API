namespace Artix.API.Core.Domain.Entities.User;

using Common;

public class UserLoginHistory : BaseEntity
{
    public long UserId { get; set; }
    public AppUser User { get; set; }
    public string IpAddress { get; set; }
    public string UserAgent { get; set; }
}
