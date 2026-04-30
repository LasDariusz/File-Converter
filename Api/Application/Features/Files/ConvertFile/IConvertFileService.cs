namespace Api.Application.Features.Files.ConvertFile;

public interface IConvertFileService
{
    Task<ConvertFileResponse> ConvertFileAsync(ConvertFileRequest req, CancellationToken cancellation);
}