namespace Api.Application.Features.Files.FetchFile;

public class FetchFileResponse
{
    public required Stream FileStream { get; set; }

    public required string ContentType { get; set; }

    public required Guid FileId { get; set; }
}