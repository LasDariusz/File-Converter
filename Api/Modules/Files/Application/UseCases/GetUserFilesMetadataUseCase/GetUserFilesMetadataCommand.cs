namespace Api.Modules.Files.Application.UseCases.GetFileDetailsUseCase;

public record class GetUserFilesMetadataCommand
{
    public Guid UserId { get; init; }

    public Guid CallerId { get; init; }
}