namespace Artix.API.Utils;

using System.Text.Json;
using System.Text.Json.Serialization;
using Core.Domain.DomainEvents;

public static class DomainEventExtensions
{
    private static readonly JsonSerializerOptions Options = new()
    {
        ReferenceHandler = ReferenceHandler.IgnoreCycles,
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    public static string ToOutboxJson(this IDomainEvent @event)
        => JsonSerializer.Serialize(@event, @event.GetType(), Options);

    public static T? FromOutboxJson<T>(this string json) where T : class
        => JsonSerializer.Deserialize<T>(json, Options);

    public static object? FromOutboxJson(this string json, Type type)
        => JsonSerializer.Deserialize(json, type, Options);
}
