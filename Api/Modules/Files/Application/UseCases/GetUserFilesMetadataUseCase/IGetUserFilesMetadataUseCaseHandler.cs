namespace Api.Modules.Files.Application.UseCases.GetFileDetailsUseCase;

public interface IGetUserFilesMetadataUseCaseHandler
{
    Task<GetUserFilesMetadataResult> ExecuteAsync(
        GetUserFilesMetadataCommand command, 
        CancellationToken cancellationToken);
}