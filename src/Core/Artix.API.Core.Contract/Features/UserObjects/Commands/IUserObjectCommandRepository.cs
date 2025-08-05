namespace Artix.API.Core.Contract.Features.UserObjects.Commands;

using Domain.Entities.User;
using Primitives.Repositories;

public interface IUserObjectCommandRepository: ICommandRepository<UserObject>
{
    
}
