namespace Artix.API.Core.DomainService.Users.LoginHistory;

using Domain.Entities.User;

public interface IUserLoginHistoryService
{
    Task RecordLoginAsync(AppUser user, string ipAddress, string userAgent);
}
