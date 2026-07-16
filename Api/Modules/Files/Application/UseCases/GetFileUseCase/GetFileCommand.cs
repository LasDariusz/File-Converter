namespace Api.Modules.Files.Application.UseCases.GetFileUseCase;

public record class GetFileCommand
{
    public Guid FileId { get; init; }

    public Guid CallerId { get; init; }
}