namespace Artix.API.Infra.RabbitMQ.Interfaces;

public interface IMessageSerializer
{
    ReadOnlyMemory<byte> Serialize<T>(T obj);
    T Deserialize<T>(ReadOnlyMemory<byte> payload);
}
