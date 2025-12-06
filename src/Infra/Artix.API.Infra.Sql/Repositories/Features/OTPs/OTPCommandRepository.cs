namespace Artix.API.Infra.Sql.Repositories.Features.OTPs;

using Core.Contract.Features.OTPs.Commands;
using Core.Domain.Entities.OTP;
using Data.DbContexts;
using Microsoft.Extensions.Logging;
using Primitives;

public sealed class OTPCommandRepository : CommandRepository<OTP>, IOTPCommandRepository
{
    public OTPCommandRepository(ArtixCommandDbContext commandDbContext, ILogger<CommandRepository<OTP>> logger)
        : base(commandDbContext, logger)
    {
    }
}
