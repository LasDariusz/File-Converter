using System.ComponentModel.DataAnnotations;

namespace Api.Modules.Files.Presentation.Requests;

public sealed class FailConversionRequestDto
{
    [Required]
    public required string Error { get; init; }
}
