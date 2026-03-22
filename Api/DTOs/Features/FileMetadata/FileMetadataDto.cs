namespace Api.DTOs.Features.FilesMetadata;

public class FileMetadataDto
{
    public Guid id { get; set; }
    public string fileName { get; set; }
    public string ContentType { get; set; } 
    public long FileSizeBytes { get; set; }
    public string Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public string DownloadUrl { get; set; }
    public string? SourceFileDownloadUrl { get; set; }
}