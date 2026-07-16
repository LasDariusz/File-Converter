namespace Api.Modules.Files.Application.UseCases.GetFileUseCase;

public interface IGetFileUseCaseHandler
{
    Task<GetFileResult> ExecuteAsync(
        GetFileCommand command, 
        CancellationToken cancellationToken);
}