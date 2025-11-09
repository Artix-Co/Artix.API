namespace Artix.API.Core.Contract.Primitives.Infra.Identity;

using Domain.Entities.User;

public interface IUserLoginHistoryService
{
    Task RecordLoginAsync(AppUser user, string ipAddress, string userAgent);
}
