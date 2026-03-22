using System.ComponentModel.DataAnnotations;
using Api.DTOs.ValidationAttributes;

namespace Api.DTOs.Features.ConvertFile;

public class ConvertFileRequestDto
{
    [Required]
    [AllowedFormats]
    public required string TargetFormat { get; set; }

    [Required]
    public required IFormFile FormFile { get; set; }
}
