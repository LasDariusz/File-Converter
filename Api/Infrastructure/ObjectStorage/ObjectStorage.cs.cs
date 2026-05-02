using Microsoft.Extensions.Options;
using Api.Options;
using Minio;
using Minio.DataModel.Args;

namespace Api.Infrastructure.ObjectStorage;

public class ObjectStorage : IObjectStorage
{
    private readonly IMinioClient _minioClient;
    private readonly ObjectStorageBucketConfigOptions _bucketConfigOptions;

    public ObjectStorage(IMinioClient minioClient, 
        IOptions<ObjectStorageBucketConfigOptions> bucketConfigOptions)
    {
        _minioClient = minioClient;
        _bucketConfigOptions = bucketConfigOptions.Value;
    }

    public async Task<Stream> GetOpenFileStreamAsync(string storageKey)
    {
        var memoryStream = new MemoryStream();

        var args = new GetObjectArgs()
            .WithBucket(_bucketConfigOptions.MainBucketName)
            .WithObject(storageKey)
            .WithCallbackStream((minioStream) =>
            {
                minioStream.CopyTo(memoryStream);
            });

        await _minioClient.GetObjectAsync(args);
        memoryStream.Position = 0;

        return memoryStream;
    }

    public async Task<bool> UploadFileAsync(string storageKey, Stream stream,
        long streamLength, string contentType)
    {
        try
        {
            var args = new PutObjectArgs()
                    .WithBucket(_bucketConfigOptions.MainBucketName)
                    .WithObject(storageKey)
                    .WithStreamData(stream)
                    .WithObjectSize(streamLength)
                    .WithContentType(contentType);

            var putObjectRes = await _minioClient.PutObjectAsync(args);

            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return false;
        }  
    }

    public async Task<bool> DeleteFileAsync(string storageKey)
    {
        throw new NotImplementedException();
    }
    
}