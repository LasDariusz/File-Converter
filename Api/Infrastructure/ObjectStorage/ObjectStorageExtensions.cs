using Api.Options;
using Minio;

namespace Api.Infrastructure.ObjectStorage;

public static class ObjectStorageExtensions
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

        builder.Services.AddSingleton<IObjectStorage, ObjectStorage>();
    }
}