using System.ComponentModel.DataAnnotations;
using Api.DTOs.ValidationAttributes;

namespace Api.DTOs.Features.UploadFile;

public class CreateNewFileRequestDto
{
    [Required, AllowedFormats]
    public required string FileFormat { get; set; }

    [Required]
    public required IFormFile FormFile { get; set; }
}
