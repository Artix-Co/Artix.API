namespace Artix.API.Infra.Sql.Repositories.Features.OTPs;

using Core.Contract.Features.OTPs.Queries;
using Core.Contract.Features.OTPs.Queries.GetLatestByPhoneNumber;
using Core.Domain.Entities.User;
using Data.DbContexts;
using Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Primitives;

public class OTPQueryRepository : QueryRepository<OTP>, IOTPQueryRepository
{
    private readonly ILogger<OTPQueryRepository> _logger;
    private readonly ArtixQueryDbContext _queryDbContext;


    public OTPQueryRepository(ArtixQueryDbContext queryDbContext, ILogger<OTPQueryRepository> logger) : base(
        queryDbContext)
    {
        this._queryDbContext = queryDbContext;
        this._logger = logger;
    }

    public async Task<LatestOTPByPhoneNumberDto> GetLatestByPhoneNumberAsync(GetLatestOTPByPhoneNumberQuery dto,
        CancellationToken cancellationToken = default)
    {
        var otp = await _queryDbContext.OTPs
            .Where(o => o.PhoneNumber == dto.PhoneNumber)
            .OrderByDescending(o => o.CreatedAt)
            .FirstOrDefaultAsync(cancellationToken);

        if (otp == null)
            throw InfrastructureNotFoundException.ForEntity(nameof(OTP), dto.PhoneNumber);

        if (!otp.IsValid(dto.OtpCode))
            throw new InvalidOperationException("Invalid or expired OTP");

        return new LatestOTPByPhoneNumberDto { Id = otp.Id, Code = otp.Code };
    }
}
