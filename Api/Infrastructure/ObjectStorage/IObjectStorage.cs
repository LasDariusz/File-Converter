namespace Api.Infrastructure.ObjectStorage;

public interface IObjectStorage
{
    Task<Stream> GetOpenFileStreamAsync(string storageKey); 
    Task<bool> UploadFileAsync(string storageKey, Stream stream, long streamLength, string contentType);
    Task<bool> DeleteFileAsync(string storageKey);
}
 