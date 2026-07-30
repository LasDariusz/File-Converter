using Api.Modules.Files.Application.Common;

namespace Api.Modules.Files.Application.UseCases.RequestFileConversionUseCase;

public class RequestFileConversionCommand
{
    public required Guid CallerId { get; set; }

    public required IncomingFile File { get; set; }

    public required string TargetFormat { get; set; }
}