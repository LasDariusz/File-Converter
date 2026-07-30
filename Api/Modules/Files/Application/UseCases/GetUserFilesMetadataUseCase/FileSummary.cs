using Api.Modules.Files.Domain.Models.Files;

namespace Api.Modules.Files.Application.UseCases.GetUserFilesMetadataUseCase;

public class FileSummary
{
    public Guid FileId { get; init; }

    public Guid OwnerId { get; init; }

    public required FileMetadata Metadata { get; init; }

    public string FileName => Metadata.FileName;

    public long SizeBytes => Metadata.SizeBytes;

    public FileFormat FileFormat => Metadata.FileFormat;

    public string ContentType => FileFormat.ContentType;

    public string Extension => FileFormat.Extension;

    public DateTime CreatedAt { get; init; }
}
