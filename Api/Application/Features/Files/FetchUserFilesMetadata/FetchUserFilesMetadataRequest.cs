namespace Api.Application.Features.Files.FetchUserFilesMetadata;

public class FetchUserFilesMetadataRequest
{
    public required Guid UserId { get; set; }
}