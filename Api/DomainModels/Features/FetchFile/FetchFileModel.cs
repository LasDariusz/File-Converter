namespace Api.DomainModels.Features.FetchFile;

public class FetchFileModel
{
    public required Guid FileId { get; set; }

    public required Guid UserId { get; set; }
}
