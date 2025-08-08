namespace Artix.API.Core.DomainService.Users.LoginHistory;

using Domain.Entities.User;
using Infra.Sql.Data.DbContexts;

public sealed class UserLoginHistoryService : IUserLoginHistoryService
{
    private readonly ArtixCommandDbContext _dbContext;
    public UserLoginHistoryService(ArtixCommandDbContext dbContext) => _dbContext = dbContext;

    public async Task RecordLoginAsync(AppUser user, string ipAddress, string userAgent)
    {
        try
        {
            _dbContext.UserLoginHistories.Add(new UserLoginHistory
            {
                UserId = user.Id, IpAddress = ipAddress, UserAgent = userAgent
            });
            await _dbContext.SaveChangesAsync();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}
