using Api.Modules.Files.Application.Ports;
using Api.Modules.Files.Application.Exceptions;

namespace Api.Modules.Files.Application.UseCases.GetFileUseCase;

public class GetFileUseCaseHandler : IGetFileUseCaseHandler
{
    private readonly IFilesRepository _filesRepository;
    private readonly IFileStorage _fileStorage;

    public GetFileUseCaseHandler(
        IFilesRepository filesRepository,
        IFileStorage fileStorage)
    {
        _filesRepository = filesRepository;
        _fileStorage = fileStorage;
    }

    public async Task<GetFileResult> ExecuteAsync(
        GetFileCommand command,
        CancellationToken cancellationToken)
    {
        var fileModel = await _filesRepository.FindUserFileByIdAsync(
            command.FileId, 
            command.CallerId, 
            cancellationToken);

        if (fileModel == null)
            throw new Exceptions.FileNotFoundException();

        var fileStream = await _fileStorage.GetOpenFileStreamAsync(
            fileModel.StorageKey, 
            cancellationToken);

        if (fileStream == null)
            throw new FileStorageException();

        return new GetFileResult
        {
            FileId = fileModel.FileId,
            ContentType = fileModel.ContentType,
            FileStream = fileStream,
        };
    } 

}