namespace Artix.API.Infra.RabbitMQ.Services;

using System.Text;
using System.Text.Json;
using Interfaces;

public class MessageSerializer : IMessageSerializer
{
    private readonly JsonSerializerOptions _options = new(JsonSerializerDefaults.Web);

    public ReadOnlyMemory<byte> Serialize<T>(T obj)
    {
        var json = JsonSerializer.Serialize(obj, _options);
        return Encoding.UTF8.GetBytes(json);
    }

    public T Deserialize<T>(ReadOnlyMemory<byte> payload)
    {
        return JsonSerializer.Deserialize<T>(Encoding.UTF8.GetString(payload.Span), _options)!;
    }
}
