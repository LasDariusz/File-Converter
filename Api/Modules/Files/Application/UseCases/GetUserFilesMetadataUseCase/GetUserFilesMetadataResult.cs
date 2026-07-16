using Api.Modules.Files.Application.UseCases.GetUserFilesMetadataUseCase;

namespace Api.Modules.Files.Application.UseCases.GetFileDetailsUseCase;

public class GetUserFilesMetadataResult
{
    public required IReadOnlyList<FileSummary> Files { get; set; }
}