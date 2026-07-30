using Api.Modules.Files.Application.Common;
using Api.Modules.Files.Application.Ports;
using Api.Modules.Files.Infrastructure.Config;
using Microsoft.Extensions.Options;
using Minio;
using Minio.DataModel.Args;

namespace Api.Modules.Files.Infrastructure.Adapters;

public sealed class FileStorage : IFileStorage
{
    private readonly IMinioClient _minioClient;
    private readonly ObjectStorageBucketConfigOptions _bucketOptions;
    private readonly SemaphoreSlim _bucketLock = new(1, 1);

    public FileStorage(
        IMinioClient minioClient,
        IOptions<ObjectStorageBucketConfigOptions> bucketOptions)
    {
        _minioClient = minioClient;
        _bucketOptions = bucketOptions.Value;
    }

    public async Task<Stream> GetOpenFileStreamAsync(
        string storageKey,
        CancellationToken cancellationToken)
    {
        var memoryStream = new MemoryStream();

        var args = new GetObjectArgs()
            .WithBucket(_bucketOptions.MainBucketName)
            .WithObject(storageKey)
            .WithCallbackStream(minioStream =>
                minioStream.CopyTo(memoryStream));

        await _minioClient.GetObjectAsync(args, cancellationToken);
        memoryStream.Position = 0;

        return memoryStream;
    }

    public async Task UploadFileAsync(
        string storageKey,
        IncomingFile incomingFile,
        CancellationToken cancellationToken)
    {
        await EnsureBucketExistsAsync(cancellationToken);

        await using var stream = incomingFile.OpenReadStream();

        var args = new PutObjectArgs()
            .WithBucket(_bucketOptions.MainBucketName)
            .WithObject(storageKey)
            .WithStreamData(stream)
            .WithObjectSize(incomingFile.SizeBytes)
            .WithContentType(incomingFile.FileFormat.ContentType);

        await _minioClient.PutObjectAsync(args, cancellationToken);
    }

    public async Task DeleteFileAsync(
        string storageKey,
        CancellationToken cancellationToken)
    {
        var args = new RemoveObjectArgs()
            .WithBucket(_bucketOptions.MainBucketName)
            .WithObject(storageKey);

        await _minioClient.RemoveObjectAsync(args, cancellationToken);
    }

    private async Task EnsureBucketExistsAsync(
        CancellationToken cancellationToken)
    {
        await _bucketLock.WaitAsync(cancellationToken);

        try
        {
            var existsArgs = new BucketExistsArgs()
                .WithBucket(_bucketOptions.MainBucketName);

            if (await _minioClient.BucketExistsAsync(existsArgs, cancellationToken))
                return;

            var makeArgs = new MakeBucketArgs()
                .WithBucket(_bucketOptions.MainBucketName);

            await _minioClient.MakeBucketAsync(makeArgs, cancellationToken);
        }
        finally
        {
            _bucketLock.Release();
        }
    }
}
