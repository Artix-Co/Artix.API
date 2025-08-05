namespace Artix.API.Infra.Sql.Repositories.Features.UserObjects;

using Core.Contract.Features.UserObjects.Commands;
using Core.Domain.Entities.User;
using Data.DbContexts;
using Microsoft.Extensions.Logging;
using Primitives;

public sealed class UserObjectCommandRepository : CommandRepository<UserObject>, IUserObjectCommandRepository
{
    public UserObjectCommandRepository(ArtixCommandDbContext commandDbContext, ILogger<UserObjectCommandRepository> logger)
        : base(commandDbContext)
    {
    }
    
}
