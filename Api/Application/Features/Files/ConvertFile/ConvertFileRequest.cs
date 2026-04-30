namespace Api.Application.Features.Files.ConvertFile;

public class ConvertFileRequest
{
    public required Func<Stream> OpenStream { get; set; }

    public required string TargetExtension { get; set; }

    public required string ContentType { get; set; }

    public required string FileName { get; set; }

    public required Guid CallerId { get; set; }
}