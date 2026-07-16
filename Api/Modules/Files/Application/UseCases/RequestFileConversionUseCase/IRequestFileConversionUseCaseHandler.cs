namespace Api.Modules.Files.Application.UseCases.RequestFileConversionUseCase;

public interface IRequestFileConversionUseCaseHandler
{
    Task<RequestFileConversionResult> ExecuteAsync(
        RequestFileConversionCommand command,
        CancellationToken cancellationToken);
}