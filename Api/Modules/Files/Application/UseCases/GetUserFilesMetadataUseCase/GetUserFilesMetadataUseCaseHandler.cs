using Api.Modules.Files.Application.Ports;
using Api.Modules.Files.Application.UseCases.GetUserFilesMetadataUseCase;

namespace Api.Modules.Files.Application.UseCases.GetFileDetailsUseCase;

public sealed class GetUserFilesMetadataUseCaseHandler : IGetUserFilesMetadataUseCaseHandler
{
    private readonly IFilesRepository _filesRepository;

    public GetUserFilesMetadataUseCaseHandler(IFilesRepository filesRepository)
    {
        _filesRepository = filesRepository;
    }

    public async Task<GetUserFilesMetadataResult> ExecuteAsync(
        GetUserFilesMetadataCommand command,
        CancellationToken cancellationToken)
    {
        var files = await _filesRepository.FindUserFilesByUserIdAsync(
            command.UserId,
            cancellationToken);

        var summaries = files
            .Select(file => new FileSummary
            {
                FileId = file.FileId,
                OwnerId = file.OwnerId,
                Metadata = file.Metadata,
                CreatedAt = file.CreatedAt
            })
            .ToList();

        return new GetUserFilesMetadataResult
        {
            Files = summaries
        };
    }
}
