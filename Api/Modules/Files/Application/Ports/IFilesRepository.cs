using Api.Modules.Files.Domain.Models.Files;

namespace Api.Modules.Files.Application.Ports;

public interface IFilesRepository
{
    Task<StoredFileModel?> FindUserFileByIdAsync(
        Guid fileId, 
        Guid callerId, 
        CancellationToken cancellationToken);

    Task<IEnumerable<StoredFileModel>> FindUserFilesByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken);
}