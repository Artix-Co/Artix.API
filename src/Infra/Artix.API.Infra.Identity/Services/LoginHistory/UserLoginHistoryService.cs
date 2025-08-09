namespace Artix.API.Infra.Identity.Services.LoginHistory;

using Core.Domain.Entities.User;
using Artix.API.Infra.Identity.Interfaces.LoginHistory;
using Sql.Data.DbContexts;

public sealed class UserLoginHistoryService : IUserLoginHistoryService
{
    private readonly ArtixCommandDbContext _dbContext;
    public UserLoginHistoryService(ArtixCommandDbContext dbContext) => this._dbContext = dbContext;

    public async Task RecordLoginAsync(AppUser user, string ipAddress, string userAgent)
    {
        try
        {
            this._dbContext.UserLoginHistories.Add(new UserLoginHistory
            {
                UserId = user.Id, IpAddress = ipAddress, UserAgent = userAgent
            });
            await this._dbContext.SaveChangesAsync();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}
