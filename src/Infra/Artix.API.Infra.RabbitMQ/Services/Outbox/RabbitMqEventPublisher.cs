namespace Artix.API.Infra.RabbitMQ.Services.Outbox;

using System.Text.Json;
using Core.Domain.DomainEvents;
using global::RabbitMQ.Client;
using global::RabbitMQ.Client.Events;
using Interfaces.Outbox;
using Microsoft.Extensions.DependencyInjection;

internal sealed class RabbitMqEventPublisher : IEventPublisher, IAsyncDisposable
{
    private readonly IServiceScopeFactory _scopeFactory;
    private IConnection _connection;
    private IChannel _channel;
    private bool _disposed;
    private const string Exchange = "domain-events";

    public RabbitMqEventPublisher(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }


    public async Task PublishAsync(IDomainEvent @event, CancellationToken cancellationToken = default)
    {
        await using var scope = this._scopeFactory.CreateAsyncScope();
        var factory = scope.ServiceProvider.GetRequiredService<RabbitMqConnectionFactory>();
        this._connection = await factory.CreateConnectionAsync();
        this._channel = await this._connection.CreateChannelAsync(null, cancellationToken);

        await _channel.ExchangeDeclareAsync(
            exchange: Exchange,
            type: ExchangeType.Topic,
            durable: true,
            autoDelete: false,
            arguments: null,
            cancellationToken: cancellationToken);

        if (_disposed)
            throw new ObjectDisposedException(nameof(RabbitMqEventPublisher));


        var eventType = @event.GetType().Name;
        var routingKey = $"domain.{eventType}";
        var messageBody = JsonSerializer.SerializeToUtf8Bytes(@event);

        var consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.ReceivedAsync += async (sender, ea) =>
        {
            var ch = ((AsyncEventingBasicConsumer)sender).Channel;
            var properties = ea.BasicProperties;
            var replyProps = new BasicProperties { CorrelationId = properties.CorrelationId };

            await _channel.BasicPublishAsync(
                exchange: Exchange,
                routingKey: routingKey,
                mandatory: true,
                basicProperties: replyProps,
                body: messageBody,
                cancellationToken: cancellationToken);

            await ch.BasicAckAsync(ea.DeliveryTag, false, cancellationToken);
        };
    }

    public async ValueTask DisposeAsync()
    {
        if (_disposed)
            return;

        _disposed = true;

        if (_channel != null)
        {
            try
            {
                await _channel.CloseAsync();
            }
            catch
            {
                // نادیده گرفتن خطاها در زمان بستن کانال
            }
            finally
            {
                _channel.Dispose();
                _channel = null;
            }
        }

        if (_connection != null)
        {
            try
            {
                await _connection.CloseAsync();
            }
            catch
            {
                // نادیده گرفتن خطاها در زمان بستن اتصال
            }
            finally
            {
                _connection.Dispose();
                _connection = null;
            }
        }

        GC.SuppressFinalize(this);
    }
}
