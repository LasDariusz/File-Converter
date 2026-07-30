using Api.Modules.Files.Application.Ports;
using Api.Modules.Files.Application.UseCases.GetConversionStatusUseCase;
using Api.Modules.Files.Application.UseCases.GetFileDetailsUseCase;
using Api.Modules.Files.Application.UseCases.GetFileUseCase;
using Api.Modules.Files.Application.UseCases.RequestFileConversionUseCase;
using Api.Modules.Files.Infrastructure.Messaging;
using Api.Modules.Files.Infrastructure.Messaging.Config;
using Api.Modules.Files.Infrastructure.Persistence;

namespace Api.Modules.Files;

public static class FilesApplicationExtensions
{
    public static void ConfigureFilesFeatures(this IHostApplicationBuilder builder)
    {
        builder.Services.Configure<RabbitMqOptions>(
            builder.Configuration.GetSection("RabbitMq"));

        builder.Services.AddScoped<IFilesRepository, FilesRepository>();
        builder.Services.AddScoped<IConversionsRepository, ConversionsRepository>();

        builder.Services.AddScoped<
            IRequestFileConversionUseCaseHandler,
            RequestFileConversionUseCaseHandler>();

        builder.Services.AddScoped<
            IGetConversionStatusUseCaseHandler,
            GetConversionStatusUseCaseHandler>();

        builder.Services.AddScoped<IGetFileUseCaseHandler, GetFileUseCaseHandler>();
        builder.Services.AddScoped<
            IGetUserFilesMetadataUseCaseHandler,
            GetUserFilesMetadataUseCaseHandler>();

        builder.Services.AddSingleton<RabbitMqConversionJobPublisher>();
        builder.Services.AddSingleton<IConversionJobPublisher>(services =>
            services.GetRequiredService<RabbitMqConversionJobPublisher>());
        builder.Services.AddHostedService(services =>
            services.GetRequiredService<RabbitMqConversionJobPublisher>());
    }
}
