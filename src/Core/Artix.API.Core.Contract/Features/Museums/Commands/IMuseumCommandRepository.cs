namespace Artix.API.Core.Contract.Features.Museums.Commands;

using Domain.Entities.Museum;
using Primitives.Repositories;
using ScanObject;

public interface IMuseumCommandRepository : ICommandRepository<Museum>
{
    
}
