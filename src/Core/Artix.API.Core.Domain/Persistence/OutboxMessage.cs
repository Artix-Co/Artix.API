namespace Artix.API.Core.Domain.Persistence;

using Enums;

public class OutboxMessage
{
    public Guid Id { get; set; } = Guid.CreateVersion7();
    public string Type { get; set; }
    public string Data { get; set; }
    public OutboxMessageStatus Status { get; set; } = OutboxMessageStatus.Pending;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ProcessedAt { get; set; }
    public int RetryCount { get; set; }
    public string Error { get; set; } = string.Empty;
}
