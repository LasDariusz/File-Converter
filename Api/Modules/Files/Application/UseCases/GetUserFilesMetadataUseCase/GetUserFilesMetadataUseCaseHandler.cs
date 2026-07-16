using Api.Modules.Files.Application.Ports;

namespace Api.Modules.Files.Application.UseCases.GetFileDetailsUseCase;

public class GetUserFilesMetadataUseCaseHandler : IGetUserFilesMetadataUseCaseHandler
{
    private readonly IFilesRepository _filesRepository;

    public GetUserFilesMetadataUseCaseHandler(
        IFilesRepository filesRepository)
    {
        _filesRepository = filesRepository;
    }

    public async Task<GetUserFilesMetadataResult> ExecuteAsync(
        GetUserFilesMetadataCommand command, 
        CancellationToken cancellationToken)
    {
        var fileList = await _filesRepository.FindUserFilesByUserIdAsync(
            command.UserId, 
            cancellationToken);

        var safeFileList = fileList
            .ToList();

        return new GetUserFilesMetadataResult
        {
            Files = null
        };
    }

}