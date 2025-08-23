namespace Artix.API.Infra.Sql.Repositories.Features.OTPs;

using Core.Contract.Features.OTPs.Commands;
using Core.Domain.Entities.OTP;
using Data.DbContexts;
using Microsoft.Extensions.Logging;
using Primitives;

public sealed class OTPCommandRepository : CommandRepository<OTP>, IOTPCommandRepository
{
    private readonly ILogger<OTPCommandRepository> _logger;

    public OTPCommandRepository(ArtixCommandDbContext commandDbContext, ILogger<OTPCommandRepository> logger)
        : base(commandDbContext)
    {
        _logger = logger;
    }
}
