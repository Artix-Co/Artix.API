namespace Artix.API.Infra.Identity.Interfaces.LoginHistory;

using Core.Domain.Entities.User;

public interface IUserLoginHistoryService
{
    Task RecordLoginAsync(AppUser user, string ipAddress, string userAgent);
}
