namespace Api.Modules.Files.Infrastructure.Messaging.Contracts;

public class ConversionRequestedContract
{
    public required Guid MessageId { get; init; }

    public required Guid ConversionId { get; init; }

    public required string OriginalFileStorageKey { get; init; }

    public required string OutputFileStorageKey { get; init; }

    public required string TargetExtension { get; init; }

    public required DateTimeOffset RequestedAtUtc { get; init; }
}