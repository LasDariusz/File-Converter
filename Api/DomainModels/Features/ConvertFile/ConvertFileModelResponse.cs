namespace Api.DomainModels.Features.ConvertFile;

public class ConvertFileModelResponse
{
    public required Stream OutputStream { get; set; }

    public required string ContentType { get; set; }
}
