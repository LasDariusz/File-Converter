namespace Api.Application.Features.Files.FetchFilesMetadata;

public interface IFetchFilesMetadataService
{
    Task<FetchFilesMetadataResponse> GetFilesMetadataAsync(FetchFilesMetadataRequest req);
}