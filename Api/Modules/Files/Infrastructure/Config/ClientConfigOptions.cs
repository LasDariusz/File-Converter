namespace Api.Modules.Files.Infrastructure.Config;

public class ClientConfigOptions
{
    public required string BaseAddress { get; set; }

    public required int TimeoutSeconds { get; set; }
}