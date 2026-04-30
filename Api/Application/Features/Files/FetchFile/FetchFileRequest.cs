namespace Api.Application.Features.Files.FetchFile;

public class FetchFileRequest
{
    public required Guid FileId { get; set; }

    public required Guid CallerId { get; set; }
}