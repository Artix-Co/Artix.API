namespace Artix.API.Infra.RabbitMQ.Services.Notification;

using System.Text.Json;
using Core.Contract.Primitives.Infra.RabbitMQ;
using global::RabbitMQ.Client;

internal sealed class NotificationProducer : INotificationProducer, IAsyncDisposable
{
    private readonly IConnection _connection;
    private IChannel? _channel;

    public NotificationProducer(IConnection connection)
    {
        _connection = connection;
    }

    private async Task<IChannel> GetChannelAsync(CancellationToken ct = default)
    {
        if (_channel is { IsOpen: true })
            return _channel;

        _channel = await _connection.CreateChannelAsync(cancellationToken: ct);

        await _channel.ExchangeDeclareAsync(
            exchange: "notifications",
            type: ExchangeType.Topic,
            durable: true,
            cancellationToken: ct);

        return _channel;
    }

    public async Task PublishAsync<T>(string exchange, string routingKey, T message, CancellationToken ct = default)
    {
        var channel = await GetChannelAsync(ct);
        var body = JsonSerializer.SerializeToUtf8Bytes(message);
        var props = new BasicProperties { DeliveryMode = DeliveryModes.Persistent };

        await channel.BasicPublishAsync(exchange, routingKey, true, props, body, ct);
    }

    public async ValueTask DisposeAsync()
    {
        if (_channel != null)
        {
            try
            {
                await _channel.CloseAsync();
            }
            catch
            {
            }

            _channel.Dispose();
        }
    }
}
