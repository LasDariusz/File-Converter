using Api.Modules.Files.Application.Ports;
using Api.Modules.Files.Infrastructure.Config;
using Minio;

namespace Api.Modules.Files.Infrastructure.Adapters;

public static class FileStorageExtensions
{
    public static void ConfigureObjectStorage(this IHostApplicationBuilder builder)
    {
        var objectStorageOptions = builder.Configuration
            .GetSection("ObjectStorageConfig")
            .Get<ObjectStorageConfigOptions>()!;

        builder.Services.Configure<ObjectStorageConfigOptions>(
            builder.Configuration.GetSection("ObjectStorageConfig")
        );

        builder.Services.Configure<ObjectStorageBucketConfigOptions>(
            builder.Configuration.GetSection("ObjectStorageBucketConfig")
        );

        builder.Services.AddSingleton<IMinioClient>(s =>
        {
            return new MinioClient()
                .WithEndpoint(objectStorageOptions.Endpoint.Replace("http://", ""))
                .WithCredentials(objectStorageOptions.AccessKey, objectStorageOptions.SecretKey)
                .WithSSL(objectStorageOptions.WithSSL)
                .Build();
        });

        builder.Services.AddSingleton<IFileStorage, FileStorage>();
    }
}