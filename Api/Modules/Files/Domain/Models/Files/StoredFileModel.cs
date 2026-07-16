using Api.Modules.Files.Application.Common;
using Api.Modules.Files.Application.UseCases.GetUserFilesMetadataUseCase;
using Api.Modules.Files.Domain.Models.Files.Exceptions;

namespace Api.Modules.Files.Domain.Models.Files;

public class StoredFileModel
{
    public Guid FileId { get; }

    public Guid OwnerId { get; }

    public FileMetadata Metadata { get; }

    public string FileName => Metadata.FileName;

    public long SizeBytes => Metadata.SizeBytes;

    public FileFormat FileFormat => Metadata.FileFormat;

    public string ContentType => FileFormat.ContentType;

    public string Extension => FileFormat.Extension;

    public string StorageKey { get; }

    public DateTime CreatedAt { get; }

    private StoredFileModel(
       Guid fileId, Guid ownerId,
       FileMetadata metadata,
       string storageKey,
       DateTime createdAt)
    {
        FileId = fileId;
        OwnerId = ownerId;
        Metadata = metadata;
        StorageKey = storageKey;
        CreatedAt = createdAt;
    }

    public static StoredFileModel Register(
        Guid fileId, Guid ownerId,
        IncomingFile incomingFile,
        string storageKey,
        DateTime createdAt)
    {
        if (fileId == Guid.Empty)
            throw new ArgumentException();

        if (ownerId == Guid.Empty)
            throw new ArgumentException();

        if (string.IsNullOrWhiteSpace(storageKey))
            throw new InvalidFileException();

        return new StoredFileModel(
            fileId, ownerId,
            incomingFile.Metadata,
            storageKey,
            createdAt);
    }

    public static StoredFileModel Rehydrate(
        Guid fileId, Guid ownerId,
        FileMetadata metadata,
        string storageKey,
        DateTime createdAt)
    {
        return new StoredFileModel(
            fileId, ownerId,
            metadata,
            storageKey,
            createdAt);
    }

}