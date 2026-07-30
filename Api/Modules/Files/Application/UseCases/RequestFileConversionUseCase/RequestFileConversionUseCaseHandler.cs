using Api.Modules.Files.Application.Ports;
using Api.Modules.Files.Domain.Models.Conversions;
using Api.Modules.Files.Domain.Models.Files;
using Api.Modules.Files.Infrastructure.Messaging.Contracts;

namespace Api.Modules.Files.Application.UseCases.RequestFileConversionUseCase;

public class RequestFileConversionUseCaseHandler : IRequestFileConversionUseCaseHandler
{
    private readonly IFileStorage _fileStorage;
    private readonly IConversionsRepository _conversionsRepository;
    private readonly IConversionJobPublisher _publisher;

    public RequestFileConversionUseCaseHandler(
        IFileStorage fileStorage,
        IConversionsRepository conversionsRepository,
        IConversionJobPublisher publisher)
    {
        _fileStorage = fileStorage;
        _conversionsRepository = conversionsRepository;
        _publisher = publisher;
    }

    public async Task<RequestFileConversionResult> ExecuteAsync(
        RequestFileConversionCommand command,
        CancellationToken cancellationToken)
    {
        var fileId = Guid.CreateVersion7();
        var conversionId = Guid.CreateVersion7();
        var createdAt = DateTime.UtcNow;
        var targetFormat = FileFormat.FromExtension(command.TargetFormat);

        var sourceStorageKey =
            $"{command.CallerId}/{fileId}/source.{command.File.FileFormat.Extension}";

        var outputStorageKey =
            $"{command.CallerId}/{conversionId}/result.{targetFormat.Extension}";

        var storedFile = StoredFileModel.Register(
            fileId,
            command.CallerId,
            command.File,
            sourceStorageKey,
            createdAt);

        var conversion = ConversionModel.Queue(
            conversionId,
            command.CallerId,
            fileId,
            command.File.FileFormat,
            targetFormat,
            createdAt);

        await _fileStorage.UploadFileAsync(
            sourceStorageKey,
            command.File,
            cancellationToken);

        try
        {
            await _conversionsRepository.CreateWithSourceFileAsync(
                conversion,
                storedFile,
                cancellationToken);
        }
        catch
        {
            await TryDeleteUploadedFileAsync(sourceStorageKey, cancellationToken);
            throw;
        }

        var message = new ConversionRequestedContract
        {
            MessageId = Guid.CreateVersion7(),
            ConversionId = conversionId,
            OriginalFileStorageKey = sourceStorageKey,
            OutputFileStorageKey = outputStorageKey,
            TargetExtension = targetFormat.Extension,
            RequestedAtUtc = DateTimeOffset.UtcNow
        };

        try
        {
            await _publisher.PublishAsync(message, cancellationToken);
        }
        catch
        {
            await _conversionsRepository.FailAsync(
                conversionId,
                "The conversion could not be queued.",
                cancellationToken);
            throw;
        }

        return new RequestFileConversionResult
        {
            ConversionId = conversionId,
            Status = ConversionStatus.Queued,
            CreatedAt = createdAt,
            SourceFile = storedFile.Metadata
        };
    }

    private async Task TryDeleteUploadedFileAsync(
        string storageKey,
        CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

}