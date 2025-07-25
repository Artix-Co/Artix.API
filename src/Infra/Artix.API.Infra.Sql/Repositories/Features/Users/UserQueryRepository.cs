namespace Artix.API.Infra.Sql.Repositories.Features.Users;

using Core.Contract.Features.Users.Queries;
using Core.Domain.Entities.User;
using Data;
using Data.DbContexts;
using Microsoft.Extensions.Logging;
using Primitives;

public sealed class UserQueryRepository : QueryRepository<AppUser>, IUserQueryRepository
{
    private readonly ILogger<UserQueryRepository> _logger;
    private readonly ArtixQueryDbContext _queryDbContext;

    public UserQueryRepository(ArtixQueryDbContext queryDbContext, ILogger<UserQueryRepository> logger)
        : base(queryDbContext)
    {
        this._queryDbContext = queryDbContext;
        this._logger = logger;
    }
}
