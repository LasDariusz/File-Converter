using Api.Modules.Files.Domain.Models;
using System.ComponentModel.DataAnnotations;

namespace Api.Modules.Files.Presentation.Requests;

public record class CreateConversionRequestDto
{
    [Required]
    public required IFormFile File { get; init; }

    [Required]
    public required string TargetFormat { get; init; }
}