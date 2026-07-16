namespace Api.Modules.Files.Application.UseCases.GetFileUseCase;

public record class GetFileResult
{
    public required Guid FileId { get; init; }

    public required string ContentType { get; init; }

    public required Stream FileStream { get; init; }
}