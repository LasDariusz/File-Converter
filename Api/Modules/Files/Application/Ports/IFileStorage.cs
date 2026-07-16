namespace Api.Modules.Files.Application.Ports;

public interface IFileStorage
{
    Task<Stream> GetOpenFileStreamAsync(
        string storageKey, 
        CancellationToken cancellationToken);

    Task<bool> UploadFileAsync(
        string storageKey, 
        Stream stream, 
        long streamLength, 
        string contentType,
        CancellationToken cancellationToken);

    Task<bool> DeleteFileAsync(
        string storageKey, 
        CancellationToken cancellationToken);
}