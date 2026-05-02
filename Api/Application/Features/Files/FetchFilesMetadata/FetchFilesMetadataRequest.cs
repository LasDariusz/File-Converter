namespace Api.Application.Features.Files.FetchFilesMetadata;

public class FetchFilesMetadataRequest
{
    public Guid? FileId { get; set; }

    public Guid UserId { get; set; }
}