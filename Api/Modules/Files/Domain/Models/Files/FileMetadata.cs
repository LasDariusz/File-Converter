using Api.Modules.Files.Domain.Models.Files.Exceptions;

namespace Api.Modules.Files.Domain.Models.Files;

public record class FileMetadata
{
    public required string FileName { get; set; }

    public required long SizeBytes { get; set; }

    public required FileFormat FileFormat { get; set; }

    public static FileMetadata Create(
        string fileName,
        long sizeBytes)
    {
        var safeFileName = Path.GetFileName(fileName.Trim());

        if (string.IsNullOrWhiteSpace(safeFileName))
            throw new InvalidFileException();

        if (sizeBytes <= 0 )
            throw new InvalidFileException();

        return new FileMetadata
        {
            FileName = fileName,
            SizeBytes = sizeBytes,
            FileFormat = FileFormat.FromFileName(safeFileName),

        };
    }

}