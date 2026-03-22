namespace Api.DomainModels.Features.UploadFile;

public class UploadFileModel
{
    public required Func<Stream> OpenStream { get; set; }
    public required string ContentType { get; set; }
    public required string FileName { get; set; }
    public required long LengthBytes { get; set; }
}