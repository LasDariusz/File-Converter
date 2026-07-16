
namespace Api.Modules.Files.Presentation.Responses;

public class ConvertFileResponseDto
{
    public required FileMetadataDto SourceFile { get; set; }

    public required FileMetadataDto ConvertedFile { get; set; }
}