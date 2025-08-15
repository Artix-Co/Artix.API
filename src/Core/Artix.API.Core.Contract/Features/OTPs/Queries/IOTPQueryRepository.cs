namespace Artix.API.Core.Contract.Features.OTPs.Queries;

using Domain.Entities.OTP;
using Domain.Entities.User;
using GetLatestByPhoneNumber;
using Primitives.Repositories;

public interface IOTPQueryRepository : IQueryRepository<OTP>
{
    Task<LatestOTPByPhoneNumberDto> GetLatestByPhoneNumberAsync(GetLatestOTPByPhoneNumberQuery query,
        CancellationToken cancellationToken = default);
}
