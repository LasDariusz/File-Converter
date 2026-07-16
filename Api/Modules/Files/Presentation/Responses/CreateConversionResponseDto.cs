namespace Api.Modules.Files.Presentation.Responses;

public record class CreateConversionResponseDto
{
    public required Guid ConversionId { get; init; }

    public required ConversionStatusDto Status { get; init; }

    public required string StatusUrl { get; init; }

    public required DateTime CreatedAt {  get; init; }
}

public enum ConversionStatusDto
{
    Queued,
    Processing,
    Completed,
    Failed,
    Cancelled
}