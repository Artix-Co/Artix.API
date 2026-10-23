namespace Artix.API.Infra.Sql.Services;
using Core.Contract.Primitives.Infra.Audit;
using Core.Domain.Entities.Audit;
using Data.DbContexts;
internal sealed class AdminAuditWriter(ArtixCommandDbContext dbContext) : IAdminAuditWriter
{
    public async Task WriteAsync(long? userId, string method, string path, int statusCode, string? roles, string? ipAddress, CancellationToken cancellationToken = default)
    { dbContext.Add(AdminAuditEntry.Create(userId, method, path, statusCode, roles, ipAddress)); await dbContext.SaveChangesAsync(cancellationToken); }
}
