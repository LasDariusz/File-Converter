namespace Api.Modules.Files.Infrastructure.Messaging.Config;

public class RabbitMqOptions
{
    public required string HostName { get; init; }

    public int Port { get; init; } = 5672;

    public required string UserName { get; init; }

    public required string Password { get; init; }
}