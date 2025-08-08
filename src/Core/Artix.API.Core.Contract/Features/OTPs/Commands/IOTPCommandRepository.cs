namespace Artix.API.Core.Contract.Features.OTPs.Commands;

using Domain.Entities.User;
using Primitives.Repositories;

public interface IOTPCommandRepository : ICommandRepository<OTP>
{
}
