namespace Artix.API.Infra.RabbitMQ.Services.Notification;

using Artix.API.Infra.RabbitMQ.Interfaces.Notification;
using Artix.API.Infra.RabbitMQ.Models.Notification;
using global::RabbitMQ.Client;
using global::RabbitMQ.Client.Events;
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
        this._factory = factory;
        this._serializer = serializer;
        this._handler = handler;

        this._queueName = queueName;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        this._connection = this._factory.CreateConnection();
        this._channel = this._connection.CreateModel();

        this._channel.ExchangeDeclare("notifications.exchange", ExchangeType.Topic, durable: true);

        this._channel.QueueDeclare(queue: this._queueName,
                              durable: true,
                              exclusive: false,
                              autoDelete: false,
                              arguments: null);

        this._channel.QueueBind(queue: this._queueName,
                           exchange: "notifications.exchange",
                           routingKey: "notifications.#");

        this._channel.BasicQos(0, 10, false);
        var consumer = new AsyncEventingBasicConsumer(this._channel);
        consumer.Received += async (sender, ea) =>
        {
            try
            {
                var body = ea.Body.ToArray();
                var message = this._serializer.Deserialize<NotificationMessage>(body);
                await this._handler.HandleAsync(message);
                this._channel.BasicAck(ea.DeliveryTag, false);
            }
            catch (Exception)
            {
                this._channel.BasicNack(ea.DeliveryTag, false, false);
            }
        };
        this._channel.BasicConsume(this._queueName, false, consumer);
        return Task.CompletedTask;
    }

    public override void Dispose()
    {
        this._channel?.Close();
        this._channel?.Dispose();
        this._connection?.Close();
        this._connection?.Dispose();
        base.Dispose();
    }
}
 
