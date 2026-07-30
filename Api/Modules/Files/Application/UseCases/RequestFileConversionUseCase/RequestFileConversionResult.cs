using Api.Modules.Files.Domain.Models.Conversions;
using Api.Modules.Files.Domain.Models.Files;

namespace Api.Modules.Files.Application.UseCases.RequestFileConversionUseCase;

public sealed class RequestFileConversionResult
{
    public required Guid ConversionId { get; init; }
    public required ConversionStatus Status { get; init; }
    public required DateTime CreatedAt { get; init; }
    public required FileMetadata SourceFile { get; init; }
}
