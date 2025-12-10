namespace Artix.API.Infra.RabbitMQ.Services.Outbox;

using System.Text.Json;
using Core.Contract.Primitives.Infra.RabbitMQ;
using Core.Domain.DomainEvents;
using global::RabbitMQ.Client;

internal sealed class RabbitMqEventPublisher : IEventPublisher, IAsyncDisposable
{
    private readonly IConnection _connection;
    private IChannel? _channel;
    private const string Exchange = "domain-events";

    public RabbitMqEventPublisher(IConnection connection)
    {
        _connection = connection;
    }

    private async Task<IChannel> GetChannelAsync(CancellationToken ct = default)
    {
        if (_channel is { IsOpen: true })
            return _channel;

        _channel = await _connection.CreateChannelAsync(cancellationToken: ct);

        await _channel.ExchangeDeclareAsync(
            exchange: Exchange,
            type: ExchangeType.Topic,
            durable: true,
            cancellationToken: ct);

        return _channel;
    }

    public async Task PublishAsync(IDomainEvent @event, CancellationToken ct = default)
    {
        var channel = await GetChannelAsync(ct);
        var routingKey = $"domain.{@event.GetType().Name}";
        var body = JsonSerializer.SerializeToUtf8Bytes(@event);
        var props = new BasicProperties { DeliveryMode = DeliveryModes.Persistent };

        await channel.BasicPublishAsync(Exchange, routingKey, true, props, body, ct);
    }

    public async ValueTask DisposeAsync()
    {
        if (_channel != null)
        {
            try { await _channel.CloseAsync(); } catch { }
            _channel.Dispose();
        }
    }
}
