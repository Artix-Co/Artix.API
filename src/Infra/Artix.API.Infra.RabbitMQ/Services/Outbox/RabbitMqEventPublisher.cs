namespace Artix.API.Infra.RabbitMQ.Services.Outbox;

using System.Text.Json;
using Core.Contract.Primitives.Infra.RabbitMQ;
using Core.Domain.DomainEvents;
using Core.Domain.Entities.Object.Events;
using global::RabbitMQ.Client;
using global::RabbitMQ.Client.Events;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

internal sealed class RabbitMqEventPublisher : IEventPublisher, IAsyncDisposable
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IConnection _connection;
    private readonly IChannel _channel;
    private bool _disposed;
    private const string Exchange = "domain-events";

    public RabbitMqEventPublisher(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;

        using var scope = _scopeFactory.CreateScope();
        var factory = scope.ServiceProvider.GetRequiredService<RabbitMqConnectionFactory>();
        _connection = factory.CreateConnectionAsync().GetAwaiter().GetResult();
        _channel = _connection.CreateChannelAsync().GetAwaiter().GetResult();
        _channel.ExchangeDeclareAsync(
            exchange: Exchange,
            type: ExchangeType.Topic,
            durable: true,
            autoDelete: false,
            arguments: null);
    }

    public async Task PublishAsync(IDomainEvent @event, CancellationToken cancellationToken = default)
    {
        if (_disposed)
            throw new ObjectDisposedException(nameof(RabbitMqEventPublisher));

        var eventType = @event.GetType().Name;
        var routingKey = $"domain.{eventType}";
        var body = JsonSerializer.SerializeToUtf8Bytes(@event);


        var properties = new BasicProperties { DeliveryMode = DeliveryModes.Persistent };


        await _channel.BasicPublishAsync(
            Exchange,
            routingKey,
            mandatory: true, 
            properties,
            body,
            cancellationToken
        );
    }

    public async ValueTask DisposeAsync()
    {
        if (_disposed)
            return;

        _disposed = true;

        try
        {
            await this._channel.CloseAsync();
        }
        catch
        {
            // نادیده گرفتن خطاها
        }
        finally
        {
            this._channel.Dispose();
        }

        try
        {
            await this._connection.CloseAsync();
        }
        catch
        {
            // نادیده گرفتن خطاها
        }
        finally
        {
            this._connection.Dispose();
        }

        GC.SuppressFinalize(this);
    }
}
