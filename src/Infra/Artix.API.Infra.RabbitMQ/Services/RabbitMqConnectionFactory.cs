namespace Artix.API.Infra.RabbitMQ.Services;

using Artix.API.Core.Contract.Configs.RabbitMQ;
using global::RabbitMQ.Client;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;


internal sealed class RabbitMqConnectionFactory
{
    private readonly ConnectionFactory _factory;
    private readonly ILogger<RabbitMqConnectionFactory> _logger;

    public RabbitMqConnectionFactory(IOptions<RabbitMqOptions> options, ILogger<RabbitMqConnectionFactory> logger)
    {
        _logger = logger;

        var rabbitOptions = options.Value;

        _logger.LogInformation("Initializing RabbitMQ ConnectionFactory for {Host}:{Port}, vhost: {VHost}", 
            rabbitOptions.Host, rabbitOptions.Port, "/");

        _factory = new ConnectionFactory
        {
            HostName = rabbitOptions.Host,
            Port = rabbitOptions.Port,
            UserName = rabbitOptions.Username,
            Password = rabbitOptions.Password,
            VirtualHost = "/",
            ConsumerDispatchConcurrency = 1,
            AutomaticRecoveryEnabled = true,
            NetworkRecoveryInterval = TimeSpan.FromSeconds(10)
        };
    }

    public async Task<IConnection> CreateConnectionAsync(CancellationToken ct = default)
    {
        try
        {
            var connection = await _factory.CreateConnectionAsync(cancellationToken: ct);
            _logger.LogInformation("Successfully created RabbitMQ connection to {Host}:{Port}", 
                _factory.HostName, _factory.Port);
            return connection;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create RabbitMQ connection to {Host}:{Port}", 
                _factory.HostName, _factory.Port);
            throw;
        }
    }
}
