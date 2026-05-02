
using Api.Options;

namespace Api.Infrastructure.ConverterClient;

public static class ConverterClientExtensions
{
    public static void ConfigureConverterHttpClient(this IHostApplicationBuilder builder)
    {
        var clientOptions = builder.Configuration
            .GetSection("ClientConfig")
            .Get<ClientConfigOptions>()!;

        builder.Services.Configure<ClientConfigOptions>
            (builder.Configuration.GetSection("ClientConfig")
        );

        builder.Services.AddHttpClient<IConverterClient, ConverterClient>(client =>
        {
            client.BaseAddress = new Uri(clientOptions.BaseAddress);
            client.Timeout = TimeSpan.FromSeconds(clientOptions.TimeoutSeconds);
        });
    }
}