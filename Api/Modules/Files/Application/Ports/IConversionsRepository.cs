using Api.Modules.Files.Domain.Models.Conversions;
using Api.Modules.Files.Domain.Models.Files;

namespace Api.Modules.Files.Application.Ports;

public interface IConversionsRepository
{
    Task<ConversionModel> CreateWithSourceFileAsync(
        ConversionModel conversion,
        StoredFileModel sourceFile,
        CancellationToken cancellationToken);

    Task<ConversionModel?> FindForOwnerAsync(
        Guid conversionId,
        Guid ownerId,
        CancellationToken cancellationToken);

    Task<ConversionStartResult?> TryStartAsync(
        Guid conversionId,
        CancellationToken cancellationToken);

    Task<bool> CompleteAsync(
        Guid conversionId,
        ConversionCompletion completion,
        CancellationToken cancellationToken);

    Task<bool> FailAsync(
        Guid conversionId,
        string errorMessage,
        CancellationToken cancellationToken);
}

public sealed record ConversionStartResult(
    bool ShouldProcess,
    ConversionStatus Status);

public sealed record ConversionCompletion(
    string OutputStorageKey,
    string Extension,
    string ContentType,
    long SizeBytes);
