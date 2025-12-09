namespace Artix.API.Core.Contract.Features.Museums;

using Primitives.Repositories;
using Domain.Entities.Museum;

public interface IMuseumCommandRepository : ICommandRepository<Museum>
{
    
}
