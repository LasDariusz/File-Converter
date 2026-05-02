using Api.Application.Common;

namespace Api.Application.Features.Files.ConvertFile;

public class ConvertFileResponse
{
    public required FileMetadata SourceFile { get; set; }

    public required FileMetadata ConvertedFile {  get; set; }
}