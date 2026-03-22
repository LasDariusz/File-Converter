namespace Api.DomainModels.Features.ConvertFile;

public class ConvertFileModel
{
    public required Guid UserId { get; set; }
    public required Func<Stream> OpenStream { get; set; }
    public required string FileName { get; set; }
    public required string TargetExtension { get; set; }
    public required string ContentType { get; set; }
}
