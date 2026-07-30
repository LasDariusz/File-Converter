using Api.Modules.Files.Application.Ports;

namespace Api.Modules.Files.Application.UseCases.GetConversionStatusUseCase;

public sealed class GetConversionStatusUseCaseHandler : IGetConversionStatusUseCaseHandler
{
    private readonly IConversionsRepository _conversionsRepository;

    public GetConversionStatusUseCaseHandler(
        IConversionsRepository conversionsRepository)
    {
        _conversionsRepository = conversionsRepository;
    }

    public async Task<GetConversionStatusResult?> ExecuteAsync(
        Guid conversionId,
        Guid callerId,
        CancellationToken cancellationToken)
    {
        var conversion = await _conversionsRepository.FindForOwnerAsync(
            conversionId,
            callerId,
            cancellationToken);

        return conversion is null
            ? null
            : new GetConversionStatusResult
            {
                ConversionId = conversion.ConversionId,
                Status = conversion.Status,
                OutputFileId = conversion.OutputFileId,
                ErrorMessage = conversion.ErrorMessage,
                CreatedAt = conversion.CreatedAt,
                StartedAt = conversion.StartedAt,
                FinishedAt = conversion.FinishedAt
            };
    }
}
