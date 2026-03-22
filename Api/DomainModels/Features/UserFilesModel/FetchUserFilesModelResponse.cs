using Api.DTOs.Features.FilesMetadata;

namespace Api.DomainModels.Features.UserFilesModel;

public class FetchUserFilesModelResponse
{
    public required IList<FileMetadataDto> Files { get; set; }
}
