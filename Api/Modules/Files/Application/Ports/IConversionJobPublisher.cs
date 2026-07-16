using Api.Modules.Files.Infrastructure.Messaging.Contracts;

namespace Api.Modules.Files.Application.Ports;

public interface IConversionJobPublisher
{
    Task PublishAsync(
        ConversionRequestedContract message,
        CancellationToken cancellationToken);

    Task PublishRetryAsync(
        ConversionRequestedContract message,
        int retryCount,
        CancellationToken cancellationToken);
}