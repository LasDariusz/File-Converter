using Api.DTOs.Shared;

namespace Api.DTOs.Responses;

public class FetchFilesMetadataResponseDto
{
    public required IEnumerable<FileMetadataDto> FilesMetadata { get; set; }
}