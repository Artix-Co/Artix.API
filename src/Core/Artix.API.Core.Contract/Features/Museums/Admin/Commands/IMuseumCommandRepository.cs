namespace Artix.API.Core.Contract.Features.Museums.Admin.Commands;

using Domain.Entities.Museum;
using Primitives.Repositories;

public interface IMuseumCommandRepository : ICommandRepository<Museum>
{
    
}
