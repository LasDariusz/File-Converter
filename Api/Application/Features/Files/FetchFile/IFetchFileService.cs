namespace Api.Application.Features.Files.FetchFile;

public interface IFetchFileService
{
    Task<FetchFileResponse> GetFileAsync(FetchFileRequest req);
}