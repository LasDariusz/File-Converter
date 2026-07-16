using Api.Application.Common;

namespace Api.Modules.Files.Application.UseCases.RequestFileConversionUseCase;

public class RequestFileConversionResult
{
    public required FileMetadata SourceFile { get; set; }

    public required FileMetadata ConvertedFile { get; set; }
}