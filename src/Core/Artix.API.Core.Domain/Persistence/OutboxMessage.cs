namespace Artix.API.Core.Domain.Persistence;

public class OutboxMessage
{
    public Guid Id { get; set; } = Guid.CreateVersion7();
    public string Type { get; set; }
    public string Data { get; set; }
    public string Status { get; set; } = "Pending";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ProcessedAt { get; set; }
}
