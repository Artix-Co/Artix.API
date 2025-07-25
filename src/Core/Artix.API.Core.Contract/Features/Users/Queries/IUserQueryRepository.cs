namespace Artix.API.Core.Contract.Features.Users.Queries;

using Artix.API.Core.Contract.Primitives.Repositories;
using Artix.API.Core.Domain.Entities.User;

public interface IUserQueryRepository: IQueryRepository<AppUser>
{
    
}
