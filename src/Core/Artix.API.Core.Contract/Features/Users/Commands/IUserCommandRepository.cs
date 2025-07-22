namespace Artix.API.Core.Contract.Features.Users.Commands;

using Domain.Entities.User;
using Primitives.Repositories;
using RegisterAdmins;
using RegisterMobiles;

public interface IUserCommandRepository : ICommandRepository<Friendship>
{
 
}
