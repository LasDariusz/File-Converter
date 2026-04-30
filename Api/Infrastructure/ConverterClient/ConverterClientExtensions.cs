
namespace Api.Infrastructure.ConverterClient;

public static class ConverterClientExtensions
{
    public static void ConfigureConverterHttpClient(this IHostApplicationBuilder builder)
    {
        builder.Services.AddHttpClient<IConverterClient, ConverterClient>(client =>
        {
            client.BaseAddress = new Uri("http://hidden-api:8000");
            client.Timeout = TimeSpan.FromMinutes(5);
        });

        builder.Services.AddScoped<IConverterClient, ConverterClient>();
    }
}