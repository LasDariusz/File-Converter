using Api.Modules.Files.Application.Common;

namespace Api.Modules.Files.Application.Ports;

public interface IFileStorage
{
    Task<Stream> GetOpenFileStreamAsync(
        string storageKey, 
        CancellationToken cancellationToken);

    Task UploadFileAsync(
        string storageKey,
        IncomingFile incomingFile,
        CancellationToken cancellationToken);

    Task DeleteFileAsync(
        string storageKey, 
        CancellationToken cancellationToken);
}