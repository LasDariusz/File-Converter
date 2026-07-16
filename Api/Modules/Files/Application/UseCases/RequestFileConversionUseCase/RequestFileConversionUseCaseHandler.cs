using Api.Modules.Files.Application.Ports;
using Api.Modules.Files.Domain.Models;

namespace Api.Modules.Files.Application.UseCases.RequestFileConversionUseCase;

public class RequestFileConversionUseCaseHandler : IRequestFileConversionUseCaseHandler
{
    private readonly IFilesRepository _filesRepository;
    private readonly IConversionsRepository _conversionsRepository;

    public RequestFileConversionUseCaseHandler(
        IFilesRepository filesRepository,
        IConversionsRepository conversionsRepository)
    {
        _filesRepository = filesRepository;
        _conversionsRepository = conversionsRepository;
    }

    public async Task<RequestFileConversionResult> ExecuteAsync(
        RequestFileConversionCommand command,
        CancellationToken cancellationToken)
    {
        
        return null;
    }
}