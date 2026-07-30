using Api.Modules.Files.Domain.Models.Conversions;

namespace Api.Modules.Files.Application.UseCases.GetConversionStatusUseCase;

public sealed class GetConversionStatusResult
{
    public required Guid ConversionId { get; init; }
    public required ConversionStatus Status { get; init; }
    public Guid? OutputFileId { get; init; }
    public string? ErrorMessage { get; init; }
    public required DateTime CreatedAt { get; init; }
    public DateTime? StartedAt { get; init; }
    public DateTime? FinishedAt { get; init; }
}
