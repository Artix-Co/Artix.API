namespace Artix.API.Infra.RabbitMQ.Interfaces.Notification;

public interface IMessageSerializer
{
    ReadOnlyMemory<byte> Serialize<T>(T obj);
    T Deserialize<T>(ReadOnlyMemory<byte> payload);
}
