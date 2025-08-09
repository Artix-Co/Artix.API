namespace Artix.API.Infra.RabbitMQ.Services;

using global::RabbitMQ.Client;
using global::RabbitMQ.Client.Events;
using Interfaces;
using Microsoft.Extensions.Hosting;
public class NotificationConsumer : BackgroundService
{
    private readonly RabbitMqConnectionFactory _factory;
    private readonly IMessageSerializer _serializer;
    private readonly INotificationHandler _handler;
    private readonly string _queueName;
    private IConnection? _connection;
    private IModel? _channel;

    public NotificationConsumer(RabbitMqConnectionFactory factory, IMessageSerializer serializer,
        INotificationHandler handler, string queueName)
    {
        _factory = factory;
        _serializer = serializer;
        _handler = handler;

        _queueName = queueName;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _connection = _factory.CreateConnection();
        _channel = _connection.CreateModel();

        _channel.ExchangeDeclare("notifications.exchange", ExchangeType.Topic, durable: true);

        _channel.QueueDeclare(queue: _queueName,
                              durable: true,
                              exclusive: false,
                              autoDelete: false,
                              arguments: null);

        _channel.QueueBind(queue: _queueName,
                           exchange: "notifications.exchange",
                           routingKey: "notifications.#");

        _channel.BasicQos(0, 10, false);
        var consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.Received += async (sender, ea) =>
        {
            try
            {
                var body = ea.Body.ToArray();
                var message = _serializer.Deserialize<Models.NotificationMessage>(body);
                await _handler.HandleAsync(message);
                _channel.BasicAck(ea.DeliveryTag, false);
            }
            catch (Exception)
            {
                _channel.BasicNack(ea.DeliveryTag, false, false);
            }
        };
        _channel.BasicConsume(_queueName, false, consumer);
        return Task.CompletedTask;
    }

    public override void Dispose()
    {
        _channel?.Close();
        _channel?.Dispose();
        _connection?.Close();
        _connection?.Dispose();
        base.Dispose();
    }
}
 
