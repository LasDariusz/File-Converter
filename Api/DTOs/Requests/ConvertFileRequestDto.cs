using System.ComponentModel.DataAnnotations;
using Api.DTOs.ValidationAttributes;


namespace Api.DTOs.Requests;

public class ConvertFileRequestDto
{
    [Required]
    [AllowedFormats]
    public required string TargetExtension { get; set; }

    [Required]
    public required IFormFile FormFile { get; set; }
}