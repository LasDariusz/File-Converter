using Api.DTOs.Shared;

namespace Api.DTOs.Responses;

public class ConvertFileResponseDto
{
    public required FileMetadataDto SourceFile { get; set; }

    public required FileMetadataDto ConvertedFile { get; set; }
}