namespace Artix.API.Core.Contract.Primitives.Infra.Audit;
public interface IAdminAuditWriter
{
    Task WriteAsync(long? userId, string method, string path, int statusCode, string? roles, string? ipAddress, CancellationToken cancellationToken = default);
}
