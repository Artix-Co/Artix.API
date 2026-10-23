namespace Artix.API.Core.Domain.Entities.Audit;

using Common;

public class AdminAuditEntry : BaseEntity
{
    public long? UserId { get; private set; }
    public string Method { get; private set; } = null!;
    public string Path { get; private set; } = null!;
    public int StatusCode { get; private set; }
    public string? Roles { get; private set; }
    public string? IpAddress { get; private set; }
    private AdminAuditEntry() { }
    public static AdminAuditEntry Create(long? userId, string method, string path, int statusCode, string? roles, string? ipAddress) => new() { UserId = userId, Method = method, Path = path, StatusCode = statusCode, Roles = roles, IpAddress = ipAddress };
}
