using Minio;

namespace Api.Infrastructure.ObjectStorage;

public static class ObjectStorageExtensions
{
    public static void ConfigureObjectStorage(this IHostApplicationBuilder builder)
    {
        var minioSettings = builder.Configuration.GetSection("MinioSetup");

        builder.Services.AddScoped<IMinioClient>(s =>
        {
            return new MinioClient()
                .WithEndpoint(minioSettings["Endpoint"]!.Replace("http://", ""))
                .WithCredentials(minioSettings["AccessKey"], minioSettings["SecretKey"])
                .WithSSL(false)
                .Build();
        });

        builder.Services.AddSingleton<IObjectStorage, ObjectStorage>();
    }
}