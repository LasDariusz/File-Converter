using System.Text.Json;
using Api.Modules.Files.Application.Ports;
using Api.Modules.Files.Infrastructure.Messaging.Config;
using Api.Modules.Files.Infrastructure.Messaging.Contracts;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace Api.Modules.Files.Infrastructure.Messaging;

public sealed class RabbitMqConversionJobPublisher :
    IConversionJobPublisher,
    IHostedService,
    IAsyncDisposable
{
    public const string QueueName = "conversion_jobs";

    private readonly RabbitMqOptions _options;
    private readonly ILogger<RabbitMqConversionJobPublisher> _logger;
    private readonly SemaphoreSlim _publishLock = new(1, 1);

    private IConnection? _connection;
    private IChannel? _channel;

    public RabbitMqConversionJobPublisher(
        IOptions<RabbitMqOptions> options,
        ILogger<RabbitMqConversionJobPublisher> logger)
    {
        _options = options.Value;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var factory = new ConnectionFactory
        {
            HostName = _options.HostName,
            Port = _options.Port,
            UserName = _options.UserName,
            Password = _options.Password,
            AutomaticRecoveryEnabled = true,
            TopologyRecoveryEnabled = true,
            ClientProvidedName = "file-converter-api-publisher"
        };

        _connection = await factory.CreateConnectionAsync(
            cancellationToken: cancellationToken);

        var channelOptions = new CreateChannelOptions(
            publisherConfirmationsEnabled: true,
            publisherConfirmationTrackingEnabled: true);

        _channel = await _connection.CreateChannelAsync(
            options: channelOptions,
            cancellationToken: cancellationToken);

        await _channel.QueueDeclareAsync(
            queue: QueueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            cancellationToken: cancellationToken);

        _logger.LogInformation(
            "RabbitMQ publisher connected and queue {QueueName} is available",
            QueueName);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    public async Task PublishAsync(
        ConversionRequestedContract message,
        CancellationToken cancellationToken)
    {
        if (_channel is null || _channel.IsClosed)
            throw new InvalidOperationException("RabbitMQ publisher is not available.");

        var body = JsonSerializer.SerializeToUtf8Bytes(
            message,
            new JsonSerializerOptions(JsonSerializerDefaults.Web));

        var properties = new BasicProperties
        {
            ContentType = "application/json",
            DeliveryMode = DeliveryModes.Persistent,
            MessageId = message.MessageId.ToString(),
            CorrelationId = message.ConversionId.ToString(),
            Type = "conversion.requested.v1"
        };

        await _publishLock.WaitAsync(cancellationToken);

        try
        {
            await _channel.BasicPublishAsync(
                exchange: string.Empty,
                routingKey: QueueName,
                mandatory: true,
                basicProperties: properties,
                body: body,
                cancellationToken: cancellationToken);
        }
        finally
        {
            _publishLock.Release();
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (_channel is not null)
            await _channel.DisposeAsync();

        if (_connection is not null)
            await _connection.DisposeAsync();

        _publishLock.Dispose();
    }
}
