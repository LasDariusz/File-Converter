namespace Api.Modules.Files.Application.UseCases.GetConversionStatusUseCase;

public interface IGetConversionStatusUseCaseHandler
{
    Task<GetConversionStatusResult?> ExecuteAsync(
        Guid conversionId,
        Guid callerId,
        CancellationToken cancellationToken);
}
