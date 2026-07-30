namespace Api.Modules.Files.Presentation.Responses;

public sealed class GetConversionStatusResponseDto
{
    public required Guid ConversionId { get; init; }
    public required ConversionStatusDto Status { get; init; }
    public Guid? OutputFileId { get; init; }
    public string? Error { get; init; }
    public required DateTime CreatedAt { get; init; }
    public DateTime? StartedAt { get; init; }
    public DateTime? FinishedAt { get; init; }
}
