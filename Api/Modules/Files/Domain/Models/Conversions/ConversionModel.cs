using Api.Modules.Files.Domain.Models.Files;

namespace Api.Modules.Files.Domain.Models.Conversions;

public sealed class ConversionModel
{
    public Guid ConversionId { get; }
    public Guid OwnerId { get; }
    public Guid SourceFileId { get; }
    public Guid? OutputFileId { get; }
    public FileFormat SourceFormat { get; }
    public FileFormat OutputFormat { get; }
    public ConversionStatus Status { get; }
    public string? ErrorMessage { get; }
    public int AttemptCount { get; }
    public DateTime CreatedAt { get; }
    public DateTime? StartedAt { get; }
    public DateTime? FinishedAt { get; }

    private ConversionModel(
        Guid conversionId,
        Guid ownerId,
        Guid sourceFileId,
        Guid? outputFileId,
        FileFormat sourceFormat,
        FileFormat outputFormat,
        ConversionStatus status,
        string? errorMessage,
        int attemptCount,
        DateTime createdAt,
        DateTime? startedAt,
        DateTime? finishedAt)
    {
        ConversionId = conversionId;
        OwnerId = ownerId;
        SourceFileId = sourceFileId;
        OutputFileId = outputFileId;
        SourceFormat = sourceFormat;
        OutputFormat = outputFormat;
        Status = status;
        ErrorMessage = errorMessage;
        AttemptCount = attemptCount;
        CreatedAt = createdAt;
        StartedAt = startedAt;
        FinishedAt = finishedAt;
    }

    public static ConversionModel Queue(
        Guid conversionId,
        Guid ownerId,
        Guid sourceFileId,
        FileFormat sourceFormat,
        FileFormat outputFormat,
        DateTime createdAt)
    {
        if (conversionId == Guid.Empty || ownerId == Guid.Empty || sourceFileId == Guid.Empty)
            throw new ArgumentException("Conversion identifiers cannot be empty.");

        return new ConversionModel(
            conversionId,
            ownerId,
            sourceFileId,
            null,
            sourceFormat,
            outputFormat,
            ConversionStatus.Queued,
            null,
            0,
            createdAt,
            null,
            null);
    }

    public static ConversionModel Rehydrate(
        Guid conversionId,
        Guid ownerId,
        Guid sourceFileId,
        Guid? outputFileId,
        FileFormat sourceFormat,
        FileFormat outputFormat,
        ConversionStatus status,
        string? errorMessage,
        int attemptCount,
        DateTime createdAt,
        DateTime? startedAt,
        DateTime? finishedAt)
    {
        return new ConversionModel(
            conversionId,
            ownerId,
            sourceFileId,
            outputFileId,
            sourceFormat,
            outputFormat,
            status,
            errorMessage,
            attemptCount,
            createdAt,
            startedAt,
            finishedAt);
    }
}
