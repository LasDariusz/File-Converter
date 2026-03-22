namespace Api.DomainModels.Features.FetchFile;

public class FetchFileModelResponse
{
    public required Stream FileStream { get; set; }
    public required string ContentType { get; set; }
    public required string FileName { get; set; }
}