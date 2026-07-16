namespace Api.Modules.Files.Presentation.Responses;

public class GetFilesMetadataResponseDto
{
    public required IEnumerable<FileMetadataDto> FilesMetadata { get; set; }
}

public class FileMetadataDto
{
    public required Guid FileId { get; set; }

    public required string FileName { get; set; }

    public required string ContentType { get; set; }

    public required long FileSizeBytes { get; set; }

    public string? Status { get; set; }

    public DateTime? CreatedAt { get; set; }

    public required string DownloadUrl { get; set; }

    public string? SourceFileDownloadUrl { get; set; }
}