namespace Api.Options;

public class ClientConfigOptions
{
    public required string BaseAddress { get; set; }

    public required int TimeoutSeconds { get; set; }
}