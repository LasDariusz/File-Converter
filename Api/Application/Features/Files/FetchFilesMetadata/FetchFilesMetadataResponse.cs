using Api.Application.Common;

namespace Api.Application.Features.Files.FetchFilesMetadata;

public class FetchFilesMetadataResponse
{
    public required IEnumerable<FileMetadata> FilesMetadata { get; set; }
}