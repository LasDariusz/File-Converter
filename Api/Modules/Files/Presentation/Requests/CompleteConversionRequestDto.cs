using System.ComponentModel.DataAnnotations;

namespace Api.Modules.Files.Presentation.Requests;

public sealed class CompleteConversionRequestDto
{
    [Required]
    public required string OutputStorageKey { get; init; }

    [Required]
    public required string Extension { get; init; }

    [Required]
    public required string ContentType { get; init; }

    [Range(1, long.MaxValue)]
    public required long SizeBytes { get; init; }
}
